using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using VerifyTests;

namespace SharpMediaInfo.Generator.Tests
{
    public class CsvSourceGeneratorTest
    {
        [Test]
        public async Task WithoutFilesShoulDoNothing()
        {

            var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: []);

            var csvSourceGenerator = new CsvSourceGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(csvSourceGenerator);

            var result = driver.RunGenerators(compilation).GetRunResult();

            await Assert.That(result.Diagnostics.Count()).IsEqualTo(0);
            await Assert.That(result.GeneratedTrees.Count()).IsEqualTo(0);
        }

        [Test]
        [Arguments("General")]
        [Arguments("Audio")]
        [Arguments("Image")]
        [Arguments("Menu")]
        [Arguments("Other")]
        [Arguments("Text")]
        [Arguments("Video")]
        [Arguments("InternalTest")]
        public async Task WithFilesShouldGenerateFiles(string track)
        {

            var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: []);

            var csvSourceGenerator = new CsvSourceGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(csvSourceGenerator);

            var text = File.ReadAllText($"Source/{track}.csv")?.ToString();
            driver = driver.AddAdditionalTexts([new InMemoryAdditionalText($"{track}.csv", text!)]);

            var result = driver.RunGenerators(compilation).GetRunResult();
            var generatorResult = result.Results.FirstOrDefault();

            await Assert.That(result.Diagnostics.Count()).IsEqualTo(0);
            await Assert.That(result.GeneratedTrees.Count()).IsEqualTo(2);
            await Assert.That(NormalizePath(result.GeneratedTrees[0].FilePath)).IsEqualTo($"SharpMediaInfo.Generator\\SharpMediaInfo.Generator.CsvSourceGenerator\\SharpMediaInfo.Tracks.{track}Track.g.cs");
            await Assert.That(NormalizePath(result.GeneratedTrees[1].FilePath)).IsEqualTo($"SharpMediaInfo.Generator\\SharpMediaInfo.Generator.CsvSourceGenerator\\SharpMediaInfo.MediaInfoBuilder.{track}.g.cs");
            await Verify(generatorResult.GeneratedSources, SnapshotSettings());
        }

        [Test]
        [Arguments("General")]
        [Arguments("Audio")]
        [Arguments("Image")]
        [Arguments("Menu")]
        [Arguments("Other")]
        [Arguments("Text")]
        [Arguments("Video")]
        [Arguments("InternalTest")]
        public async Task WithFilesAndOverrideShouldGenerateFiles(string track)
        {

            var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: []);

            var csvSourceGenerator = new CsvSourceGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(csvSourceGenerator);

            var text = File.ReadAllText($"Source/{track}.csv")?.ToString();
            var @override = File.ReadAllText($"Source/{track}Override.json")?.ToString();


            driver = driver.AddAdditionalTexts([new InMemoryAdditionalText($"{track}.csv", text!), new InMemoryAdditionalText($"{track}Override.json", @override!)]);

            var result = driver.RunGenerators(compilation).GetRunResult();
            var generatorResult = result.Results.FirstOrDefault();

            await Assert.That(result.Diagnostics.Count()).IsEqualTo(0);
            await Assert.That(result.GeneratedTrees.Count()).IsEqualTo(2);
            await Assert.That(NormalizePath(result.GeneratedTrees[0].FilePath)).IsEqualTo($"SharpMediaInfo.Generator\\SharpMediaInfo.Generator.CsvSourceGenerator\\SharpMediaInfo.Tracks.{track}Track.g.cs");
            await Assert.That(NormalizePath(result.GeneratedTrees[1].FilePath)).IsEqualTo($"SharpMediaInfo.Generator\\SharpMediaInfo.Generator.CsvSourceGenerator\\SharpMediaInfo.MediaInfoBuilder.{track}.g.cs");
            await Verify(generatorResult.GeneratedSources, SnapshotSettings());
        }

        // Roslyn builds generated file paths with the platform separator; the expected paths and
        // snapshots use '\', so normalize to that to keep one set of snapshots for every OS.
        private static string NormalizePath(string path) => path.Replace('/', '\\');

        private static VerifySettings SnapshotSettings()
        {
            var settings = new VerifySettings();
            settings.ScrubLinesWithReplace(line =>
                line.TrimStart().StartsWith("FilePath: SharpMediaInfo.Generator", StringComparison.Ordinal)
                    ? NormalizePath(line)
                    : line);
            return settings;
        }

        internal class InMemoryAdditionalText(string path, string content) : AdditionalText
        {
            private readonly SourceText _content = SourceText.From(content, Encoding.UTF8);

            public override string Path { get; } = path;

            public override SourceText GetText(CancellationToken cancellationToken = default)
                => _content;
        }
    }
}
