module BinaryObjectMapper.Mapping

open BinaryObjectMapper.FSharp.Mapping
open Microsoft.CodeAnalysis

let ProcessProject (proj: Project) =
    processProject proj