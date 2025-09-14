using SaberTools.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

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

    public class UsfNodeMesh
    {
        public void Read(BinaryReader reader)
        {
            Int32 version = reader.ReadInt32();
            int vertexCount = reader.ReadInt32();
            vertixes = new UsfVector3[vertexCount];
            for (int i = 0; i < vertexCount; ++i)
            {
                vertixes[i] = new UsfVector3();
                vertixes[i].Read(reader);
            }
            int normalCount = reader.ReadInt32();
            normals = new UsfVector3[normalCount];
            for (int i = 0; i < normalCount; ++i)
            {
                normals[i] = new UsfVector3();
                normals[i].Read(reader);
            }
            if (version < 0x102)
            {
                int colorCount = reader.ReadInt32();
                if (colorCount == 0)
                {
                    colors = null;
                }
                else
                {
                    colors = new UInt32[1][];
                    colors[0] = new UInt32[colorCount];
                    for (int i = 0; i < colorCount; ++i)
                    {
                        colors[0][i] = reader.ReadUInt32();
                    }
                }
            }
            else
            {
                int colorCount = reader.ReadInt32();
                colors = new UInt32[colorCount][];
                for (int i = 0; i < colorCount; i++)
                {
                    int colorCount2 = reader.ReadInt32();
                    colors[i] = new UInt32[colorCount2];
                    for (int j = 0; j < colorCount2; ++j)
                    {
                        colors[i][j] = reader.ReadUInt32();
                    }
                }
            }
            int uvSetCount = reader.ReadInt32();
            uvSets = new UsfUVSet[uvSetCount];
            for (int i = 0; i < uvSetCount; i++)
            {
                int uvCount = reader.ReadInt32();
                uvSets[i] = new UsfUVSet();
                uvSets[i].uvs = new UsfVector2[uvCount];
                for (int j = 0; j < uvCount; ++j)
                {
                    uvSets[i].uvs[j] = new UsfVector2();
                    uvSets[i].uvs[j].Read(reader);
                }
                if (version >= 0x107)
                {
                    uvSets[i].name = UsfString.Read(reader);
                }
                else
                {
                    uvSets[i].name = $"uv{i}";
                }
            }

            if (version >= 0x103)
            {
                int tangentCount = reader.ReadInt32();
                tangents = new UsfVector4[tangentCount][];
                for (int i = 0; i < tangentCount; i++)
                {
                    int tangentCount2 = reader.ReadInt32();
                    tangents[i] = new UsfVector4[tangentCount2];
                    for (int j = 0; j < tangentCount2; ++j)
                    {
                        tangents[i][j] = new UsfVector4();
                        tangents[i][j].Read(reader);
                    }
                }
            }
            else
            {
                tangents = new UsfVector4[uvSetCount][];
            }

            int faceCount = reader.ReadInt32();
            faces = new UsfFace[faceCount];
            for (int i = 0; i < faceCount; ++i)
            {
                faces[i] = new UsfFace();
                faces[i].Read(reader);
            }

            int materialCount = reader.ReadInt32();
            materials = new UsfMaterial[materialCount];

            for (int i = 0; i < materialCount; i++)
            {
                materials[i] = new UsfMaterial();
                materials[i].Read(reader);
            }

            int bonesPerVertex = reader.ReadInt32();
            int skinCount = reader.ReadInt32();
            skins = new UsfSkinVtx[skinCount];
            for (int i = 0; i < skinCount; ++i)
            {
                var skin = new UsfSkinVtx(bonesPerVertex);
                skin.Read(reader);
                skins[i] = skin;
            }

            if (version >= 0x104)
            {
                skinningMethod = (SkinnginMethod)reader.ReadByte();
            }
            else
            {
                skinningMethod = SkinnginMethod.CLASSIC_LINEAR;
            }

            int boneCount = reader.ReadInt32();
            bones = new UsfBone[boneCount];
            for (int i = 0; i < boneCount; i++)
            {
                var bone = new UsfBone();
                bone.Read(reader);
                bones[i] = bone;
            }

            if (version >= 0x105)
            {
                int bonePairCount = reader.ReadInt32();
                bonePairs = new KeyValuePair<int, int>[bonePairCount];
                for (int i = 0; i < bonePairCount; ++i)
                {
                    var pair = new KeyValuePair<int, int>(reader.ReadInt32(), reader.ReadInt32());
                    bonePairs[i] = pair;
                }
            }

            if (version >= 0x106)
            {
                int blendShapeCount = reader.ReadInt32();
                blendShapes = new KeyValuePair<string, UsfSpl>[blendShapeCount];
                for (int i = 0; i < blendShapeCount; ++i)
                {
                    var key = UsfString.Read(reader);
                    var spline = new UsfSpl();
                    spline.Read(reader);
                    var shape = new KeyValuePair<string, UsfSpl>(key, spline);
                    blendShapes[i] = shape;
                }
            }
        }

        public void Write(BinaryWriter writer)
        {
            Int32 version = 263;
            writer.Write(version);

            writer.Write(vertixes.Length);
            for (int i = 0; i < vertixes.Length; ++i)
            {
                vertixes[i].Write(writer);
            }

            writer.Write(normals.Length);
            for (int i = 0; i < normals.Length; ++i)
            {
                normals[i].Write(writer);
            }

            writer.Write(colors.Length);
            for (int i = 0; i < colors.Length; i++)
            {
                var colors2 = colors[i];
                writer.Write(colors2.Length);
                for (int j = 0; j < colors2.Length; ++j)
                {
                    writer.Write(colors[i][j]);
                }
            }

            writer.Write(uvSets.Length);
            for (int i = 0; i < uvSets.Length; i++)
            {
                writer.Write(uvSets[i].uvs.Length);
                for (int j = 0; j < uvSets[i].uvs.Length; ++j)
                {
                    uvSets[i].uvs[j].Write(writer);
                }
                UsfString.Write(writer, uvSets[i].name);
            }

            writer.Write(tangents.Length);
            for (int i = 0; i < tangents.Length; i++)
            {
                writer.Write(tangents[i].Length);
                for (int j = 0; j < tangents[i].Length; ++j)
                {
                    tangents[i][j].Write(writer);
                }
            }

            writer.Write(faces.Length);
            for (int i = 0; i < faces.Length; ++i)
            {
                if (faces[i].materialIndex == 1)
                {
                    var a = 0;
                }
                if (faces[i].materialIndex == 2)
                {
                    var a = 0;
                }
                faces[i].Write(writer);
            }

            writer.Write(materials.Length);
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].Write(writer);
            }

            if (skins.Length == 0)
            {
                writer.Write(0);
                writer.Write(0);
            }
            else
            {
                writer.Write(skins[0].boneIdx.Length);
                writer.Write(skins.Length);
                for (int i = 0; i < skins.Length; ++i)
                {
                    skins[i].Write(writer);
                }
            }


            writer.Write((byte)skinningMethod);

            writer.Write(bones.Length);
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].Write(writer);
            }


            writer.Write(bonePairs.Length);
            for (int i = 0; i < bonePairs.Length; ++i)
            {
                writer.Write(bonePairs[i].Key);
                writer.Write(bonePairs[i].Value);
            }

            writer.Write(blendShapes.Length);
            for (int i = 0; i < blendShapes.Length; ++i)
            {
                UsfString.Write(writer, blendShapes[i].Key);
                blendShapes[i].Value.Write(writer);
            }

        }

        public UsfVector3[] vertixes { get; set; }
        public UsfVector3[] normals { get; set; }
        public UInt32[][] colors { get; set; }
        public UsfUVSet[] uvSets { get; set; }
        public UsfVector4[][] tangents { get; set; }
        public UsfFace[] faces { get; set; }
        public UsfMaterial[] materials { get; set; }
        public UsfSkinVtx[] skins { get; set; }
        public SkinnginMethod skinningMethod { get; set; }
        public UsfBone[] bones { get; set; }
        public KeyValuePair<Int32, Int32>[] bonePairs { get; set; }
        public KeyValuePair<string, UsfSpl>[] blendShapes { get; set; }
    }
    public class UsfBone
    {
        public void Read(BinaryReader reader)
        {
            usfNodeUid = reader.ReadInt32();
            bindMatrix.Read(reader);
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(usfNodeUid);
            bindMatrix.Write(writer);
        }
        public Int32 usfNodeUid { get; set; }
        public UsfMatrix4 bindMatrix { get; set; } = new UsfMatrix4();
    }
    public class UsfSkinVtx
    {
        public UsfSkinVtx(int bonesPerVertex)
        {
            boneIdx = new int[bonesPerVertex];
            weight = new float[bonesPerVertex];
        }

        public void Read(BinaryReader reader)
        {
            for (int i = 0; i < boneIdx.Length; ++i)
            {
                boneIdx[i] = reader.ReadInt32();
            }
            for (int i = 0; i < weight.Length; ++i)
            {
                weight[i] = reader.ReadSingle();
            }
        }

        public void Write(BinaryWriter writer)
        {
            for (int i = 0; i < boneIdx.Length; ++i)
            {
                writer.Write(boneIdx[i]);
            }
            for (int i = 0; i < weight.Length; ++i)
            {
                writer.Write(weight[i]);
            }
        }

        public int[] boneIdx { get; set; }
        public float[] weight { get; set; }
    };
    public class UsfMaterial
    {
        public void Read(BinaryReader reader)
        {
            Int32 version = reader.ReadInt32();

            if (version >= 0x10A)
            {
                vertexColorUsage = (VertexColorUsage)reader.ReadUInt32();
                data = new Ps();
                data.Read(reader);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Write(BinaryWriter writer)
        {
            Int32 version = 266;
            writer.Write(version);
            writer.Write((UInt32)vertexColorUsage);
            data.Write(writer);

        }

        public void InitializeDefault()
        {
            var defaultStr = "auxiliaryTextures = {\n   mask = {\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n}\ncolorSets = [ ]\ndynamicParameters = [\n]\nextraUVData = {\n   uvSetIdx = -1\n}\nextraVertexColorData = {\n   colorA = {\n      colorSetIdx = -1\n   }\n   colorB = {\n      colorSetIdx = -1\n   }\n   colorG = {\n      colorSetIdx = -1\n   }\n   colorR = {\n      colorSetIdx = -1\n   }\n}\nlayers = [\n   {\n      albedo_tint = [\n         255,\n         255,\n         255,\n         255\n      ]\n      blending = {\n         heightmap = {\n            colorSetIdx = -1\n            invert = 0\n            softnessMultiplier = 1.000000\n            useHeightmapBlending = 0\n         }\n         normalAdditiveBlend = 0\n         upVector = {\n            angle = 90.000000\n            enabled = 0\n            falloff = 0.000000\n         }\n         weights = {\n            colorSetIdx = -1\n            multiplier = 1.000000\n            useFromLayerAlpha = 0\n            useFromMaskChannel = -1\n         }\n      }\n      heightmapOverride = \"\"\n      heightmapUVOverride = {\n         enabled = 0\n         tilingU = 1.000000\n         tilingV = 1.000000\n         uvSetIdx = -1\n      }\n      isVisible = 1\n      subMaterial = \"default\"\n      textureName = \"ch_hormagaunt_head\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      type = 0\n      uvSetIdx = 0\n   }\n]\nmaterial = {\n   name = \"ch_hormagaunt_head_mat0\"\n   parent = \"\"\n   subMaterial = \"\"\n   type = 0\n   version = 2\n}\nocclusion = {\n   occlusionTexture = {\n      colorSetIdx = -1\n      isVisible = 1\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n}\nparallax = {\n   baseLayerParallax = {\n      colorSetIdx = -1\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = 0\n   }\n   parallaxSettings = {\n      enableSecondParallaxLayer = 0\n      flatten = {\n         colorSetIdx = -1\n      }\n      overrideSecondParallaxTexture = 0\n   }\n   secondLayerParallax = {\n      colorSetIdx = -1\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = 0\n   }\n}\nreliefNormalmaps = {\n   macro = {\n      end = 0.000000\n      falloff = 5.000000\n      isVisible = 1\n      scale = 1.000000\n      start = 0.000000\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n   micro1 = {\n      end = 0.000000\n      falloff = 5.000000\n      isVisible = 1\n      scale = 1.000000\n      start = 0.000000\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n   micro2 = {\n      end = 0.000000\n      falloff = 5.000000\n      isVisible = 1\n      scale = 1.000000\n      start = 0.000000\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n}\ntransparency = {\n   colorSetIdx = -1\n   dirtModifyAlpha = 0\n   enabled = 0\n   multiplier = 1.000000\n   sources = 0\n   useBlendMaskAlpha = 0\n}\nuvSets = [\n   {\n      isUsedInLayersOnly = 1\n      name = \"map1\"\n   }\n]";
            data = new Ps(defaultStr);
        }

        public void InitializeDefaultCloth()
        {
            var defaultStr = "auxiliaryTextures = {\n   mask = {\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n}\ncolorSets = [\n   {\n      colorChannelIdx = 0\n      isUsedInLayersOnly = 0\n   },\n   {\n      colorChannelIdx = 1\n      isUsedInLayersOnly = 0\n   },\n   {\n      colorChannelIdx = 2\n      isUsedInLayersOnly = 0\n   },\n   {\n      colorChannelIdx = 3\n      isUsedInLayersOnly = 0\n   }\n]\ndynamicParameters = [\n]\nextraUVData = {\n   uvSetIdx = -1\n}\nextraVertexColorData = {\n   colorA = {\n      colorSetIdx = -1\n   }\n   colorB = {\n      colorSetIdx = -1\n   }\n   colorG = {\n      colorSetIdx = -1\n   }\n   colorR = {\n      colorSetIdx = -1\n   }\n}\nlayers = [\n   {\n      albedo_tint = [\n         255,\n         255,\n         255,\n         255\n      ]\n      blending = {\n         heightmap = {\n            colorSetIdx = -1\n            invert = 0\n            softnessMultiplier = 1.000000\n            useHeightmapBlending = 0\n         }\n         normalAdditiveBlend = 0\n         upVector = {\n            angle = 90.000000\n            enabled = 0\n            falloff = 0.000000\n         }\n         weights = {\n            colorSetIdx = -1\n            multiplier = 1.000000\n            useFromLayerAlpha = 0\n            useFromMaskChannel = -1\n         }\n      }\n      heightmapOverride = \"\"\n      heightmapUVOverride = {\n         enabled = 0\n         tilingU = 1.000000\n         tilingV = 1.000000\n         uvSetIdx = -1\n      }\n      isVisible = 1\n      subMaterial = \"default\"\n      textureName = \"ch_hormagaunt_head\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      type = 0\n      uvSetIdx = 0\n   }\n]\nmaterial = {\n   name = \"ch_hormagaunt_head_mat0\"\n   parent = \"\"\n   subMaterial = \"\"\n   type = 0\n   version = 2\n}\nocclusion = {\n   occlusionTexture = {\n      colorSetIdx = -1\n      isVisible = 1\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n}\nparallax = {\n   baseLayerParallax = {\n      colorSetIdx = -1\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = 0\n   }\n   parallaxSettings = {\n      enableSecondParallaxLayer = 0\n      flatten = {\n         colorSetIdx = -1\n      }\n      overrideSecondParallaxTexture = 0\n   }\n   secondLayerParallax = {\n      colorSetIdx = -1\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = 0\n   }\n}\nreliefNormalmaps = {\n   macro = {\n      end = 0.000000\n      falloff = 5.000000\n      isVisible = 1\n      scale = 1.000000\n      start = 0.000000\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n   micro1 = {\n      end = 0.000000\n      falloff = 5.000000\n      isVisible = 1\n      scale = 1.000000\n      start = 0.000000\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n   micro2 = {\n      end = 0.000000\n      falloff = 5.000000\n      isVisible = 1\n      scale = 1.000000\n      start = 0.000000\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n}\ntransparency = {\n   colorSetIdx = -1\n   dirtModifyAlpha = 0\n   enabled = 0\n   multiplier = 1.000000\n   sources = 0\n   useBlendMaskAlpha = 0\n}\nuvSets = [\n   {\n      isUsedInLayersOnly = 1\n      name = \"map1\"\n   }\n]";
            data = new Ps(defaultStr);
        }
        public void InitializeDefaultDecal()
        {
            var defaultStr = "auxiliaryTextures = {\n   mask = {\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n}\ncolorSets = [\n   {\n      colorChannelIdx = 12\n      isUsedInLayersOnly = 0\n   }\n]\ndynamicParameters = [\n]\nextraUVData = {\n   uvSetIdx = -1\n}\nextraVertexColorData = {\n   colorA = {\n      colorSetIdx = -1\n   }\n   colorB = {\n      colorSetIdx = -1\n   }\n   colorG = {\n      colorSetIdx = -1\n   }\n   colorR = {\n      colorSetIdx = -1\n   }\n}\nlayers = [\n   {\n      albedo_tint = [\n         255,\n         255,\n         255,\n         255\n      ]\n      blending = {\n         heightmap = {\n            colorSetIdx = -1\n            invert = 0\n            softnessMultiplier = 1.000000\n            useHeightmapBlending = 0\n         }\n         normalAdditiveBlend = 0\n         upVector = {\n            angle = 90.000000\n            enabled = 0\n            falloff = 0.000000\n         }\n         weights = {\n            colorSetIdx = -1\n            multiplier = 1.000000\n            useFromLayerAlpha = 0\n            useFromMaskChannel = -1\n         }\n      }\n      heightmapOverride = \"\"\n      heightmapUVOverride = {\n         enabled = 0\n         tilingU = 1.000000\n         tilingV = 1.000000\n         uvSetIdx = -1\n      }\n      isVisible = 1\n      subMaterial = \"default\"\n      textureName = \"ch_lgd\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      type = 0\n      uvSetIdx = 0\n   }\n]\nmaterial = {\n   name = \"ch_lgd_mat0\"\n   parent = \"\"\n   subMaterial = \"\"\n   type = 0\n   version = 2\n}\nocclusion = {\n   occlusionTexture = {\n      colorSetIdx = -1\n      isVisible = 1\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n}\nparallax = {\n   baseLayerParallax = {\n      colorSetIdx = -1\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = 0\n   }\n   parallaxSettings = {\n      enableSecondParallaxLayer = 0\n      flatten = {\n         colorSetIdx = -1\n      }\n      overrideSecondParallaxTexture = 0\n   }\n   secondLayerParallax = {\n      colorSetIdx = -1\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = 0\n   }\n}\nreliefNormalmaps = {\n   macro = {\n      end = 0.000000\n      falloff = 5.000000\n      isVisible = 1\n      scale = 1.000000\n      start = 0.000000\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n   micro1 = {\n      end = 0.000000\n      falloff = 5.000000\n      isVisible = 1\n      scale = 1.000000\n      start = 0.000000\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n   micro2 = {\n      end = 0.000000\n      falloff = 5.000000\n      isVisible = 1\n      scale = 1.000000\n      start = 0.000000\n      textureName = \"\"\n      tilingU = 1.000000\n      tilingV = 1.000000\n      uvSetIdx = -1\n   }\n}\ntransparency = {\n   colorSetIdx = 0\n   dirtModifyAlpha = 0\n   enabled = 0\n   multiplier = 1.000000\n   sources = 0\n   useBlendMaskAlpha = 1\n}\nuvSets = [\n   {\n      isUsedInLayersOnly = 1\n      name = \"map1\"\n   }\n]";
            data = new Ps(defaultStr);
        }
        public void InitializeDefaultCdt()
        {
            var defaultStr = "material = {\n   version = 2\n   type = 0\n}\nlayers = [\n\n]\nuvSets = [\n\n]\ncolorSets = [\n\n]\n";
            data = new Ps(defaultStr);
        }

        public VertexColorUsage vertexColorUsage { get; set; } = VertexColorUsage.VCU_NO;
        public Ps data { get; set; }
    }

    public class UsfFace
    {
        public UInt32 materialIndex { get; set; }
        public UInt32 edge { get; set; }
        public void Read(BinaryReader reader)
        {
            var data = reader.ReadUInt32();
            materialIndex = data & materialIndexMask;
            edge = (data & edgeMask) >> 29;

            if (materialIndex == 3)
            {
                var a = 0;
            }
        }
        public void Write(BinaryWriter write)
        {
            var data = (edge << 29) | materialIndex;
            write.Write(data);
        }
        private const UInt32 materialIndexMask = 0b00011111111111111111111111111111;
        private const UInt32 edgeMask = 0b11100000000000000000000000000000;
    }


    public class UsfUVSet
    {
        public UsfVector2[] uvs { get; set; }
        public string name { get; set; }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SkinnginMethod
    {
        CLASSIC_LINEAR = 0,
        DUAL_QUATERNION,
        WEIGHT_BLENDED,
        MAX
    };
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VertexColorUsage
    {
        VCU_NO,
        VCU_REQUIRED,
        VCU_OPTIONAL
    };
}
