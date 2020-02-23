[<System.CLSCompliant(false)>]
module BinaryObjectMapper2.FSharp.Mapping

open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open Microsoft.CodeAnalysis
open System

module private Common =

    let baseList =
        "IMappableType"
        |>SyntaxFactory.ParseTypeName
        |>SyntaxFactory.SimpleBaseType
        :> BaseTypeSyntax
        |>SyntaxFactory.SingletonSeparatedList
        |>SyntaxFactory.BaseList

    let inline toMember x : MemberDeclarationSyntax = x

let rec processClass (``class``: ClassDeclarationSyntax) =
    let hasAttr (``class``: ClassDeclarationSyntax) =
        ``class``.AttributeLists
        |>Seq.exists (fun x -> x.Attributes |> Seq.exists (fun attr -> (string attr.Name) = "MappableTypeAttribute"))

    if hasAttr ``class`` then
        let subtypes =
            ``class``.Members
            |>Seq.choose tryProcessMember
            |>Seq.toArray
        SyntaxFactory
            .ClassDeclaration(``class``.Identifier)
            .WithModifiers(``class``.Modifiers)
            .WithBaseList(Common.baseList)
            .AddMembers(subtypes)
        |>Some
    else
        None

and processNamespace (``namespace``:NamespaceDeclarationSyntax) =
    let children =
        ``namespace``.ChildNodes()
        |>Seq.choose tryProcessMember
    if not (Seq.isEmpty children) then
        SyntaxFactory
            .NamespaceDeclaration(``namespace``.Name)
            .WithMembers(children |> SyntaxList)
        |>Some
    else
        None

and tryProcessMember (``member`` : SyntaxNode) =
    match ``member`` with
    | :? NamespaceDeclarationSyntax as ns ->
        processNamespace ns
        |>Option.map Common.toMember
    | :? ClassDeclarationSyntax as c ->
        processClass c
        |>Option.map Common.toMember
    | _ -> None

let processTree (syntaxTree: SyntaxTree) =
    match syntaxTree.GetRoot() with
    | :? CompilationUnitSyntax as compUnit ->
        let newMember =
            compUnit.Members
            |>Seq.choose tryProcessMember
            |>Seq.toArray
        SyntaxFactory
            .CompilationUnit()
            .AddMembers(newMember)
        |>SyntaxFactory.SyntaxTree
        |>Some
    |root ->
        root 
        |>tryProcessMember
        |> Option.map SyntaxFactory.SyntaxTree