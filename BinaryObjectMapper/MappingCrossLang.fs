module BinaryObjectMapper.Mapping

open BinaryObjectMapper.FSharp.Mapping
open Microsoft.CodeAnalysis

let ProcessTree (syntaxTree: SyntaxTree) =
    processTree syntaxTree
    |>Option.toObj