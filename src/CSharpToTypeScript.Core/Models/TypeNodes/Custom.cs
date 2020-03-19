using System.Collections.Generic;
using System.Linq;
using CSharpToTypeScript.Core.Options;
using CSharpToTypeScript.Core.Utilities;

namespace CSharpToTypeScript.Core.Models.TypeNodes
{
    internal class Custom : NamedTypeNode
    {
        public Custom(string name) : base(name) { }

        public override IEnumerable<string> Requires => new[] { Name };

        public override bool IsOptional(CodeConversionOptions options, out TypeNode of)
        {
            of = this;
            return options.OptionalReferenceTypes;
        }

        public override string WriteTypeScript(CodeConversionOptions options, Context context)
            => Name.TransformIf(options.RemoveInterfacePrefix && !context.GenericTypeParameters.Contains(Name), StringUtilities.RemoveInterfacePrefix);
    }
}