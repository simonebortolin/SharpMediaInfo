using System.Globalization;
using System.Runtime.CompilerServices;

namespace SharpMediaInfo.Tests;

/// <summary>
/// Global test setup. Forces invariant culture so that the <c>ToString()</c>
/// formatting of the domain types (which rely on the current culture's number
/// formatting) is deterministic regardless of the machine locale.
/// </summary>
internal static class TestSetup
{
    [ModuleInitializer]
    public static void Initialize()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
    }
}
