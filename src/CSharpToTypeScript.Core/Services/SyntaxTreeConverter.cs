using System;
using System.Collections.Generic;
using System.Linq;
using CSharpToTypeScript.Core.Models;

using Microsoft.CodeAnalysis;
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

        public FileNode Convert(CompilationUnitSyntax root, Func<string, bool> predicate)
            => new FileNode(ConvertRootNodes(root, predicate));

        private IEnumerable<RootNode> ConvertRootNodes(CompilationUnitSyntax root, Func<string, bool> predicate)
            => root.DescendantNodes()
                .Where(node => 
                    (node is TypeDeclarationSyntax type && IsNotStatic(type) || node is EnumDeclarationSyntax)
                    && predicate(((BaseTypeDeclarationSyntax) node).Identifier.ValueText)
                )
                .Select(node => node switch
                {
                    TypeDeclarationSyntax type => (RootNode)_rootTypeConverter.Convert(type),
                    EnumDeclarationSyntax @enum => _rootEnumConverter.Convert(@enum),
                    _ => throw new ArgumentException("Unknown syntax type.")
                });

        private bool IsNotStatic(TypeDeclarationSyntax type)
            => type.Modifiers.All(m => m.Kind() != SyntaxKind.StaticKeyword);
    }
}