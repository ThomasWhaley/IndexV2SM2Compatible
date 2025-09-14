// This file is part of Model Converter for Space Marine 2
// Copyright (C) 2025 Neo_Kesha
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// See <https://www.gnu.org/licenses/> for more details.

using System.IO;

namespace SaberTools.USF
{
    public class UsfCameraInfo
    {
        public void Read(BinaryReader reader)
        {
            int version = reader.ReadInt32();
            fov = reader.ReadSingle();
            aspectHW = reader.ReadSingle();
            byte splFovFlag = reader.ReadByte();
            if (splFovFlag != 0)
            {
                splFov = new UsfSpl();
                splFov.Read(reader);
            }
            byte splRollFlag = reader.ReadByte();
            if (splRollFlag != 0)
            {
                splRoll = new UsfSpl();
                splRoll.Read(reader);
            }
            return;
        }
        public void Write(BinaryWriter writer)
        {
            int version = 256;
            writer.Write(version);
            writer.Write(fov);
            writer.Write(aspectHW);
            if (splFov != null)
            {
                writer.Write((byte)1);
                splFov.Write(writer);
            }
            else
            {
                writer.Write((byte)0);
            }
            if (splRoll != null)
            {
                writer.Write((byte)1);
                splRoll.Write(writer);
            }
            else
            {
                writer.Write((byte)0);
            }
        }

        public float fov { get; set; }
        public float aspectHW { get; set; }
        UsfSpl splFov;
        UsfSpl splRoll;
    }
}
