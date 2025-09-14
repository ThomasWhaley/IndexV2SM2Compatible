using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaberTools
{
    public class SaberResource
    {
        public string resType;
        public string resTag;
        public SaberResource(BinaryReader reader)
        {
            reader.ReadUInt32(); //1SER
            resType = new string(reader.ReadChars(0x20)).Replace("\0","");
            resTag = new string(reader.ReadChars(0x10)).Replace("\0", "");
            reader.ReadBytes(12); //padding
        }

        public SaberResource(string type)
        {
            resType = type;
            resTag = "S3DRESOURCE";
        }

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write("1SER".ToCharArray());
            writer.Write(resType.ToCharArray());
            for (var i = 0; i < 0x20 - resType.Length; ++i)
            {
                writer.Write((Byte)0);
            }
            writer.Write(resTag.ToCharArray());
            for (var i = 0; i < 0x10 - resTag.Length; ++i)
            {
                writer.Write((Byte)0x20);
            }
            writer.Write((UInt32)0);
            writer.Write((UInt32)0);
            writer.Write((UInt32)0);
        }
    }
}
