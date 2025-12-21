using Microsoft.CodeAnalysis;
using System.Text.Json;

namespace SharpMediaInfo.Generator
{

    [Generator]
    public sealed class CsvSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context) {

            IncrementalValuesProvider<AdditionalText> textFiles = context.AdditionalTextsProvider.Where(file => file.Path.EndsWith(".csv"));
            IncrementalValuesProvider<AdditionalText> overrideFiles = context.AdditionalTextsProvider.Where(file => file.Path.EndsWith("Override.json"));

            IncrementalValuesProvider<(string name, string? content)> namesAndContents = textFiles.Select((text, cancellationToken) => (name: Path.GetFileNameWithoutExtension(text.Path), content: text.GetText(cancellationToken)?.ToString()));

            IncrementalValuesProvider<(string name, string content)> csvContents =
                textFiles.Select((text, cancellationToken) =>
                {
                    var content = text.GetText(cancellationToken)?.ToString() ?? string.Empty;
                    var name = Path.GetFileNameWithoutExtension(text.Path);
                    return (name, content);
                });

            IncrementalValuesProvider<(string name, string? content)> overrideContents =
                overrideFiles.Select((text, cancellationToken) =>
                {
                    var content = text.GetText(cancellationToken)?.ToString();
                    var name = Path.GetFileNameWithoutExtension(text.Path);
                    return (name, content);
                });

            IncrementalValuesProvider<(string name, string csvContent, string? overrideContent)> merged =
                csvContents
                .Combine(overrideContents.Collect())
                .Select((pair, cancellationToken) =>
                {
                    var (csv, overrides) = pair;
                    var matchingOverride = overrides.FirstOrDefault(o => o.name == csv.name + "Override");
                    return (csv.name, csv.content, matchingOverride.content);
                });

            context.RegisterSourceOutput(merged, (spc, nameAndContent) =>
            {
                var overrideFields = string.IsNullOrEmpty(nameAndContent.overrideContent) == false ? JsonSerializer.Deserialize<IOverridableField[]>(nameAndContent.overrideContent!) : null;

                var trackResult = GenerateTrackClass(nameAndContent.name, nameAndContent.csvContent, overrideFields);
                if (trackResult != null)
                    spc.AddSource($"SharpMediaInfo.Tracks.{nameAndContent.name}Track.g.cs", trackResult);

                var helperResult = GeneratePopulateMediaInfoClass(nameAndContent.name, nameAndContent.csvContent, overrideFields);
                if (helperResult != null)
                    spc.AddSource($"SharpMediaInfo.MediaInfo.{nameAndContent.name}.g.cs", helperResult);
            });
        }

        private IEnumerable<TrackFieldInfo> CalculateOverrideFields(List<TrackFieldInfo> fromCsv, IOverridableField[]? @override = null)
        {
            var mergedFields = new List<TrackFieldInfo>();
            foreach (var field in fromCsv)
            {
                if (@override != null)
                {
                    var matchingOverride = @override.FirstOrDefault(o => o is TrackFieldInfo tfi && tfi.CsvName == field.CsvName) as TrackFieldInfo;
                    if (matchingOverride != null)
                    {
                        field.Name = string.IsNullOrEmpty(matchingOverride.Name) ? field.Name : matchingOverride.Name;
                        field.Type = string.IsNullOrEmpty(matchingOverride.Type) ? field.Type : matchingOverride.Type;
                        field.Description = string.IsNullOrEmpty(matchingOverride.Description) ? field.Description : matchingOverride.Description;
                        field.UoM = string.IsNullOrEmpty(matchingOverride.UoM) ? field.UoM : matchingOverride.UoM;
                        field.Deprecated = matchingOverride.Deprecated;
                        field.ParseFunction = string.IsNullOrEmpty(matchingOverride.ParseFunction) ? field.ParseFunction : matchingOverride.ParseFunction;
                        field.Modifier = string.IsNullOrEmpty(matchingOverride.Modifier) ? field.Modifier : matchingOverride.Modifier;
                        field.Skip = matchingOverride.Skip;
                    }
                    var matchingOverrideMerge = @override.FirstOrDefault(o => o is MergeFieldInfo mfi && mfi.CsvNames.Contains(field.CsvName)) as MergeFieldInfo;
                    if (matchingOverrideMerge != null)
                    {
                        field.Skip = true;
                    }
                }
            }

            foreach(var mergeOverride in @override?.Where(o => o is MergeFieldInfo).Cast<MergeFieldInfo>() ?? Enumerable.Empty<MergeFieldInfo>())
            {
                var newField = new CombinedTrackFieldInfo
                (
                    name: mergeOverride.Name,
                    type: mergeOverride.Type!,
                    description: mergeOverride.Description,
                    csvName: string.Join("+", mergeOverride.CsvNames),
                    parseFunction: mergeOverride.ParseFunction!,
                    deprecated: mergeOverride.Deprecated,
                    trackFieldInfos: fromCsv.Where(f => mergeOverride.CsvNames.Contains(f.CsvName)).ToArray()
                );
                mergedFields.Add(newField);
            }

            return fromCsv.Union(mergedFields);
        }

        public string GenerateTrackClass(string name, string csvText, IOverridableField[]? @override = null)
        {
            var fields = CalculateOverrideFields(TrackFieldInfoParser.ParseCsv(csvText), @override);

            string code = TrackCodeBuilder.GenerateClass($"{name}Track", fields.Where(it => it.Skip == false));

            return code;
        }

        public string GeneratePopulateMediaInfoClass(string name, string csvText, IOverridableField[]? @override = null)
        {

            var fields = CalculateOverrideFields(TrackFieldInfoParser.ParseCsv(csvText), @override);

            string code = PopulateMediaInfoBuilder.GeneratePopulateMediaInfo($"{name}Track", fields.Where(it => it.Skip == false));

            return code;
        }
    }
}
