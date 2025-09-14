using SaberTools.Common;
using System.IO;

// This file is part of Model Converter for Space Marine 2
// Copyright (C) 2025 Neo_Kesha
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// See <https://www.gnu.org/licenses/> for more details.

namespace SaberTools.USF
{
    public class UsfAnimation
    {
        public void Read(BinaryReader reader)
        {
            int version = reader.ReadInt32();

            initialTranslate.Read(reader);
            byte splTranslateFlag = reader.ReadByte();
            if (splTranslateFlag != 0)
            {
                splTranslate.Read(reader);
            }
            initialRotation.Read(reader);
            byte splRotationFlag = reader.ReadByte();
            if (splRotationFlag != 0)
            {
                splRotation.Read(reader);
            }
            initialScale.Read(reader);
            byte splScaleFlag = reader.ReadByte();
            if (splScaleFlag != 0)
            {
                splScale.Read(reader);
            }

            if (version >= 0x101)
            {
                initialVisibility = reader.ReadSingle();
            }
            else
            {
                initialVisibility = 1.0f;
            }
            byte splVisibilityFlag = reader.ReadByte();
            if (splVisibilityFlag != 0)
            {
                splVisibility.Read(reader);
            }
            return;
        }
        public void Write(BinaryWriter writer)
        {
            splVisibility = null;

            var version = 257;
            writer.Write(version);

            initialTranslate.Write(writer);
            if (splTranslate != null)
            {
                writer.Write((byte)1);
                splTranslate.Write(writer);
            }
            else
            {
                writer.Write((byte)0);
            }

            initialRotation.Write(writer);
            if (splRotation != null)
            {
                writer.Write((byte)1);
                splRotation.Write(writer);
            }
            else
            {
                writer.Write((byte)0);
            }

            initialScale.Write(writer);
            if (splScale != null)
            {
                writer.Write((byte)1);
                splScale.Write(writer);
            }
            else
            {
                writer.Write((byte)0);
            }

            writer.Write(initialVisibility);
            if (splVisibility != null)
            {
                writer.Write((byte)1);
                splVisibility.Write(writer);
            }
            else
            {
                writer.Write((byte)0);
            }
        }
        public UsfVector3 initialTranslate { get; set; } = new UsfVector3();
        public UsfQuaternion initialRotation { get; set; } = new UsfQuaternion();
        public UsfVector3 initialScale { get; set; } = new UsfVector3();
        public float initialVisibility { get; set; }
        public UsfSpl splTranslate { get; set; } = new UsfSpl();
        public UsfSpl splRotation { get; set; } = new UsfSpl();
        public UsfSpl splScale { get; set; } = new UsfSpl();
        public UsfSpl splVisibility { get; set; } = new UsfSpl();
    }
}
