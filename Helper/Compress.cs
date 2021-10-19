using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Utils {
    public static class GZip {
        public static string compress(string text) {
            if (string.IsNullOrEmpty(text)) { return text; }

            byte[] buffer = Encoding.UTF8.GetBytes(text);

            using(var outStream = new MemoryStream())
            using(var zip = new GZipStream(outStream, CompressionMode.Compress)) {
                zip.Write(buffer, 0, buffer.Length);
                zip.Close();

                string compressedBase64 = Convert.ToBase64String(outStream.ToArray());
                return compressedBase64;
            }
        }

        public static string decompress(string compressed) {
            if (string.IsNullOrEmpty(compressed)) { return compressed; }

            byte[] buffer = Convert.FromBase64String(compressed);

            using(var inStream = new MemoryStream(buffer))
            using(var outStream = new MemoryStream())
            using(var zip = new GZipStream(inStream, CompressionMode.Decompress)) {
                zip.CopyTo(outStream);
                zip.Close();

                string text = Encoding.UTF8.GetString(outStream.ToArray());
                return text;
            }
        }
    }
}