using System;
using CodeGeneration.Roslyn;

namespace BinaryObjectMapper
{
    [AttributeUsage(AttributeTargets.Class)]
    [CodeGenerationAttribute("BinaryObjectMapper.BinaryMapperGenerator")]
    public class MappableTypeAttribute:
        Attribute
    {
    }
}
