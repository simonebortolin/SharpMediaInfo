using SharpMediaInfo.GUI.Services;
using System.Windows;

namespace SharpMediaInfo.GUI.WPF.Services
{
    public class WpfClipboardService : IClipboardService
    {
        public void CopyText(string text)
        {
            Clipboard.SetText(text);
        }

        public string? GetText()
        {
            return Clipboard.ContainsText() ? Clipboard.GetText() : null;
        }
    }
}