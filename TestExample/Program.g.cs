using System.IO;
using System.Runtime.CompilerServices;
using BinaryObjectMapper;

namespace TestExample
{
    [CompilerGenerated]
    partial class Test : IMappableType
    {
        public void Deserialize(BinaryReader reader)
        {
            field1 = reader.ReadInt32();
            field2 = reader.ReadDouble();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(field1);
            writer.Write(field2);
        }
    }
}