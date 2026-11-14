import { dotnet } from './_framework/dotnet.js';

const dotnetRuntime = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

// Imported by Services/BrowserInterop.cs
dotnetRuntime.setModuleImports('main.js', {
    storage: {
        // localStorage throws when disabled (privacy settings, some private windows).
        getItem: (key) => {
            try { return globalThis.localStorage.getItem(key); } catch { return null; }
        },
        setItem: (key, value) => {
            try { globalThis.localStorage.setItem(key, value); return true; } catch { return false; }
        },
    },
    download: (fileName, mimeType, content) => {
        const blob = new Blob([content.slice()], { type: mimeType });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        a.remove();
        setTimeout(() => URL.revokeObjectURL(url), 60_000);
    },
});

const config = dotnetRuntime.getConfig();
await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
