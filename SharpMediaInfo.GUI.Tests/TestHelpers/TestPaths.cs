namespace SharpMediaInfo.GUI.Tests.TestHelpers;

/// <summary>
/// Percorsi di test assoluti e nativi per il sistema corrente (C:\Videos\test.mp4 su Windows,
/// /Videos/test.mp4 altrove), così Path.GetFileName li interpreta correttamente ovunque.
/// </summary>
public static class TestPaths
{
    private static readonly string Root = Path.GetPathRoot(Path.GetTempPath())!;

    public static string Rooted(params string[] parts) => Path.Combine([Root, .. parts]);
}
