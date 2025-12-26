namespace SharpMediaInfo.GUI.Models
{
    /// <summary>
    /// Specifies the type of file to be processed or filtered.
    /// </summary>
    /// <remarks>Use this enumeration to indicate the category of a file, such as media, text, or image. The
    /// All value can be used to represent any file type when filtering or processing collections of files.</remarks>
    public enum FileType
    {
        Media,
        Text,
        Image,
        All
    }
}