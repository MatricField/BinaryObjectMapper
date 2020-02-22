using System;

namespace BinaryObjectMapper.lib
{
    /// <summary>
    /// Test attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct)]
    public class ObjectModelAttribute:
        Attribute
    {
    }
}
