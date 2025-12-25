namespace SharpMediaInfo.GUI.Models
{
    public class FileDialogOptions
    {
        public string Title { get; set; } = "Select a file";
        public FileType FileType { get; set; } = FileType.All;
        public string DefaultFileName { get; set; } = "file";
        public string DefaultExtension { get; set; } = "*.*";
        public bool AllowMultipleSelection { get; set; } = false;
    }
}