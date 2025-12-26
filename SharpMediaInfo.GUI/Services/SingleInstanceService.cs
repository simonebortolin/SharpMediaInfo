using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using SysException = System.Exception;

namespace SharpMediaInfo.GUI.Services;

/// <summary>
/// Cross-platform single instance service using Named Pipes and Polly for resilience.
/// Works on Windows, Linux, and macOS.
/// </summary>
public class SingleInstanceService : ISingleInstanceService
{
    private const string MasterPipeName = "SharpMediaInfo_Master";
    private readonly AsyncRetryPolicy _electionPolicy;
    private NamedPipeServerStream? _masterServer;
    private CancellationTokenSource? _cts;
    private TaskCompletionSource<bool>? _serverReadyTcs;

    public SingleInstanceService()
    {
        // Polly retry policy with exponential backoff + jitter
        _electionPolicy = Policy
            .Handle<TimeoutException>()
            .WaitAndRetryForeverAsync(
                sleepDurationProvider: attempt =>
                {
                    var baseDelay = TimeSpan.FromSeconds(1);
                    var jitter = TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000));
                    return baseDelay + jitter; // 1-2 sec with jitter
                },
                onRetry: (exception, timespan) =>
                {
                    Debug.WriteLine($"Master heartbeat failed, retry in {timespan.TotalSeconds:F1}s");
                });
    }

    public bool ShouldCheckSingleInstance(string[] args)
    {
        // Single instance check ONLY if NO --window-position
        // Drag & drop instances skip initial check but still participate in election
        return !args.Contains("--window-position");
    }

    public async Task<bool> TrySendToMasterAsync(string[] filePaths)
    {
        try
        {
            using var client = new NamedPipeClientStream(
                ".",
                MasterPipeName,
                PipeDirection.Out);

            await client.ConnectAsync(1000); // 1 second timeout

            var message = new PipeMessage { Files = filePaths ?? [] };
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            await client.WriteAsync(bytes, 0, bytes.Length);
            await client.FlushAsync();

            Debug.WriteLine($"Sent {filePaths?.Length ?? 0} file(s) to master (or just activated)");
            return true; // Master found!
        }
        catch (TimeoutException)
        {
            Debug.WriteLine("No master found - becoming master");
            return false; // No master - we become one!
        }
        catch (SysException ex)
        {
            Debug.WriteLine($"Error sending to master: {ex.Message}");
            return false;
        }
    }

    public async Task StartServerAsync(Func<string[], Task> onFilesReceived)
    {
        _cts = new CancellationTokenSource();
        _serverReadyTcs = new TaskCompletionSource<bool>();

        // Try to become master immediately
        if (await TryBecomeMasterAsync())
        {
            Debug.WriteLine("Became master instance");
            StartMasterServer(onFilesReceived);
            
            // Wait for server to be ready
            await _serverReadyTcs.Task;
        }
        else
        {
            Debug.WriteLine("Master exists - starting election monitor");
            // Start election monitor with Polly
            StartElectionMonitor(onFilesReceived);
            _serverReadyTcs.SetResult(true); // No server to wait for
        }
    }

    public async Task StopServerAsync()
    {
        _cts?.Cancel();
        _masterServer?.Dispose();
        await Task.CompletedTask;
    }

    private async Task<bool> TryBecomeMasterAsync()
    {
        try
        {
            _masterServer = new NamedPipeServerStream(
                MasterPipeName,
                PipeDirection.InOut,
                1, // Max 1 client at a time
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous);

            return true; // Successfully created master pipe!
        }
        catch (IOException)
        {
            return false; //Master already exists
        }
    }

    private async void StartMasterServer(Func<string[], Task> onFilesReceived)
    {
        await Task.Run(async () =>
        {
            // Signal that server is ready to accept connections
            _serverReadyTcs?.SetResult(true);
            
            while (!_cts?.Token.IsCancellationRequested == true)
            {
                try
                {
                    await _masterServer!.WaitForConnectionAsync(_cts?.Token ?? default);

                    using var reader = new StreamReader(_masterServer);
                    var json = await reader.ReadToEndAsync();
                    var message = JsonSerializer.Deserialize<PipeMessage>(json);

                    if (message?.Files != null && message.Files.Length > 0)
                    {
                        Debug.WriteLine($"Master received {message.Files.Length} file(s)");
                        await onFilesReceived(message.Files);
                    }
                    else
                    {
                        Debug.WriteLine($"Master received focus request (no files)");
                        await onFilesReceived(Array.Empty<string>());
                    }

                    _masterServer.Disconnect();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (SysException ex)
                {
                    Debug.WriteLine($"Master server error: {ex.Message}");
                    break;
                }
            }
        }, _cts?.Token ?? default);
    }

    private void StartElectionMonitor(Func<string[], Task> onFilesReceived)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                // Polly infinite retry loop - monitors master health
                await _electionPolicy.ExecuteAsync(async (ct) =>
                {
                    // Heartbeat check
                    using var client = new NamedPipeClientStream(
                        ".",
                        MasterPipeName,
                        PipeDirection.Out);

                    await client.ConnectAsync(300, ct); // 300ms quick check

                    // Master alive - throw to continue retry loop
                    throw new TimeoutException("Master still alive");

                }, _cts?.Token ?? default);
            }
            catch (OperationCanceledException)
            {
                // Shutdown requested
                return;
            }
            catch (TimeoutException)
            {
                // Master dead! Try to become master
                if (await TryBecomeMasterAsync())
                {
                    Debug.WriteLine("Elected as new master!");
                    StartMasterServer(onFilesReceived);
                }
                else
                {
                    // Lost election - restart monitor
                    Debug.WriteLine("Lost election, restarting monitor");
                    StartElectionMonitor(onFilesReceived);
                }
            }
        }, _cts?.Token ?? default);
    }

    private class PipeMessage
    {
        public string[] Files { get; set; } = [];
    }
}
