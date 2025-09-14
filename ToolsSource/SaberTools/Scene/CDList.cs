using SaberTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SaberTools.Scene
{
    public class CDList : SaberResource
    {
        public List<string> instanceKeys = new List<string>();
        public Dictionary<string, CreationInstance> instances = new Dictionary<string, CreationInstance>();

        public CDList() : base("cd_list")
        {

        }

        public CDList(BinaryReader reader) : base(reader) 
        {
            var cnt1 = reader.ReadUInt32();
            var cnt2 = reader.ReadUInt32(); //always 0xA
            var hasNames = reader.ReadByte() == 1;
            if (hasNames)
            {
                for (int i = 0; i < cnt1; i++)
                {
                    var strLen = reader.ReadInt32();
                    var name = new string(reader.ReadChars(strLen));
                    var instance = new CreationInstance();
                    instance.name = name;
                    instance.idx = i;
                    instances.Add(name, instance);
                    instanceKeys.Add(name);
                }
            }

            reader.ReadByte(); //skip string

            var hasTransform = reader.ReadByte() == 1;
            if (hasTransform)
            {
                for (int i = 0; i < cnt1; i++)
                {
                    var instance = instances[instanceKeys[i]];
                    instance.matrix = new Matrix4x4();
                    for (int row = 0; row < 4; ++row)
                    {
                        for (int col = 0; col < 4; ++col)
                        {
                            instance.matrix[row, col] = reader.ReadSingle();
                        }
                    }
                }
            }

            var hasAffix = reader.ReadByte() == 1;
            if (hasAffix)
            {
                for (int i = 0; i < cnt1; i++)
                {
                    var instance = instances[instanceKeys[i]];
                    var len = reader.ReadInt32();
                    if (len == 0) continue;
                    instance.affixes = new string(reader.ReadChars(len));
                }
            }


            var hasChachedPS = reader.ReadByte() == 2;
            if (hasChachedPS)
            {
                //my head is fried sorry
                var bytes = cnt1 / 8;
                if (bytes * 8 < cnt1) bytes += 1;
                byte[] bytemask = reader.ReadBytes((int)bytes);

                for (int i = 0; i < cnt1; i++)
                {
                    int byte_index = i / 8;
                    int bit_index = i % 8;
                    int byte_val = bytemask[byte_index];
                    int bit_val = (byte_val >> bit_index) & 0x01;
                    var instance = instances[instanceKeys[i]];
                    instance.hasCachedPS = bit_val != 0;
                }

                for (int i = 0; i < cnt1; i++)
                {
                    var instance = instances[instanceKeys[i]];
                    var chkid1 = reader.ReadInt16(); //ps
                    var offsetSkip1 = reader.ReadInt32();
                    var strlen = reader.ReadInt32();
                    instance.ps = new string(reader.ReadChars(strlen));
                    var chkid2 = reader.ReadInt16(); //end
                    var offsetSkip2 = reader.ReadInt32();
                }
            }

            reader.ReadInt16(); //skip uint16

            var hasParentInstanceIdx = reader.ReadByte() == 1;
            if (hasParentInstanceIdx)
            {
                for (int i = 0; i < cnt1; i++)
                {
                    var instance = instances[instanceKeys[i]];
                    instance.parentInstanceIdx = reader.ReadUInt32();
                    if (instance.parentInstanceIdx != 0xFFFFFFFF) instance.parentInstanceName = instanceKeys[(int)instance.parentInstanceIdx];
                }
            }

            var hasParentObject = reader.ReadByte() == 1;
            if (hasParentObject)
            {
                for (int i = 0; i < cnt1; i++)
                {
                    var instance = instances[instanceKeys[i]];
                    var len = reader.ReadInt32();
                    if (len == 0) continue;
                    instance.parentObject = new string(reader.ReadChars(len));
                }
            }

            var hasGameObjectFlags = reader.ReadByte() == 1;
            if (hasGameObjectFlags)
            {
                for (int i = 0; i < cnt1; i++)
                {
                    var instance = instances[instanceKeys[i]];
                    instance.gameObjectFlags = reader.ReadByte();
                }
            }
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write((UInt32)instanceKeys.Count);
            writer.Write((UInt32)0xA);
            writer.Write((Byte)1); //has names;
            foreach (var key in instanceKeys)
            {
                var instance = instances[key];
                writer.Write((Int32)instance.name.Length);
                writer.Write(instance.name.ToCharArray());
            }

            writer.Write((Byte)0); //skip string

            writer.Write((Byte)1); //has transform;
            foreach (var key in instanceKeys)
            {
                var instance = instances[key];
                for (int row = 0; row < 4; ++row)
                {
                    for (int col = 0; col < 4; ++col)
                    {
                        writer.Write(instance.matrix[row, col]);
                    }
                }
            }

            writer.Write((Byte)1); //has affix;
            foreach (var key in instanceKeys)
            {
                var instance = instances[key];
                if (instance.affixes == null || instance.affixes.Length == 0)
                {
                    writer.Write((Int32)0);
                }
                else
                {
                    writer.Write((Int32)instance.affixes.Length);
                    writer.Write(instance.affixes.ToCharArray());
                }
            }

            writer.Write((Byte)2); //has cached ps;

            var bytes = instanceKeys.Count / 8;
            if (bytes * 8 < instanceKeys.Count) bytes += 1;
            for (var i = 0; i < bytes; ++i)
            {
                writer.Write((Byte)0xFF); //hack for the sake of my sanity
            }

            foreach (var key in instanceKeys)
            {
                var instance = instances[key];
                writer.Write((Int16)0); //PS Chunk ID;
                writer.Write((Int32)(writer.BaseStream.Position + 8 + instance.ps.Length));
                writer.Write((Int32)instance.ps.Length);
                writer.Write(instance.ps.ToCharArray());
                writer.Write((Int16)(-1)); //End Chunk ID;
                writer.Write((Int32)(writer.BaseStream.Position + 4));
            }

            writer.Write((UInt16)0); //write skip

            writer.Write((Byte)1); //has parent instance idx;
            foreach (var key in instanceKeys)
            {
                var instance = instances[key];
                writer.Write(instance.parentInstanceIdx);
            }

            writer.Write((Byte)1); //has parent obj;
            foreach (var key in instanceKeys)
            {
                var instance = instances[key];
                if (instance.parentObject == null || instance.parentObject.Length == 0)
                {
                    writer.Write((Int32)0);
                }
                else
                {
                    writer.Write((Int32)instance.parentObject.Length);
                    writer.Write(instance.parentObject.ToCharArray());
                }
            }

            writer.Write((Byte)1); //has game object flags;
            foreach (var key in instanceKeys)
            {
                var instance = instances[key];
                writer.Write(instance.gameObjectFlags);
            }
        }
    
        public void Sort()
        {
            instanceKeys.Sort((a, b) => (instances[a].idx - instances[b].idx));
        }

        public void Add(CreationInstance inst)
        {
            var name = inst.name;
            instanceKeys.Add(name);
            instances.Add(name, inst);
        }
    }

    public class CreationInstance
    {
        public string name;
        public Matrix4x4 matrix;
        public string affixes;
        public bool hasCachedPS;

        public string ps;

        public UInt32 parentInstanceIdx = 0xFFFFFFFF;
        public string parentObject;
        public byte gameObjectFlags;

        public int idx = -1;
        public string parentInstanceName;
    }
}
