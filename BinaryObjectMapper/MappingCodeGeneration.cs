using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;

namespace BinaryObjectMapper
{
    class MappingCodeGeneration
    {
        public SyntaxNode ProcessCode(SyntaxNode node)
        {
            var classGroups =
                node
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(
                    @class => @class.AttributeLists.Any(
                        attrList => attrList.Attributes.Any(
                            attr => attr.Name.ToString() == nameof(MappableTypeAttribute)
                            )
                        )
                    )
                .GroupBy(@class => @class.FindEnclosingNamespace());

            var newCompilationUnit = CSharpSyntaxTree.ParseText("").GetRoot() as CompilationUnitSyntax;

            foreach(var (@namespace, classes) in classGroups)
            {
                SyntaxNode parent;
                switch(@namespace)
                {

                    case null:
                        parent = newCompilationUnit;
                        break;
                    default:
                        var newns = SyntaxFactory.NamespaceDeclaration(@namespace.Name);
                        newCompilationUnit = newCompilationUnit.AddMembers(newns);
                        parent = newns;
                        break;
                }


                foreach(var @class in classes)
                {

                }
            }
            return newCompilationUnit;
        }


    }
}
