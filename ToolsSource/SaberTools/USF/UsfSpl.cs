using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
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
    public class UsfSpl
    {
        public float[] Get()
        {
            var result = new float[splDataSize / 4];
            using (MemoryStream stream = new MemoryStream(splData))
            {
                BinaryReader reader = new BinaryReader(stream);
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = reader.ReadSingle();
                }
            }

            return result;
        }
        public Vector3[] ConvertData(float[] data)
        {
            var result = new Vector3[data.Length / 4];
            for (int i = 0; i < result.Length; i++)
            {
                var idx = (int)data[i * 4];
                result[idx] = new Vector3(data[i * 4 + 1], data[i * 4 + 2], data[i * 4 + 3]);
            }

            return result;
        }
        public Quaternion[] ConvertDataRot(float[] data)
        {
            var result = new Quaternion[data.Length / 5];
            for (int i = 0; i < result.Length; i++)
            {
                var idx = (int)data[i * 5];
                float x = data[i * 5 + 2];
                float y = data[i * 5 + 3];
                float z = data[i * 5 + 4];
                float w = data[i * 5 + 1];
                result[idx] = new Quaternion(x, y , z, w);
            }

            return result;
        }
        public void FromList(List<Vector3> list)
        {
            using (var stream = new MemoryStream(list.Count * 4 * 4))
            {
                var writer = new BinaryWriter(stream);
                for (int i = 0; i < list.Count; ++i)
                {
                    writer.Write((float)i);
                    writer.Write(list[i].X);
                    writer.Write(list[i].Y);
                    writer.Write(list[i].Z);
                }

                splData = stream.ToArray();
                splType = UsfSplType.M3D_SPL_LINEAR3D;
                splDataSize = (uint)splData.Length;
                splValueDataDim = 3;
                splValueDim = 3;
                splNkp = (uint)list.Count;
                splState = 0;
            }
        }
        public void FromList(List<Quaternion> list)
        {
            using (var stream = new MemoryStream(list.Count * 4 * 4))
            {
                var writer = new BinaryWriter(stream);
                for (int i = 0; i < list.Count; ++i)
                {
                    writer.Write((float)i);
                    writer.Write(list[i].W);
                    writer.Write(list[i].X);
                    writer.Write(list[i].Y);
                    writer.Write(list[i].Z);
                }

                splData = stream.ToArray();
                splType = UsfSplType.M3D_SPL_QUAT;
                splDataSize = (uint)splData.Length;
                splValueDataDim = 4;
                splValueDim = 4;
                splNkp = (uint)list.Count;
                splState = 0;
            }
        }
        public void Read(BinaryReader reader)
        {
            ChunkId chunkId = ChunkId.INVALID;
            while (chunkId != ChunkId.FIO_CNK_END)
            {
                chunkId = (ChunkId)reader.ReadUInt16();
                uint skipOffset = reader.ReadUInt32();
                switch (chunkId)
                {
                    case ChunkId.CNK_SPL_TYPE:
                        splType = (UsfSplType)reader.ReadByte();
                        break;
                    case ChunkId.CNK_SPL_STATE:
                        splState = reader.ReadByte();
                        break;
                    case ChunkId.CNK_SPL_VALUE_DIM:
                        splValueDim = reader.ReadByte();
                        break;
                    case ChunkId.CNK_SPL_DATA_DIM:
                        splValueDataDim = reader.ReadByte();
                        break;
                    case ChunkId.CNK_SPL_NKP:
                        splNkp = reader.ReadUInt32();
                        break;
                    case ChunkId.CNK_SPL_DATASIZE:
                        splDataSize = reader.ReadUInt32();
                        break;
                    case ChunkId.CNK_SPL_DATA:
                        splData = reader.ReadBytes((int)splDataSize);
                        break;
                }
            }
        }

        public void Write(BinaryWriter writer)
        {
            //Write splType
            {
                short chunkId = (short)ChunkId.CNK_SPL_TYPE;
                uint skipOffset = (uint)(writer.BaseStream.Position + 2 + 4 + 1);

                writer.Write(chunkId);
                writer.Write(skipOffset);
                writer.Write((byte)splType);
            }
            //Write splState
            {
                short chunkId = (short)ChunkId.CNK_SPL_STATE;
                uint skipOffset = (uint)(writer.BaseStream.Position + 2 + 4 + 1);

                writer.Write(chunkId);
                writer.Write(skipOffset);
                writer.Write((byte)splState);
            }
            //Write splValueDim
            {
                short chunkId = (short)ChunkId.CNK_SPL_VALUE_DIM;
                uint skipOffset = (uint)(writer.BaseStream.Position + 2 + 4 + 1);

                writer.Write(chunkId);
                writer.Write(skipOffset);
                writer.Write((byte)splValueDim);
            }
            //Write splValueDataDim
            {
                short chunkId = (short)ChunkId.CNK_SPL_DATA_DIM;
                uint skipOffset = (uint)(writer.BaseStream.Position + 2 + 4 + 1);

                writer.Write(chunkId);
                writer.Write(skipOffset);
                writer.Write((byte)splValueDataDim);
            }
            //Write splNkp
            {
                short chunkId = (short)ChunkId.CNK_SPL_NKP;
                uint skipOffset = (uint)(writer.BaseStream.Position + 2 + 4 + 4);

                writer.Write(chunkId);
                writer.Write(skipOffset);
                writer.Write(splNkp);
            }
            //Write splDataSize
            {
                short chunkId = (short)ChunkId.CNK_SPL_DATASIZE;
                uint skipOffset = (uint)(writer.BaseStream.Position + 2 + 4 + 4);

                writer.Write(chunkId);
                writer.Write(skipOffset);
                writer.Write(splDataSize);
            }
            //Write splData
            {
                short chunkId = (short)ChunkId.CNK_SPL_DATA;
                uint skipOffset = (uint)(writer.BaseStream.Position + 2 + 4 + splData.Length);

                writer.Write(chunkId);
                writer.Write(skipOffset);
                writer.Write(splData);
            }
            //Write end
            {
                short chunkId = (short)ChunkId.FIO_CNK_END;
                uint skipOffset = (uint)(writer.BaseStream.Position + 2 + 4);

                writer.Write(chunkId);
                writer.Write(skipOffset);
            }
        }

        public UsfSplType splType { get; set; }
        public byte splState { get; set; }
        public byte splValueDim { get; set; }
        public byte splValueDataDim { get; set; }
        public UInt32 splNkp { get; set; }
        public UInt32 splDataSize { get; set; }
        public byte[] splData { get; set; }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UsfSplType
    {
        M3D_SPL_NONE = -1,
        M3D_SPL_LINEAR1D,
        M3D_SPL_LINEAR2D,
        M3D_SPL_LINEAR3D,
        M3D_SPL_HERMIT,
        //M3D_SPL_BEZIER1D,
        M3D_SPL_BEZIER2D, // IP: For backward compatibility
        M3D_SPL_BEZIER3D, // IP: For backward compatibility
        M3D_SPL_LAGRANGE,
        M3D_SPL_QUAT,
        M3D_SPL_COLOR,
        M3D_SPL_COLOR_SEP,
        M3D_SPL_LAST,
        M3D_SPL_BEZIER_IS,
        M3D_SPL_BEZIER1D, // IP: For backward compatibility

        M3D_SPL_UNKNOWN = 1000,
    };
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChunkId
    {
        INVALID = -1,
        FIO_CNK_NONE = 0,
        FIO_CNK_END,
        FIO_CNK_USER = 0x00F0,
        CNK_SPL_TYPE = FIO_CNK_USER,
        CNK_SPL_STATE,
        CNK_SPL_VALUE_DIM,
        CNK_SPL_DATA_DIM,
        CNK_SPL_NKP,
        CNK_SPL_DATASIZE,
        CNK_SPL_DATA
    };

}
