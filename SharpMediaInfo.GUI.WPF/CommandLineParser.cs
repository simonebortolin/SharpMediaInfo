using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMediaInfo.GUI.WPF;

/// <summary>
/// Command line argument parser with proper validation
/// </summary>
public class CommandLineParser
{
    public class ParsedArguments
    {
        public (int X, int Y)? WindowPosition { get; set; }
        public string[] Files { get; set; } = Array.Empty<string>();
        public bool HasErrors { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public static ParsedArguments Parse(string[] args)
    {
        var result = new ParsedArguments();
        var files = new List<string>();

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            // Parse --window-position X,Y
            if (arg == "--window-position" || arg == "-p")
            {
                if (i + 1 >= args.Length)
                {
                    result.Errors.Add($"Missing value for {arg}");
                    result.HasErrors = true;
                    i++;
                    continue;
                }

                var posValue = args[i + 1];
                var coords = posValue.Split(',');

                if (coords.Length != 2)
                {
                    result.Errors.Add($"Invalid window position format: '{posValue}'. Expected: X,Y");
                    result.HasErrors = true;
                }
                else if (!int.TryParse(coords[0], out int x))
                {
                    result.Errors.Add($"Invalid X coordinate: '{coords[0]}'");
                    result.HasErrors = true;
                }
                else if (!int.TryParse(coords[1], out int y))
                {
                    result.Errors.Add($"Invalid Y coordinate: '{coords[1]}'");
                    result.HasErrors = true;
                }
                else
                {
                    result.WindowPosition = (x, y);
                }

                i++; // Skip next arg (position value)
            }
            // Parse file paths (non-option arguments)
            else if (!arg.StartsWith("-"))
            {
                if (System.IO.File.Exists(arg))
                {
                    files.Add(arg);
                }
                else
                {
                    result.Errors.Add($"File not found: '{arg}'");
                    // Not a critical error, just skip it
                }
            }
            // Unknown option
            else
            {
                result.Errors.Add($"Unknown option: '{arg}'");
                result.HasErrors = true;
            }
        }

        result.Files = files.ToArray();
        return result;
    }

    public static bool HasWindowPosition(string[] args)
    {
        return args.Contains("--window-position") || args.Contains("-p");
    }

    public static void PrintUsage()
    {
        Console.WriteLine("SharpMediaInfo GUI - MediaInfo viewer with MVVM architecture");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  SharpMediaInfo.GUI.WPF.exe [options] [files...]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --window-position, -p <X,Y>  Set window position (for drag & drop)");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  files                        Media files to analyze");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  SharpMediaInfo.GUI.WPF.exe video.mp4");
        Console.WriteLine("  SharpMediaInfo.GUI.WPF.exe video.mp4 audio.flac");
        Console.WriteLine("  SharpMediaInfo.GUI.WPF.exe --window-position 1920,100 video.mp4");
    }
}
