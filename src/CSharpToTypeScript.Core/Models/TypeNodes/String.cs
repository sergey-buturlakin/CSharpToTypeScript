using CSharpToTypeScript.Core.Options;

namespace CSharpToTypeScript.Core.Models.TypeNodes
{
    internal class String : TypeNode
    {
        public override bool IsOptional(CodeConversionOptions options, out TypeNode of)
        {
            of = this;
            return options.OptionalReferenceTypes;
        }

        public override string WriteTypeScript(CodeConversionOptions options, Context context) => "string";
    }
}