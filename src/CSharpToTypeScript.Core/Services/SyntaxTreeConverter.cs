using System;
using System.Collections.Generic;
using System.Linq;
using CSharpToTypeScript.Core.Models;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpToTypeScript.Core.Services
{
    internal class SyntaxTreeConverter
    {
        private readonly RootTypeConverter _rootTypeConverter;
        private readonly RootEnumConverter _rootEnumConverter;

        public SyntaxTreeConverter(RootTypeConverter rootTypeConverter, RootEnumConverter rootEnumConverter)
        {
            _rootTypeConverter = rootTypeConverter;
            _rootEnumConverter = rootEnumConverter;
        }

        public FileNode Convert(CompilationUnitSyntax root, Func<string, IgnoreMode> predicate)
            => new FileNode(ConvertRootNodes(root, predicate));

        private IEnumerable<RootNode> ConvertRootNodes(CompilationUnitSyntax root, Func<string, IgnoreMode> predicate)
            => root.DescendantNodes()
                .Where(node => 
                    node is TypeDeclarationSyntax type && IsNotStatic(type) || node is EnumDeclarationSyntax
                )
                .Select(node =>
                {
                    var ignoreMode = predicate(((BaseTypeDeclarationSyntax) node).Identifier.ValueText);

                    if (ignoreMode == IgnoreMode.Exclude)
                        return null;

                    var rootNode = node switch
                    {
                        TypeDeclarationSyntax type => (RootNode) _rootTypeConverter.Convert(type),
                        EnumDeclarationSyntax @enum => _rootEnumConverter.Convert(@enum),
                        _ => throw new ArgumentException("Unknown syntax type.")
                    };

                    rootNode.TsIgnore = ignoreMode == IgnoreMode.Ignore;

                    return rootNode;
                })
                .Where(node => node != null);

        private bool IsNotStatic(TypeDeclarationSyntax type)
            => type.Modifiers.All(m => m.Kind() != SyntaxKind.StaticKeyword);
    }
}