using System.Text.Json.Serialization;

namespace SharpMediaInfo.Generator
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "OverrideType")]
    [JsonDerivedType(typeof(TrackFieldInfo), "Field")]
    [JsonDerivedType(typeof(MergeFieldInfo), "Merge")]
    public interface IOverridableField
    {
        string Name { get; set; }
        string? ParseFunction { get; set; }
        bool Deprecated { get; set; }
        string? Type { get; set; }
        string Description { get; set; }
    }

    public class MergeFieldInfo(string name, string? type, string description, string[] csvNames, string? parseFunction = null, bool deprecated = false) : IOverridableField
    {
        public string Name { get; set; } = name;
        public string? Type { get; set; } = type;
        public string Description { get; set; } = description;
        public string? ParseFunction { get; set; } = parseFunction;
        public bool Deprecated { get; set; } = deprecated;
        public string[] CsvNames { get; set; } = csvNames;
    }

    public class CombinedTrackFieldInfo(string name, string csvName, string type, string description, string parseFunction, TrackFieldInfo[] trackFieldInfos, string? uoM = null, bool deprecated = false, string modifier = "public", bool skip = false) : TrackFieldInfo(name, csvName, type, description, uoM, deprecated, parseFunction, modifier, skip)
    {
        public TrackFieldInfo[] TrackFieldInfos { get; set; } = trackFieldInfos;
    }

    public class TrackFieldInfo(string name, string csvName, string type, string description, string? uoM = null, bool deprecated = false, string? parseFunction = null, string modifier = "public", bool skip = false) : IOverridableField
    {
        public string Modifier { get; set; } = modifier;
        public string Name { get; set; } = name;
        public string CsvName { get; set; } = csvName;
        public string? Type { get; set; } = type;
        public string Description { get; set; } = description;
        public string? UoM { get; set; } = uoM;
        public string? ParseFunction { get; set; } = parseFunction;
        public bool Deprecated { get; set; } = deprecated;
        public bool Skip { get; set; } = skip;
    }
}
