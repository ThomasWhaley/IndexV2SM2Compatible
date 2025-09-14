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
    public class UsfSaberActor
    {
        public void Read(BinaryReader reader)
        {
            int version = reader.ReadInt32();
            actorTypeString = UsfString.Read(reader);
            actorType = getTypeByString[actorTypeString];
            tplName = UsfString.Read(reader);
            int animCount = reader.ReadInt32();
            animations = new Anim[animCount];
            for (int i = 0; i < animCount; ++i)
            {
                animations[i] = new Anim();
                animations[i].Read(reader);
            }

            int sourceCount = reader.ReadInt32();
            sourceUsfUids = new int[sourceCount];
            for (int i = 0; i < sourceCount; ++i)
            {
                sourceUsfUids[i] = reader.ReadInt32();
            }

            refName = UsfString.Read(reader);

            if (version >= 0x107)
            { //mark2
                byte nestedFlag = reader.ReadByte();
                if (nestedFlag != 0)
                {
                    isNested = reader.ReadInt32() != 0;
                }
                else
                {
                    isNested = false;
                }

                byte rtcFlag = reader.ReadByte();
                if (rtcFlag != 0)
                {
                    isRTC = reader.ReadInt32() != 0;
                }
                else
                {
                    isRTC = false;
                }

                byte clsFlag = reader.ReadByte();
                if (clsFlag != 0)
                {
                    clsName = UsfString.Read(reader);
                }
                else
                {
                    clsName = "";
                }

                if (version >= 0x108)
                {
                    byte subnedtedFlag = reader.ReadByte();
                    if (subnedtedFlag != 0)
                    {
                        isSubNested = reader.ReadInt32() != 0;
                    }
                    else
                    {
                        isSubNested = false;
                    }
                }
            }

            return;
        }

        public void Write(BinaryWriter writer)
        {
            Int32 version = 264;
            writer.Write(version);
            UsfString.Write(writer, actorTypeString);
            UsfString.Write(writer, tplName);
            writer.Write((Int32)animations.Length);
            for (int i = 0; i < animations.Length; ++i)
            {
                animations[i].Write(writer);
            }

            writer.Write((Int32)sourceUsfUids.Length);
            for (int i = 0; i < sourceUsfUids.Length; ++i)
            {
                writer.Write(sourceUsfUids[i]);
            }

            UsfString.Write(writer, refName);

            writer.Write((byte)1);
            writer.Write(isNested ? 1 : 0);
            writer.Write((byte)1);
            writer.Write(isRTC ? 1 : 0);
            writer.Write((byte)1);
            UsfString.Write(writer, clsName);
            writer.Write((byte)1);
            writer.Write(isSubNested ? 1 : 0);
            return;
        }

        private static Dictionary<string, UsfActorType> getTypeByString = new Dictionary<string, UsfActorType>()
        {
            { "external", UsfActorType.USF_ACTORTYPE_EXTERNAL },
            { "nested", UsfActorType.USF_ACTORTYPE_EXTERNAL },
            { "rtc", UsfActorType.USF_ACTORTYPE_EXTERNAL },
            { "scene_desc", UsfActorType.USF_ACTORTYPE_EXTERNAL },
            { "tpl_desc", UsfActorType.USF_ACTORTYPE_EXTERNAL },
            { "ref_desc", UsfActorType.USF_ACTORTYPE_EXTERNAL },
            { "mark2", UsfActorType.USF_ACTORTYPE_EXTERNAL }
        };

        public string actorTypeString { get; set; } = "tpl_desc";
        public UsfActorType actorType { get; set; } = UsfActorType.USF_ACTORTYPE_EXTERNAL;
        public string tplName { get; set; } = "";
        public Anim[] animations { get; set; } = new Anim[0];
        public int[] sourceUsfUids { get; set; } = new int[0];
        public string refName { get; set; } = "";
        public bool isNested { get; set; } = false;
        public bool isRTC { get; set; } = false;
        public string clsName { get; set; } = "";
        public bool isSubNested { get; set; } = false;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UsfActorType
    {
        USF_ACTORTYPE_EXTERNAL,
        USF_ACTORTYPE_NESTED,
        USF_ACTORTYPE_RTC,
        USF_ACTORTYPE_SCENE_DESC,
        USF_ACTORTYPE_TPL_DESC,
        USF_ACTORTYPE_REF_DESC,
        USF_ACTORTYPE_MARK2

    };
    public class ActionFrame
    {
        public void Read(BinaryReader reader)
        {
            id = reader.ReadInt32();
            frame = reader.ReadInt32();
            comment = UsfString.Read(reader);
        }
        public void Write(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(frame);
            UsfString.Write(writer, comment);
        }
        public int id { get; set; }
        public int frame { get; set; }
        public string comment { get; set; }
    };
    public struct Anim
    {
        public void Read(BinaryReader reader)
        {
            name = UsfString.Read(reader);
            begin = reader.ReadInt32();
            end = reader.ReadInt32();
            time = reader.ReadSingle();
            var actionCount = reader.ReadInt32();
            actionFrames = new ActionFrame[actionCount];
            for (int i = 0; i < actionCount; ++i)
            {
                actionFrames[i] = new ActionFrame();
                actionFrames[i].Read(reader);
            }
        }
        public void Write(BinaryWriter writer)
        {
            UsfString.Write(writer, name);
            writer.Write(begin);
            writer.Write(end);
            writer.Write(time);
            writer.Write(actionFrames.Length);
            for (var i = 0; i < actionFrames.Length; ++i)
            {
                actionFrames[i].Write(writer);
            }
        }
        public string name { get; set; }
        public int begin { get; set; }
        public int end { get; set; }
        public float time { get; set; }
        public ActionFrame[] actionFrames { get; set; }
    };
}
