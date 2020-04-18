using System;

namespace BinaryObjectMapper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class MappableTypeAttribute:
        Attribute
    {
    }
}
