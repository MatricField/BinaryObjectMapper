using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BinaryObjectMapper
{
    public static class Helpers
    {
        public static NamespaceDeclarationSyntax FindEnclosingNamespace(this SyntaxNode @this) =>
            @this
                ?.SyntaxTree
                ?.GetRoot()
                ?.DescendantNodes()
                ?.OfType<NamespaceDeclarationSyntax>()
                ?.Where(@namespace => @namespace.Contains(@this))
                ?.FirstOrDefault();

        public static void Deconstruct<K, V>(this IGrouping<K, V> group, out K k, out IEnumerable<V> v)
        {
            k = group.Key;
            v = group;
        }
    }
}
