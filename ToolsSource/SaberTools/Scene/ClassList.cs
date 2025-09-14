using SaberTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaberTools.Scene
{
    public class ClassList : SaberResource
    {
        public List<string> instanceKeys = new List<string>();
        public Dictionary<string, ClassData> instances = new Dictionary<string, ClassData>();

        public ClassList() : base("class_list")
        {

        }

        public ClassList(BinaryReader reader) : base(reader)
        {
            var cnt1 = reader.ReadUInt32();
            var cnt2 = reader.ReadUInt32(); //always 0x02
            var hasNames = reader.ReadByte() == 1;
            if (hasNames)
            {
                for (int i = 0; i < cnt1; i++)
                {
                    var strLen = reader.ReadInt32();
                    var name = new string(reader.ReadChars(strLen));
                    var instance = new ClassData();
                    instance.name = name;
                    instances.Add(name, instance);
                    instanceKeys.Add(name);
                }
            }

            var hasPs = reader.ReadByte() == 1;
            if (hasPs)
            {
                for (int i = 0; i < cnt1; i++)
                {
                    var instance = instances[instanceKeys[i]];
                    var len = reader.ReadInt32();
                    if (len == 0) continue;
                    instance.ps = new string(reader.ReadChars(len));
                }
            }
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write((UInt32)instanceKeys.Count);
            writer.Write((UInt32)0x2);

            writer.Write((Byte)1); //has names;
            foreach (var key in instanceKeys)
            {
                var instance = instances[key];
                writer.Write((Int32)instance.name.Length);
                writer.Write(instance.name.ToCharArray());
            }

            writer.Write((Byte)1); //has ps;
            foreach (var key in instanceKeys)
            {
                var instance = instances[key];
                writer.Write((Int32)instance.ps.Length);
                if (instance.ps.Length == 0) continue;

                writer.Write(instance.ps.ToCharArray());
            }
        }

        public void Add(ClassData cls)
        {
            var name = cls.name;
            instanceKeys.Add(name);
            instances.Add(name, cls);
        }
    }

    public class ClassData
    {
        public string name;
        public string ps;
    }
}
