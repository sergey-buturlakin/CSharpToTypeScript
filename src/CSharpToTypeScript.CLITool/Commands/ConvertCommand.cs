using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSharpToTypeScript.CLITool.Conventions;
using CSharpToTypeScript.CLITool.Utilities;
using CSharpToTypeScript.Core.Options;
using CSharpToTypeScript.Core.Services;
using McMaster.Extensions.CommandLineUtils;

namespace CSharpToTypeScript.CLITool.Commands
{
    [Command(Name = "dotnet cs2ts", Description = "Convert C# Models, ViewModels and DTOs into their TypeScript equivalents"),
    Subcommand(typeof(InitializeCommand))]
    public class ConvertCommand : CommandBase
    {
        private readonly ICodeConverter _codeConverter;
        private readonly IFileNameConverter _fileNameConverter;

        public ConvertCommand(ICodeConverter codeConverter, IFileNameConverter fileNameConverter)
        {
            _codeConverter = codeConverter;
            _fileNameConverter = fileNameConverter;
        }

        public CodeConversionOptions CodeConversionOptions
            => new CodeConversionOptions(!SkipExport, UseTabs, TabSize, ConvertDatesTo, ConvertNullablesTo,
                !PreserveCasing, !PreserveInterfacePrefix,
                ImportGeneration, UseKebabCase, AppendModelSuffix, QuotationMark,
                OptionalReferenceTypes, NoSemicolon, EnumAsString);

        public void OnExecute()
        {
            if (AngularMode)
            {
                AngularConventions.Override(this);
            }

            if (ClearOutputDirectory && !string.IsNullOrWhiteSpace(Output)
            && Directory.Exists(Output) && !Output.IsSameOrParrentDirectory(Input))
            {
                Directory.Delete(Output, true);
            }

            if (Input.EndsWithFileExtension() && File.Exists(Input))
            {
                OnInputIsFile();
            }
            else if (!Input.EndsWithFileExtension() && Directory.Exists(Input))
            {
                OnInputIsDirectory();
            }
        }

        private void OnInputIsFile()
        {
            var exclusions = GetExclusions().TryGetValue(Input, out var set) ? set : null;

            if (exclusions == _ignoreFile)
                return;

            var content = File.ReadAllText(Input);
            var converted = _codeConverter.ConvertToTypeScript(content, CodeConversionOptions, name => exclusions?.Contains(name) != true);
            var outputPath = GetOutputFilePath(Input, Output, CodeConversionOptions);

            CreateOrUpdateFile(outputPath, converted, PartialOverride);
        }

        private void OnInputIsDirectory()
        {
            var exclusions = GetExclusions();

            var files = FileSystem.GetFilesWithExtension(Input, "cs")
                .Select(f =>
                {
                    var relativePath = Path.GetRelativePath(Input, f);

                    return new
                    {
                        InputPath = f,
                        RelativePath = relativePath,
                        Exclusions = exclusions.TryGetValue(relativePath, out var set) ? set : null
                    };
                })
                .Where(f => f.Exclusions != _ignoreFile)
                .Select(f => new
                {
                    f.InputPath,
                    f.RelativePath,
                    Content = _codeConverter.ConvertToTypeScript(File.ReadAllText(f.InputPath), CodeConversionOptions, name => f.Exclusions?.Contains(name) != true)
                })
                .Where(f => !string.IsNullOrWhiteSpace(f.Content));

            if (SingleFile)
            {
                string outputFileName;
                if (Output == null)
                {
                    var inputDir = new DirectoryInfo(Input);
                    outputFileName = Path.Combine(inputDir.FullName, _fileNameConverter.ConvertToTypeScript(inputDir.Name, CodeConversionOptions));
                }
                else
                {
                    outputFileName = Output;
                    Directory.CreateDirectory(outputFileName.ContainingDirectory());
                }
                var content = string.Join("\n\n\n", files.Select(f => $"// {f.RelativePath}\n\n{f.Content}"));
                CreateOrUpdateFile(outputFileName, content, PartialOverride);
                return;
            }

            var outputFiles = files
                .Select(f => new
                {
                    OutputPath = GetOutputFilePath(f.InputPath, Output, CodeConversionOptions),
                    f.Content
                })
                .GroupBy(f => f.OutputPath)
                .Select(g => g.First());

            foreach (var file in outputFiles)
            {
                CreateOrUpdateFile(file.OutputPath, file.Content, PartialOverride);
            }
        }

        private void CreateOrUpdateFile(string path, string content, bool partialOverride)
        {
            Directory.CreateDirectory(path.ContainingDirectory());

            if (partialOverride)
            {
                content = Marker.Update(File.Exists(path) ? File.ReadAllText(path) : string.Empty, content);
            }

            File.WriteAllText(path, content);
        }

        private Dictionary<string, HashSet<string>> GetExclusions()
        {
            var result = new Dictionary<string, HashSet<string>>();

            if (string.IsNullOrWhiteSpace(ExclusionFile))
                return result;

            foreach (var line in File.ReadLines(ExclusionFile))
            {
                var parts = line.Split(':', 2);

                if (parts.Length == 1)
                {
                    result[parts[0]] = _ignoreFile;
                    continue;
                }

                if (result.TryGetValue(parts[0], out var set))
                {
                    if (set == _ignoreFile)
                        continue;
                }
                else
                {
                    set = new HashSet<string>();
                    result.Add(parts[0], set);
                }

                set.UnionWith(parts[1].Split(',').Where(s => !string.IsNullOrWhiteSpace(s)));
            }

            return result;
        }

        private string GetOutputFilePath(string input, string output, ModuleNameConversionOptions options)
            => !input.EndsWithFileExtension() ? throw new ArgumentException("Input should end with file extension.")
            : output?.EndsWithFileExtension() == true ? output
            : !string.IsNullOrWhiteSpace(output) ? Path.Join(output, _fileNameConverter.ConvertToTypeScript(input, options))
            : Path.Join(input.ContainingDirectory(), _fileNameConverter.ConvertToTypeScript(input, options));

        private static readonly HashSet<string> _ignoreFile = new HashSet<string>();
    }
}