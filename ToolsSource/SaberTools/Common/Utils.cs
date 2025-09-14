// This file is part of Model Converter for Space Marine 2
// Copyright (C) 2025 Neo_Kesha
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// See <https://www.gnu.org/licenses/> for more details.

using System;
using System.IO;

namespace SaberTools.Common
{
    public static class UsfString
    {
        public static string Read(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            if (length == 0)
            {
                return "";
            }

            return new string(reader.ReadChars(length));
        }

        public static void Write(BinaryWriter writer, string str)
        {
            writer.Write((Int32)str.Length);
            if (str.Length == 0)
            {
                return;
            }

            writer.Write(str.ToCharArray());
        }
    }
}
