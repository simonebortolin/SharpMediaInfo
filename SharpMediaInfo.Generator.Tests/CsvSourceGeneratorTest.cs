using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Text;

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
        public async Task WithFilesShouldGenerateFiles()
        {

            var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: []);

            var csvSourceGenerator = new CsvSourceGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(csvSourceGenerator);

            var text = File.ReadAllText("Source/General.csv")?.ToString();
            driver = driver.AddAdditionalTexts([new InMemoryAdditionalText("General.csv", text!)]);

            var result = driver.RunGenerators(compilation).GetRunResult();
            var generatorResult = result.Results.FirstOrDefault();

            await Assert.That(result.Diagnostics.Count()).IsEqualTo(0);
            await Assert.That(result.GeneratedTrees.Count()).IsEqualTo(1);
            await Assert.That(result.GeneratedTrees[0].FilePath).IsEqualTo("SharpMediaInfo.Generator\\SharpMediaInfo.Generator.CsvSourceGenerator\\SharpMediaInfo.Tracks.GeneralTrack.g.cs");
            await Verify(generatorResult.GeneratedSources);
        }

        [Test]
        public async Task WithFilesAndOverrideShouldGenerateFiles()
        {

            var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: []);

            var csvSourceGenerator = new CsvSourceGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(csvSourceGenerator);

            var text = File.ReadAllText("Source/General.csv")?.ToString();
            var @override = File.ReadAllText("Source/GeneralOverride.json")?.ToString();


            driver = driver.AddAdditionalTexts([new InMemoryAdditionalText("General.csv", text!), new InMemoryAdditionalText("GeneralOverride.json", @override!)]);

            var result = driver.RunGenerators(compilation).GetRunResult();
            var generatorResult = result.Results.FirstOrDefault();

            await Assert.That(result.Diagnostics.Count()).IsEqualTo(0);
            await Assert.That(result.GeneratedTrees.Count()).IsEqualTo(2);
            await Assert.That(result.GeneratedTrees[0].FilePath).IsEqualTo("SharpMediaInfo.Generator\\SharpMediaInfo.Generator.CsvSourceGenerator\\SharpMediaInfo.Tracks.GeneralTrack.g.cs");
            await Verify(generatorResult.GeneratedSources);
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
