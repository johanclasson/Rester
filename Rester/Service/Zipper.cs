using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Rester.Service
{
    public interface IZipper
    {
        Task WriteCompressedDataToStreamAsync(Stream streamToWriteTo, string data);
        Task<string> GetDataFromCompressedStreamAsync(Stream compressedStream);
    }

    internal class Zipper : IZipper
    {
        public Task WriteCompressedDataToStreamAsync(Stream streamToWriteTo, string data)
        {
            return Task.Run(() =>
            {
                using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
                using (var compressionStream = new GZipStream(streamToWriteTo, CompressionLevel.Optimal))
                {
                    uncompressedStream.CopyTo(compressionStream);
                }
            });
        }

        public Task<string> GetDataFromCompressedStreamAsync(Stream compressedStream)
        {
            return Task.Run(() =>
            {
                using (var decompress = new GZipStream(compressedStream, CompressionMode.Decompress))
                using (var sr = new StreamReader(decompress))
                {
                    return sr.ReadToEnd();
                }
            });
        }
    }
}