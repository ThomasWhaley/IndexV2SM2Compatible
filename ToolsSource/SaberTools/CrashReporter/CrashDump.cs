using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaberTools.CrashReporter
{
    public class CrashDump
    {
        [JsonProperty(PropertyName = "leech-arguments")]
        public LeechArguments leech_arguments { get; set; }
        public static CrashDump Build(string text)
        {
            var decodedArgument = DecodeArgument(text);
            if (decodedArgument == null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<CrashDump>(decodedArgument);
        }

        private static string DecodeArgument(string text)
        {
            try
            {
                var textBuffer = Convert.FromBase64String(text);
                int magic = 0;
                int decompressedSize = 0;
                byte[] gzipBuffer;
                using (var stream = new MemoryStream(textBuffer))
                {
                    var reader = new BinaryReader(stream);
                    magic = reader.ReadUInt16();
                    decompressedSize = reader.ReadUInt16();
                    gzipBuffer = reader.ReadBytes((int)stream.Length);
                }
                string resultString;
                using (var stream = new MemoryStream(gzipBuffer))
                using (var zipStream = new ZLibStream(stream, CompressionMode.Decompress))
                {
                    var buffer = new byte[decompressedSize];
                    zipStream.Read(buffer, 0, buffer.Length);
                    resultString = Encoding.UTF8.GetString(buffer);
                }

                return resultString;
            } 
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class LeechArguments
    {
        [JsonProperty(PropertyName = "dump-path")]
        public string dump_path { get; set; }
    }
}
