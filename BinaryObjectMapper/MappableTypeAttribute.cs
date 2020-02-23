using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryObjectMapper
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class MappableTypeAttribute:
        Attribute
    {

    }
}
