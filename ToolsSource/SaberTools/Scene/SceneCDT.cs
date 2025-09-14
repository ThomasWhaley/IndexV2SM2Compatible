using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaberTools.Scene
{
    public class SceneCDT : SaberResource
    {
        public SceneCDT() : base("cdt")
        {

        }

        public SceneCDT(BinaryReader reader) : base(reader)
        {
            var version = reader.ReadByte();
            var moppCount = reader.ReadInt32();

            for (int i = 0; i < moppCount; ++i)
            {
                var objCnt = reader.ReadInt32();
                if (objCnt < 0) continue;
            }
        }
    }

    public class MOPPHeader
    {
        public Int16 a;
    }
}
