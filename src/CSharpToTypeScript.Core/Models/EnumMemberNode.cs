using CSharpToTypeScript.Core.Options;
using CSharpToTypeScript.Core.Utilities;

namespace CSharpToTypeScript.Core.Models
{
    internal class EnumMemberNode : IWritableNode
    {
        public EnumMemberNode(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }

        public string WriteTypeScript(CodeConversionOptions options, Context context)
        {
            var name = Name.TransformIf(options.ToCamelCase, StringUtilities.ToCamelCase);

            return name + (
                !string.IsNullOrWhiteSpace(Value)
                    ? " = " + Value?.SquashWhistespace()
                    : options.EnumAsString
                        ? " = " + name.InQuotes(options.QuotationMark)
                        : string.Empty
            );
        }
    }
}