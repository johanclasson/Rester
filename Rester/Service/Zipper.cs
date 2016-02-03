using System.IO;
using System.IO.Compression;
using System.Text;

namespace Rester.Service
{
    internal class Zipper : IZipper
    {
        public void WriteCompressedDataToStream(Stream streamToWriteTo, string data)
        {
            using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            using (var compressionStream = new GZipStream(streamToWriteTo, CompressionLevel.Optimal))
            {
                uncompressedStream.CopyTo(compressionStream);
            }
        }

        public string GetDataFromCompressedStream(Stream compressedStream)
        {
            using (var decompress = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var sr = new StreamReader(decompress))
            {
                return sr.ReadToEnd();
            }
        }
    }
}