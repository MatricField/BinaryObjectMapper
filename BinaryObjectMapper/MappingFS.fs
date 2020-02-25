module BinaryObjectMapper.FSharp.Mapping

open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open Microsoft.CodeAnalysis
open System
open System.IO

module private Common =

    let baseList =
        "IMappableType"
        |>SyntaxFactory.ParseTypeName
        |>SyntaxFactory.SimpleBaseType
        :> BaseTypeSyntax
        |>SyntaxFactory.SingletonSeparatedList
        |>SyntaxFactory.BaseList

    let compilerGenerated = 
        "CompilerGenerated"
        |>SyntaxFactory.ParseName
        |>SyntaxFactory.Attribute
        |>SyntaxFactory.SingletonSeparatedList
        |>SyntaxFactory.AttributeList

    let usings =
        seq{
            "System.Runtime.CompilerServices"
            "BinaryObjectMapper"
        }
        |>Seq.map (SyntaxFactory.ParseName>>SyntaxFactory.UsingDirective)
        |>Seq.toArray

    let inline toMember x : MemberDeclarationSyntax = x

let rec processClass (``class``: ClassDeclarationSyntax) =
    let hasAttr (``class``: ClassDeclarationSyntax) =
        ``class``.AttributeLists
        |>Seq.exists (fun x -> x.Attributes |> Seq.exists (fun attr -> (string attr.Name) = "MappableTypeAttribute" || (string attr.Name) = "MappableType"))

    if hasAttr ``class`` then
        let subtypes =
            ``class``.Members
            |>Seq.choose tryProcessMember
            |>Seq.toArray
        SyntaxFactory
            .ClassDeclaration(``class``.Identifier)
            .WithModifiers(``class``.Modifiers)
            .AddAttributeLists(Common.compilerGenerated)
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

let processNode (syntaxNode: SyntaxNode) =
    match syntaxNode with
    | :? CompilationUnitSyntax as compUnit ->
        let newMember =
            compUnit.Members
            |>Seq.choose tryProcessMember
            |>Seq.toArray
        if Seq.isEmpty newMember  then
            None
        else
            SyntaxFactory
                .CompilationUnit()
                .AddMembers(newMember)
                .AddUsings(Common.usings)
            :>SyntaxNode
            |>Some
    |root ->
        root 
        |>tryProcessMember
        |>Option.map (fun x -> x:>SyntaxNode)

    |>Option.map (fun x -> x.NormalizeWhitespace())

let processProject (proj:Project) =
    let processFile (oldProj:Project) (id:DocumentId) =
        let document = oldProj.GetDocument id
        let node =
            document.GetSyntaxRootAsync()
            |>Async.AwaitTask
            |>Async.RunSynchronously
        match processNode node with
        |None -> oldProj
        |Some newNode ->
            let newName =
                (Path.GetFileNameWithoutExtension document.Name) + ".g.cs"
            match
                oldProj.Documents
                |>Seq.filter (fun doc -> doc.Name = newName)
                |>Seq.tryHead
                with
                | None ->
                    oldProj.AddDocument(newName, newNode).Project
                | Some doc ->
                    doc.WithSyntaxRoot(newNode).Project
    let mutable newP = proj
    for i in proj.DocumentIds do
        newP <- processFile newP i
    newP