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
    public enum UsfNodeCommonUID
    {
        Root = 0,
        DX = 1,
        Actor = 2,
        Character = 3,
        SkinnedGeom = 4,
        CharacterRoot = 5,
        Top
    }
    public class UsfNode
    {
        public static explicit operator string(UsfNode obj) { return obj.ToString(); }
        public override string ToString()
        {
            return name;
        }
        public static UsfNode BuildRootNode()
        {
            var node = new UsfNode();
            node.affixes = "\n\n\n";
            node.pses = new UsfPses(true);
            node.name = ".root.";
            node.mayaNodeId = "";

            node.local = UsfMatrix4.Identity();
            node.localOriginal = UsfMatrix4.Identity();
            node.world = UsfMatrix4.Identity();
            node.uid = (int)UsfNodeCommonUID.Root;
            return node;
        }

        public static UsfNode BuildCharacterROOTNode()
        {
            var node = new UsfNode();
            node.affixes = "export_preserve_position\n\n\n";
            node.pses = new UsfPses(true);
            node.name = "ROOT";
            node.mayaNodeId = "locator";
            node.sourceId = "|ROOT";

            node.local = UsfMatrix4.Identity();
            node.localOriginal = UsfMatrix4.Identity();
            node.world = UsfMatrix4.Identity();
            node.uid = (int)UsfNodeCommonUID.CharacterRoot;
            return node;
        }
        public static UsfNode BuildDxNode()
        {
            var node = new UsfNode();
            node.affixes = "\n\n\n";
            node.pses = new UsfPses(true);
            node.name = "s3dSaberDxTechnicalNode";
            node.mayaNodeId = "saberDxNode";
            node.sourceId = "|s3dSaberDxTechnicalNode";

            node.local = UsfMatrix4.Identity();
            node.localOriginal = UsfMatrix4.Identity();
            node.world = UsfMatrix4.Identity();
            node.uid = (int)UsfNodeCommonUID.DX;
            return node;
        }
        public static UsfNode BuildActorNode(string actorRootName)
        {
            var node = new UsfNode();
            node.affixes = "\n\n\n";
            node.pses = new UsfPses(true);
            node.pses.ps = new Ps();
            node.pses.ps["tpl_name"] = new ValueString("weapons\\" + actorRootName + ".tpl");
            node.pses.ps["tpl_obj"] = new ValueString("");
            node.pses.ps["__type"] = new ValueString("iactor");
            node.pses.ps["ExportOptions"] = new ValueProperty();
            node.pses.ps["ExportOptions"]["generateCollisionData"] = new ValueBool(true);
            node.pses.ps["ExportOptions"]["buildSkinCompound"] = new ValueBool(true);
            node.name = actorRootName;
            node.mayaNodeId = "saberActor";
            node.actor = new UsfSaberActor();
            node.actor.tplName = "weapons\\" + actorRootName + ".tpl";

            node.sourceId = "|" + actorRootName;
            node.local = UsfMatrix4.Identity();
            node.localOriginal = UsfMatrix4.Identity();
            node.world = UsfMatrix4.Identity();
            node.uid = (int)UsfNodeCommonUID.Actor;
            return node;
        }
        public static UsfNode BuildCharNode(string actorRootName)
        {
            var node = new UsfNode();
            node.affixes = "\n\n\n";
            node.pses = new UsfPses(true);
            node.pses.ps.str = "tpl_name = \u0022characters\\\\cc_hormagaunt.tpl\u0022;tpl_obj = \u0022|ROOT;|skinned_geometry\u0022;isAutoDestroy = false;ExportOptions = {exportHavokData = false;havokExportPreset = \u0022rb_shrink\u0022;generateCollisionData = true;buildSkinCompound = true;};__type = \u0022iactor\u0022;";
            node.pses.ps["tpl_name"] = new ValueString("characters\\\\" + actorRootName + ".tpl");

            node.name = "characterBuilderExportTplDesc";
            node.mayaNodeId = "saberActor";
            node.actor = new UsfSaberActor();
            node.actor.tplName = "characters\\" + actorRootName + ".tpl";
            node.sourceId = "|characterBuilderExportTplDesc";

            node.local = UsfMatrix4.Identity();
            node.localOriginal = UsfMatrix4.Identity();
            node.world = UsfMatrix4.Identity();
            node.uid = (int)UsfNodeCommonUID.Character;
            return node;
        }
        public static UsfNode BuildSkinnedGeometryNode()
        {
            var node = new UsfNode();//
            node.affixes = "export_preserve_geometry\nuse_hierarchy_name\n0\n\nexport_preserve_position\n\n\n";
            node.pses = new UsfPses(true);
            node.name = "skinned_geometry";
            node.mayaNodeId = "";
            node.sourceId = "|skinned_geometry";

            node.local = UsfMatrix4.Identity();
            node.localOriginal = UsfMatrix4.Identity();
            node.world = UsfMatrix4.Identity();
            node.uid = (int)UsfNodeCommonUID.SkinnedGeom;
            return node;
        }
        public UsfNode() { }

        public void AddChild(UsfNode node)
        {
            if (node.parent != null)
            {
                var a = "FUCK YOU BALTIMORE";
            }
            node.parent = this;
            children.Add(node);
        }
        public void Read(BinaryReader reader)
        {
            int version = reader.ReadInt32();
            uid = reader.ReadInt32();
            name = UsfString.Read(reader);
            modelName = UsfString.Read(reader);
            affixes = UsfString.Read(reader);
            pses.Read(reader);
            local.Read(reader);
            world.Read(reader);

            if (version >= 0x10D)
            {
                localOriginal.Read(reader);
            }
            else
            {
                localOriginal = local;
            }

            sourceId = UsfString.Read(reader);

            if (version >= 0x10C)
            {
                byte lwiInfoFlag = reader.ReadByte();
                if (lwiInfoFlag != 0)
                {
                    lwiInfo = new UsfLwiInfo();
                    lwiInfo.Read(reader);
                }
            }
            byte meshFlag = reader.ReadByte();
            if (meshFlag != 0)
            {
                mesh = new UsfNodeMesh();
                mesh.Read(reader);
            }
            byte animationFlag = reader.ReadByte();
            if (animationFlag != 0)
            {
                animation = new UsfAnimation();
                animation.Read(reader);
            }
            byte saberActorInfoFlag = reader.ReadByte();
            if (saberActorInfoFlag != 0)
            {
                actor = new UsfSaberActor();
                actor.Read(reader);
            }
            byte reflocFlag = reader.ReadByte();
            if (reflocFlag != 0)
            {
                throw new NotImplementedException();
                //AddReflocInfo();
                // GetReflocInfo()->Load(f);
            }
            byte lightInfoFlag = reader.ReadByte();
            if (lightInfoFlag != 0)
            {
                throw new NotImplementedException();
                //AddLightInfo();
                //GetLightInfo()->Load(f);
            }
            byte cameraInfoFlag = reader.ReadByte();
            if (cameraInfoFlag != 0)
            {
                cameraInfo = new UsfCameraInfo();
                if (version >= 0x106)
                {
                    cameraInfo.Read(reader);
                }
            }
            if (version >= 0x105)
            {
                byte navWPFlag = reader.ReadByte();
                if (navWPFlag != 0)
                {
                    throw new NotImplementedException();
                    //AddNavWPInfo();
                    //GetNavWPInfo()->Load(f);
                }
                byte navNSFlag = reader.ReadByte();
                if (navNSFlag != 0)
                {
                    throw new NotImplementedException();
                    //AddNavNSInfo();
                    //GetNavNSInfo()->Load(f);
                }
            }
            if (version >= 0x107)
            {
                byte refDescInfoFlag = reader.ReadByte();
                if (refDescInfoFlag != 0)
                {
                    throw new NotImplementedException();
                    //AddRefDescInfo();
                    //GetRefDescInfo()->Load(f);
                }
            }
            if (version >= 0x108)
            {
                byte decalInfoFlag = reader.ReadByte();
                if (decalInfoFlag != 0)
                {
                    throw new NotImplementedException();
                    //AddDecalInfo();
                    //GetDecalInfo()->Load(f);
                }
            }
            if (version >= 0x109)
            {
                byte animationExtraFlag = reader.ReadByte();
                if (animationExtraFlag != 0)
                {
                    throw new NotImplementedException();
                    //AddAnimationExtra();
                    //GetAnimationExtra()->Load(f);
                }
            }
            if (version >= 0x10A)
            {
                byte affixesFlag = reader.ReadByte();
                if (affixesFlag != 0)
                {
                    throw new NotImplementedException();
                    //AddTypedAffixes();
                    //GetTypedAffixes()->Load(f);
                }
            }
            if (version >= 0x10E)
            {
                byte ecsEntityFlag = reader.ReadByte();
                if (ecsEntityFlag != 0)
                {
                    throw new NotImplementedException();
                    //AddEcsEntity();
                    //GetEcsEntity()->Load(f);
                }
            }
            if (version >= 0x10B)
            {
                mayaNodeId = UsfString.Read(reader);
            }

            var num = reader.ReadInt32();
            for (var i = 0; i < num; i++)
            {
                var child = new UsfNode();
                child.Read(reader);
                child.parent = this;
                children.Add(child);
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(270); //check version
            writer.Write(uid);
            UsfString.Write(writer, name);
            UsfString.Write(writer, modelName);
            UsfString.Write(writer, affixes);
            pses.Write(writer);
            local.Write(writer);
            world.Write(writer);
            localOriginal.Write(writer);


            UsfString.Write(writer, sourceId);
            if (lwiInfo != null)
            {
                writer.Write((byte)1);
                lwiInfo.Write(writer);
            }
            else
            {
                writer.Write((byte)0);
            }

            if (mesh != null)
            {
                writer.Write((byte)1);
                mesh.Write(writer);
            }
            else
            {
                writer.Write((byte)0);
            }

            if (animation != null)
            {
                writer.Write((byte)1);
                animation.Write(writer);
            }
            else
            {
                writer.Write((byte)0);
            }

            if (actor != null)
            {
                writer.Write((byte)1);
                actor.Write(writer);
            }
            else
            {
                writer.Write((byte)0);
            }

            writer.Write((byte)0); //refLoc
            writer.Write((byte)0); //lightInfo
            if (cameraInfo != null)
            {
                writer.Write((byte)1);
                cameraInfo.Write(writer);
            }
            else
            {
                writer.Write((byte)0);
            }
            writer.Write((byte)0); //navWP
            writer.Write((byte)0); //navNS
            writer.Write((byte)0); //refDesc
            writer.Write((byte)0); //decalInfo
            writer.Write((byte)0); //animExtra
            writer.Write((byte)0); //affixes
            writer.Write((byte)0); //ecs

            UsfString.Write(writer, mayaNodeId);

            writer.Write(children.Count);
            for (var i = 0; i < children.Count; i++)
            {
                children[i].Write(writer);
            }
        }

        public int uid { get; set; }
        public string name { get; set; } = "";
        public string modelName { get; set; } = "";
        public string affixes { get; set; } = "";
        public UsfPses pses { get; set; } = new UsfPses();
        public UsfMatrix4 local { get; set; } = new UsfMatrix4();
        public UsfMatrix4 localOriginal { get; set; } = new UsfMatrix4();
        public UsfMatrix4 world { get; set; } = new UsfMatrix4();
        public string sourceId { get; set; } = "";
        public UsfLwiInfo lwiInfo { get; set; }
        public UsfNodeMesh mesh { get; set; }
        public UsfSaberActor actor { get; set; }
        public UsfAnimation animation { get; set; }
        public UsfCameraInfo cameraInfo { get; set; }
        public string mayaNodeId { get; set; }
        public List<UsfNode> children { get; set; } = new List<UsfNode>();

        public UsfNode GetParent()
        {
            return parent;
        }

        private UsfNode parent;
    }
}
