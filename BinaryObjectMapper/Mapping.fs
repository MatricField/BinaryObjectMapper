namespace BinaryObjectMapper

open System
open System.Collections.Immutable
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open Microsoft.CodeAnalysis.Diagnostics


module private Common =

    let CanDoCodeGenDescriptor =
        DiagnosticDescriptor(
            id = "BOM001",
            title = "Generate Mapper Implementation",
            messageFormat = "Generate implementation of binary mapper for class {0}",
            category = "Refactor", 
            defaultSeverity = DiagnosticSeverity.Info,
            isEnabledByDefault = true,
            customTags = [||])
    let Rules =
        [|
            CanDoCodeGenDescriptor
        |]
        |>ImmutableArray.Create<DiagnosticDescriptor>

    let MappableTypeAttribute =
        "BinaryObjectMapper.MappableTypeAttribute"


[<DiagnosticAnalyzer(LanguageNames.CSharp)>]
type SerializationCodeGenAnalyzer() =
    inherit DiagnosticAnalyzer()

    let analyzeSyntaxNode (ctx:SyntaxNodeAnalysisContext) =
        let Compilation = ctx.Compilation
        let mappable =
            Compilation.GetTypeByMetadataName(Common.MappableTypeAttribute)
    
        let scanForMappableAttr (node:AttributeSyntax) =
            ctx.CancellationToken.ThrowIfCancellationRequested()
            let attr = ctx.SemanticModel.GetTypeInfo(node, ctx.CancellationToken).Type
            attr.Equals(mappable)
    
        match ctx.Node with
        | :? ClassDeclarationSyntax as cds->
            if cds.AttributeLists |> Seq.collect (fun ls -> ls.Attributes) |> Seq.exists scanForMappableAttr then
                Diagnostic.Create(Common.CanDoCodeGenDescriptor, cds.GetLocation(), cds.Identifier.ToString())
                |>ctx.ReportDiagnostic
        | _ -> ArgumentException() |> raise
        ()

    override __.SupportedDiagnostics 
        with get() = Common.Rules
    override __.Initialize(context) =
        try
            context.RegisterSyntaxNodeAction(analyzeSyntaxNode, SyntaxKind.ClassDeclaration)
        with
        |ex -> failwith ex.StackTrace


