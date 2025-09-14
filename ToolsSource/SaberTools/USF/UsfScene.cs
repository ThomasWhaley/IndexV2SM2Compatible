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
    public class UsfScene
    {
        public void Read(BinaryReader reader)
        {
            version = reader.ReadUInt32();
            byte hasRoot = reader.ReadByte();
            if (hasRoot != 0)
            {
                root = new UsfNode();
                root.Read(reader);
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(version);
            if (root == null)
            {
                writer.Write((byte)0);
                return;
            }
            writer.Write((byte)1);
            root.Write(writer);
        }

        public void Initialize(string actorRootName, bool isCharacter)
        {
            if (root == null) root = GenerateRoot(actorRootName, isCharacter);
        }

        private UsfNode GenerateRoot(string actorRootName, bool isCharacter)
        {
            var rootNode = UsfNode.BuildRootNode();
            var dxNode = UsfNode.BuildDxNode();
            UsfNode skinNode = null;
            if (isCharacter)
            {
                skinNode = UsfNode.BuildSkinnedGeometryNode();
            }
            if (!isCharacter)
            {
                var actorNode = UsfNode.BuildActorNode(actorRootName);
                rootNode.AddChild(dxNode);
                if (skinNode != null) rootNode.AddChild(skinNode);
                rootNode.AddChild(actorNode);
            }
            else
            {
                var characterNode = UsfNode.BuildCharNode(actorRootName);
                var characterRootNode = UsfNode.BuildCharacterROOTNode();
                rootNode.AddChild(characterRootNode);
                rootNode.AddChild(dxNode);
                if (skinNode != null) rootNode.AddChild(skinNode);
                rootNode.AddChild(characterNode);
            }
            return rootNode;
        }

        public uint version { get; set; } = 256;
        public UsfNode root { get; set; }

    }



}
