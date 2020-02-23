module BinaryObjectMapper2.Mapping

open BinaryObjectMapper2.FSharp.Mapping
open Microsoft.CodeAnalysis

let ProcessTree (syntaxTree: SyntaxTree) =
    processTree syntaxTree
    |>Option.toObj