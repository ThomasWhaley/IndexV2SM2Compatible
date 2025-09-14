using SaberTools.Common;
using System;
using System.Collections.Generic;
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
    public class UsfLwiInfo
    {
        public void Read(BinaryReader reader)
        {
            Int32 version = reader.ReadInt32();
            if (version >= 0x102 && version < 0x105)
            {
                refName = UsfString.Read(reader);
            }
            tplName = UsfString.Read(reader);
            if (version >= 0x103)
            {
                Int32 materialCount = reader.ReadInt32();
                materialChanges = new TplObjectMaterialChange[materialCount];
                for (int i = 0; i < materialCount; i++)
                {
                    var materialChange = materialChanges[i];
                    materialChange.fromMaterialName = UsfString.Read(reader);
                    materialChange.toMaterialName = UsfString.Read(reader);
                    materialChange.textureName = UsfString.Read(reader);
                    materialChange.objName = UsfString.Read(reader);
                    materialChange.applyHierarchically = (reader.ReadByte() != 0);
                }
            }
            if (version >= 0x104)
            {
                Int32 overrideCount = reader.ReadInt32();
                tplOverrides = new Dictionary<string, TplObjectOverride>(overrideCount);
                for (int i = 0; i < overrideCount; i++)
                {
                    string key = UsfString.Read(reader);
                    var tplOverride = new TplObjectOverride();
                    tplOverrides[key] = tplOverride;
                    tplOverride.isHidden = reader.ReadByte() != 0;
                    byte hasTransform = reader.ReadByte();
                    if (hasTransform != 0)
                    {
                        tplOverride.transform = new UsfMatrix4();
                        tplOverride.transform.Read(reader);
                    }
                }
            }

            if (version >= 0x106)
            {
                disableCollisionMask = reader.ReadUInt64();
            }

            if (version >= 0x108)
            {
                createCollisionActor = reader.ReadBoolean();
            }

            if (version >= 0x109)
            {
                castShadows = reader.ReadBoolean();
            }

            if (version >= 0x107)
            {
                customizationIndex = reader.ReadUInt32();
            }
        }

        public void Write(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public string refName { get; set; }
        public string tplName { get; set; }
        public TplObjectMaterialChange[] materialChanges { get; set; }
        public Dictionary<string, TplObjectOverride> tplOverrides { get; set; }
        public UInt64 disableCollisionMask { get; set; }
        public bool createCollisionActor { get; set; }
        public bool castShadows { get; set; }
        public UInt32 customizationIndex { get; set; }
    }
    public class TplObjectMaterialChange
    {
        public string fromMaterialName { get; set; }
        public string toMaterialName { get; set; }
        public string textureName { get; set; }
        public string objName { get; set; }
        public bool applyHierarchically { get; set; }
    };
    public class TplObjectOverride
    {
        public bool isHidden { get; set; }
        public UsfMatrix4 transform { get; set; }
    };
}
