using System.IO;
using System.Numerics;

// This file is part of Model Converter for Space Marine 2
// Copyright (C) 2025 Neo_Kesha
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// See <https://www.gnu.org/licenses/> for more details.

namespace SaberTools.Common
{
    public class UsfVector2
    {
        public UsfVector2() { }
        public UsfVector2(Vector2 vector)
        {
            this.x = vector.X;
            this.y = vector.Y;
        }
        public float x { get; set; }
        public float y { get; set; }
        public void Read(BinaryReader reader)
        {
            x = reader.ReadSingle();
            y = reader.ReadSingle();
        }
        public void Write(BinaryWriter writer)
        {
            writer.Write(x);
            writer.Write(y);
        }

        public static implicit operator string(UsfVector2 vec) => vec.ToString();
        public override string ToString()
        {
            return $"({x},{y})";
        }
    }
    public class UsfVector3
    {
        public UsfVector3() { }
        public UsfVector3(Vector3 vector)
        {
            this.x = vector.X;
            this.y = vector.Y;
            this.z = vector.Z;
        }

        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public void Read(BinaryReader reader)
        {
            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
        }
        public void Write(BinaryWriter writer)
        {
            writer.Write(x);
            writer.Write(y);
            writer.Write(z);
        }
        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public static implicit operator string(UsfVector3 vec) => vec.ToString();
        public override string ToString()
        {
            return $"({x},{y},{z})";
        }
    }
    public class UsfVector4
    {
        public UsfVector4() { }
        public UsfVector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public UsfVector4(Vector4 vector)
        {
            this.x = vector.X;
            this.y = vector.Y;
            this.z = vector.Z;
            this.w = vector.W;
        }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }
        public void Read(BinaryReader reader)
        {
            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            w = reader.ReadSingle();
        }
        public void Write(BinaryWriter writer)
        {
            writer.Write(x);
            writer.Write(y);
            writer.Write(z);
            writer.Write(w);
        }
        public bool IsZero()
        {
            return x == 0 && y == 0 && z == 0 && w == 0;
        }

        public static implicit operator string(UsfVector4 vec) => vec.ToString();
        public override string ToString()
        {
            return $"({x},{y},{z},{w})";
        }
    }
    public class UsfQuaternion : UsfVector4
    {
        public UsfQuaternion()
        {

        }
        public UsfQuaternion(Quaternion quat)
        {
            x = quat.X;
            y = quat.Y;
            z = quat.Z;
            w = quat.W;
        }
        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }
    }

    public class UsfMatrix4
    {
        public static UsfMatrix4 Identity()
        {
            var m = new UsfMatrix4();
            m.rows[0] = new UsfVector4(1, 0, 0, 0);
            m.rows[1] = new UsfVector4(0, 1, 0, 0);
            m.rows[2] = new UsfVector4(0, 0, 1, 0);
            m.rows[3] = new UsfVector4(0, 0, 0, 1);

            return m;
        }
        public UsfVector4[] rows { get; set; } = new UsfVector4[4] { new UsfVector4(), new UsfVector4(), new UsfVector4(), new UsfVector4() };
        public void Read(BinaryReader reader)
        {
            rows[0].Read(reader);
            rows[1].Read(reader);
            rows[2].Read(reader);
            rows[3].Read(reader);
        }
        public void Write(BinaryWriter writer)
        {
            rows[0].Write(writer);
            rows[1].Write(writer);
            rows[2].Write(writer);
            rows[3].Write(writer);
        }
    }
}
