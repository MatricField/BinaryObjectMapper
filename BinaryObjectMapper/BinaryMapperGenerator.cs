using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BinaryObjectMapper
{
    public class BinaryMapperGenerator :
        ICodeGenerator
    {
        public BinaryMapperGenerator(AttributeData attributeData)
        {
            
        }

        public async Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            var Compilation = context.Compilation;
            var Model = context.SemanticModel;

        }

        private readonly (SpecialType, string)[] MappableSystemTypes = new[]
            {
                (SpecialType.System_Byte, nameof(BinaryReader.ReadByte)),
                (SpecialType.System_SByte, nameof(BinaryReader.ReadByte)),
            };
    }

    class GenerateWorker
    {
        private Compilation Compilation;

        private SemanticModel Model;

        private TypeDeclarationSyntax GeneratingType;

        public TypeDeclarationSyntax Generated { get; private set; }

        public GenerateWorker(TransformationContext context, CancellationToken cancellationToken)
        {
            Compilation = context.Compilation;
            Model = context.SemanticModel;
            if(context.ProcessingNode is ClassDeclarationSyntax @class)
            {
                GeneratingType = @class;
            }
            else if(context.ProcessingNode is StructDeclarationSyntax @struct)
            {
                GeneratingType = @struct;
            }
            else
            {
                throw new ArgumentException();
            }
            Generated = Generate(cancellationToken);
        }

        private TypeDeclarationSyntax Generate(CancellationToken cancellationToken)
        {
            var mappableInterface = Compilation.GetTypeByMetadataName(typeof(BinaryObjectMapper.IMappableType).FullName);
            var bases = GeneratingType.BaseList switch
            {
            null =>
            SyntaxFactory.BaseList(
                SyntaxFactory.Token(SyntaxKind.ColonToken),
                SyntaxFactory.SeparatedList<BaseTypeSyntax>(
                    new[] { SyntaxFactory.SimpleBaseType(
                            SyntaxFactory.ParseTypeName(typeof(BinaryObjectMapper.IMappableType).FullName))})),
            BaseListSyntax list =>
                list switch
                {
                    _ when list.Types.Any(
                        baseType =>
                            Model.GetSymbolInfo(baseType).Equals(mappableInterface))
                        => list,
                    _ => list.AddTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(mappableInterface.Name)))
                }
            };

        }

        private MethodDeclarationSyntax GenerateRead()
        {

        }

        private MethodDeclarationSyntax GenerateWrite()
        {

        }

        public static implicit operator TypeDeclarationSyntax(GenerateWorker worker)
        {
            return worker.Generated;
        }
    }
}
