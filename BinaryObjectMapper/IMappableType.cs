using System;
using System.IO;

namespace BinaryObjectMapper
{
    public interface IMappableType
    {
        void Serialize(BinaryWriter writer);

        void Deserialize(BinaryReader reader);
    }
}
