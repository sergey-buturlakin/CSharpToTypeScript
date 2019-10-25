using System;
using System.IO;
using System.Linq;
using Xunit;

namespace CSharpToTypeScript.CLITool.Tests
{
    public class CLIShould : IClassFixture<CLIFixture>
    {
        private readonly CLI _cli;

        public CLIShould(CLIFixture fixture)
        {
            _cli = fixture.CLI;
        }

        private void Prepare(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }

            Directory.CreateDirectory(directory);
        }

        [Fact]
        public void ConvertSingleSimpleFile()
        {
            Prepare(nameof(ConvertSingleSimpleFile));
            var originalFilePath = Path.Join(nameof(ConvertSingleSimpleFile), "Item.cs");
            File.WriteAllText(originalFilePath, "class Item { }");

            _cli.Input = originalFilePath;

            _cli.OnExecute();

            var generatedFilePath = Path.Join(nameof(ConvertSingleSimpleFile), "item.ts");

            Assert.True(File.Exists(generatedFilePath));
            Assert.Equal("export interface Item {" + Environment.NewLine + Environment.NewLine + "}", File.ReadAllText(generatedFilePath));
        }

        [Fact]
        public void ConvertDirectory()
        {
            Prepare(nameof(ConvertDirectory));

            File.WriteAllText(Path.Join(nameof(ConvertDirectory), "File1.cs"), string.Empty);
            File.WriteAllText(Path.Join(nameof(ConvertDirectory), "File2.cs"), string.Empty);
            File.WriteAllText(Path.Join(nameof(ConvertDirectory), "File3.cs"), string.Empty);

            _cli.Input = nameof(ConvertDirectory);

            _cli.OnExecute();

            var convertedFiles = Directory.GetFiles(nameof(ConvertDirectory))
                .Where(f => f.EndsWith(".ts"))
                .Select(Path.GetFileName);

            Assert.Equal(new[] { "file1.ts", "file2.ts", "file3.ts" }, convertedFiles);
        }

        [Fact]
        public void ConvertSingleFileIntoProvidedOutputFile()
        {
            Prepare(nameof(ConvertSingleFileIntoProvidedOutputFile));

            var inputFilePath = Path.Join(nameof(ConvertSingleFileIntoProvidedOutputFile), "File.cs");
            var outputFilePath = Path.Join(nameof(ConvertSingleFileIntoProvidedOutputFile), "converted.ts");

            File.WriteAllText(inputFilePath, string.Empty);

            _cli.Input = inputFilePath;
            _cli.Output = outputFilePath;

            _cli.OnExecute();

            Assert.True(File.Exists(outputFilePath));
        }

        [Fact]
        public void ConvertSingleFileIntoProvidedOutputDirectory()
        {
            Prepare(nameof(ConvertSingleFileIntoProvidedOutputDirectory));

            var inputFilePath = Path.Join(nameof(ConvertSingleFileIntoProvidedOutputDirectory), "File.cs");
            var outputDirectoryPath = Path.Join(nameof(ConvertSingleFileIntoProvidedOutputDirectory), "models");
            File.WriteAllText(inputFilePath, string.Empty);

            _cli.Input = inputFilePath;
            _cli.Output = outputDirectoryPath;

            _cli.OnExecute();

            Assert.True(File.Exists(Path.Join(outputDirectoryPath, "file.ts")));
        }

        [Fact]
        public void ConvertDirectoryIntoProvidedOutputDirectory()
        {
            Prepare(nameof(ConvertDirectoryIntoProvidedOutputDirectory));

            var inputDirectoryPath = Path.Join(nameof(ConvertDirectoryIntoProvidedOutputDirectory), "Input");
            var outputDirectoryPath = Path.Join(nameof(ConvertDirectoryIntoProvidedOutputDirectory), "Output");

            Directory.CreateDirectory(inputDirectoryPath);

            File.WriteAllText(Path.Join(inputDirectoryPath, "File1.cs"), string.Empty);
            File.WriteAllText(Path.Join(inputDirectoryPath, "File2.cs"), string.Empty);
            File.WriteAllText(Path.Join(inputDirectoryPath, "File3.cs"), string.Empty);

            _cli.Input = inputDirectoryPath;
            _cli.Output = outputDirectoryPath;

            _cli.OnExecute();

            var convertedFiles = Directory.GetFiles(outputDirectoryPath)
                .Where(f => f.EndsWith(".ts"))
                .Select(Path.GetFileName);

            Assert.Equal(new[] { "file1.ts", "file2.ts", "file3.ts" }, convertedFiles);
        }

        [Fact]
        public void UseAngularConventionsWhenRequested()
        {
            Prepare(nameof(UseAngularConventionsWhenRequested));

            var originalFilePath = Path.Join(nameof(UseAngularConventionsWhenRequested), "ShoppingCartItem.cs");
            File.WriteAllText(originalFilePath, @"class ShoppingCartItem 
            {
                public int Id { get; set; }
            }");

            _cli.Input = originalFilePath;
            _cli.AngularMode = true;

            _cli.OnExecute();

            var generatedFilePath = Path.Join(nameof(UseAngularConventionsWhenRequested), "shopping-cart-item.model.ts");

            Assert.True(File.Exists(generatedFilePath));
            Assert.Equal(
                "export interface ShoppingCartItem {" + Environment.NewLine
                + "  id: number;" + Environment.NewLine
                + "}",
                File.ReadAllText(generatedFilePath));
        }
    }
}