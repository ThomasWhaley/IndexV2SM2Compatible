using SaberTools.Common;
using System;
using System.IO;
using System.Text;

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

    public class UsfPses
    {
        public UsfPses()
        {
            ps = new Ps();
        }
        public UsfPses(bool semicolon)
        {
            if (semicolon)
            {
                ps = new Ps(true);
            }
        }
        public void Read(BinaryReader reader)
        {
            version = reader.ReadInt32();
            if (version <= 0x101)
            {
                throw new NotImplementedException();
            }

            ps = new Ps();
            ps.Read(reader);
        }
        public void Write(BinaryWriter writer)
        {
            int version = 258;
            writer.Write(version);
            ps.Write(writer);
        }
        public Int32 version { get; set; }
        public Ps ps { get; set; }

    }

    public class Ps
    {
        public Ps()
        {
            this.str = "";
        }

        public Ps(string str)
        {
            this.str = str;
        }
        public Ps(bool semicolon)
        {
            this.semicolon = semicolon;
            this.str = "";
        }
        public void Write(BinaryWriter writer)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter streamWriter = new StreamWriter(stream);
                ps.Write(streamWriter);
                streamWriter.Flush();
                if (semicolon)
                {
                    _str = Encoding.Default.GetString(stream.ToArray()).Replace("\r", "").Replace("\n", ";");
                }
                else
                {
                    _str = Encoding.Default.GetString(stream.ToArray()).Replace("\r", "");
                }
                streamWriter.Flush();
            }

            UsfString.Write(writer, str);
        }
        public void Read(BinaryReader reader)
        {
            str = UsfString.Read(reader);
        }

        public string str
        {
            get => _str; set
            {
                _str = value;
                ps = PropertySection.Parse(_str);
            }
        }
        private string _str;
        private bool semicolon = false;

        private PropertySection ps;
        public ValueBase this[string key]
        {
            get
            {
                return ps[key];
            }

            set
            {
                ps[key] = value;
            }
        }
    }

}
