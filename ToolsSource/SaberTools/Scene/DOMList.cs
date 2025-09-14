using SaberTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace SaberTools.Scene
{
    public class DOMList : SaberResource
    {
        public List<string> instanceKeys = new List<string>();
        public Dictionary<string, Domain> instances = new Dictionary<string, Domain>();

        public DOMList() : base("dom_list")
        {

        }

        public DOMList(BinaryReader reader) : base(reader)
        {
            var cnt1 = reader.ReadUInt32();
            var cnt2 = reader.ReadUInt32(); //always 09
            var hasNames = reader.ReadByte() == 1;
            if (hasNames)
            {
                var names = new List<string>();
                for (int i = 0; i < cnt1; i++)
                {
                    var strLen = reader.ReadInt32();
                    var name = new string(reader.ReadChars(strLen));
                    var instance = new Domain();
                    instance.name = name;
                    instances.Add(name, instance);
                    instanceKeys.Add(name);
                }
            }
            var hasTransform = reader.ReadByte() == 1;
            if (hasTransform)
            {
                for (int i = 0; i < cnt1; i++)
                {
                    var instance = instances[instanceKeys[i]];
                    var cnt = reader.ReadInt32();
                    instance.polyList = new List<Triangle2D>();
                    for (var j = 0; j < cnt; ++j)
                    {
                        var poly = new Triangle2D();
                        poly.Read(reader);
                        instance.polyList.Add(poly);
                    }
                }
            }
            var hasYLevel = reader.ReadByte() == 1;
            if (hasYLevel)
            {
                for (int i = 0; i < cnt1; i++)
                {
                    var instance = instances[instanceKeys[i]];
                    instance.yLevel = reader.ReadSingle();
                }
            }
            var hasHeight = reader.ReadByte() == 1;
            if (hasHeight)
            {
                for (int i = 0; i < cnt1; i++)
                {
                    var instance = instances[instanceKeys[i]];
                    instance.height = reader.ReadSingle();
                }
            }
            var hasBBox = reader.ReadByte() == 1;
            if (hasBBox)
            {
                for (int i = 0; i < cnt1; i++)
                {
                    var instance = instances[instanceKeys[i]];
                    instance.bbox = new BoundingBox();
                    instance.bbox.Read(reader);
                }
            }
            var hasIsQuatZero = reader.ReadByte() == 1;
            if (hasIsQuatZero)
            {
                for (int i = 0; i < cnt1; i++)
                {
                    var instance = instances[instanceKeys[i]];
                    instance.isQuatZero = reader.ReadByte() == 1;
                }
            }
            var hasQuat = reader.ReadByte() == 1;
            if (hasQuat)
            {
                throw new NotImplementedException();
            }
            var hasPivotNames = reader.ReadByte() == 1;
            if (hasPivotNames)
            {
                throw new NotImplementedException();
            }
            var hasPivots = reader.ReadByte() == 1;
            if (hasPivots)
            {
                for (int i = 0; i < cnt1; i++)
                {
                    var instance = instances[instanceKeys[i]];
                    instance.pivot = new Vector3();
                    instance.pivot.X = reader.ReadSingle();
                    instance.pivot.Y = reader.ReadSingle();
                    instance.pivot.Z = reader.ReadSingle();
                }
            }
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write((UInt32)instanceKeys.Count);
            writer.Write((UInt32)0x9);

            writer.Write((Byte)1); //has names;
            foreach (var key in instanceKeys)
            {
                var instance = instances[key];
                writer.Write((Int32)instance.name.Length);
                writer.Write(instance.name.ToCharArray());
            }

            writer.Write((Byte)1); //has transforms
            for (int i = 0; i < instanceKeys.Count; i++)
            {
                var instance = instances[instanceKeys[i]];
                writer.Write((Int32)instance.polyList.Count);
                foreach(var poly in instance.polyList)
                {
                    poly.Write(writer);
                }
            }
            writer.Write((Byte)1); //has ylevel
            for (int i = 0; i < instanceKeys.Count; i++)
            {
                var instance = instances[instanceKeys[i]];
                writer.Write(instance.yLevel);
            }

            writer.Write((Byte)1); //has height
            for (int i = 0; i < instanceKeys.Count; i++)
            {
                var instance = instances[instanceKeys[i]];
                writer.Write(instance.height);
            }

            writer.Write((Byte)1); //has bbox
            for (int i = 0; i < instanceKeys.Count; i++)
            {
                var instance = instances[instanceKeys[i]];
                instance.bbox.Write(writer);
            }

            writer.Write((Byte)1); //has isQuatZero
            for (int i = 0; i < instanceKeys.Count; i++)
            {
                var instance = instances[instanceKeys[i]];
                writer.Write(instance.isQuatZero);
            }
            writer.Write((Byte)0); //has no quat
            writer.Write((Byte)0); //has no pivot names
            writer.Write((Byte)1); //has pivots
            for (int i = 0; i < instanceKeys.Count; i++)
            {
                var instance = instances[instanceKeys[i]];
                writer.Write(instance.pivot.X);
                writer.Write(instance.pivot.Y);
                writer.Write(instance.pivot.Z);
            }
        }

        public void Add(Domain dom)
        {
            var name = dom.name;
            instanceKeys.Add(name);
            instances.Add(name, dom);
        }
    }

    public class Domain
    {
        public string name;
        public List<Triangle2D> polyList;

        public float yLevel;
        public float height;
        public BoundingBox bbox;
        public bool isQuatZero;
        public Quaternion quaternion;
        public string pivotName;
        public Vector3 pivot;
    }

    public class Triangle2D
    {
        public Vector2 v1 = new Vector2();
        public Vector2 v2 = new Vector2();
        public Vector2 v3 = new Vector2();
        public void Read(BinaryReader reader)
        {
            v1.X = reader.ReadSingle(); v1.Y = reader.ReadSingle();
            v2.X = reader.ReadSingle(); v2.Y = reader.ReadSingle();
            v3.X = reader.ReadSingle(); v3.Y = reader.ReadSingle();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(v1.X); writer.Write(v1.Y);
            writer.Write(v3.X); writer.Write(v2.Y);
            writer.Write(v3.X); writer.Write(v3.Y);
        }
    }
    public class BoundingBox
    {
        public Vector3 a = new Vector3();
        public Vector3 b = new Vector3();
        public void Read(BinaryReader reader)
        {
            a.X = reader.ReadSingle(); a.Y = reader.ReadSingle(); a.Z = reader.ReadSingle();
            b.X = reader.ReadSingle(); b.Y = reader.ReadSingle(); b.Z = reader.ReadSingle();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(a.X); writer.Write(a.Y); writer.Write(a.Z);
            writer.Write(b.X); writer.Write(b.Y); writer.Write(b.Z);
        }
    }
}
