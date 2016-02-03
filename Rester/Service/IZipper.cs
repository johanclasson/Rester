using System.IO;

namespace Rester.Service
{
    public interface IZipper
    {
        void WriteCompressedDataToStream(Stream streamToWriteTo, string data);
        string GetDataFromCompressedStream(Stream compressedStream);
    }
}