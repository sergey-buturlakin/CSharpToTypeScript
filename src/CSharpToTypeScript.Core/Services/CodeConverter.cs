using System;

using CSharpToTypeScript.Core.Options;

using Microsoft.CodeAnalysis.CSharp;

namespace CSharpToTypeScript.Core.Services
{
    internal class CodeConverter : ICodeConverter
    {
        private readonly SyntaxTreeConverter _syntaxTreeConverter;

        public CodeConverter(SyntaxTreeConverter syntaxTreeConverter)
        {
            _syntaxTreeConverter = syntaxTreeConverter;
        }

        public string ConvertToTypeScript(string code, CodeConversionOptions options, Func<string, IgnoreMode> predicate)
            => _syntaxTreeConverter.Convert(CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot(), predicate)
                .WriteTypeScript(options);
    }
}
