using System.IO;

namespace BinaryObjectMapper
{
    public interface IMappableType
    {
        void WriteTo(BinaryWriter writer);
        void ReadFrom(BinaryReader reader);
    }
}
