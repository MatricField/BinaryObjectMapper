using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace TestExample
{
    class Program
    {
        static void Main(string[] args)
        {

            var tree = CSharpSyntaxTree.ParseText(
                @"namespace TestN { [MappableTypeAttribute]class TestC {} class TestA{}} [MappableTypeAttribute]class TestB{} class TestD{}");
            var ns = BinaryObjectMapper.Mapping.ProcessTree(tree);
            Console.WriteLine(ns.GetRoot().NormalizeWhitespace().ToString());
        }
    }
}
