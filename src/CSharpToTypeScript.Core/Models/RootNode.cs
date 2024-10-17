using System.Collections.Generic;
using System.Linq;
using CSharpToTypeScript.Core.Options;

namespace CSharpToTypeScript.Core.Models
{
    internal abstract class RootNode : IWritableNode, IDependentNode
    {
        public bool TsIgnore { get; set; }

        public abstract string Name { get; }

        public virtual IEnumerable<string> Requires => Enumerable.Empty<string>();

        public abstract string WriteTypeScript(CodeConversionOptions options, Context context);
    }
}