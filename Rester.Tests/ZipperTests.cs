using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Rester.Tests
{
    public class ZipperTests : ResterTestBase
    {
        private string Data => SerializationTests.SerializedConfigurations;

        [Fact]
        public async void SerializedConfiguration_ShouldBeMuchLessThanTheOriginalStream()
        {
            using (var compressedStream = new MemoryStream())
            {
                await Zipper.WriteCompressedDataToStreamAsync(compressedStream, Data);
                int uncompressedLength = Encoding.UTF8.GetBytes(Data).Length;
                int compressedLength = compressedStream.ToArray().Length;
                compressedLength.Should().BeLessThan(uncompressedLength / 4);
            }
        }

        [Fact]
        public async void DecompressingACompressedStream_ShouldYieldTheOriginalData()
        {
            using (var compressedStream = new MemoryStream())
            {
                await Zipper.WriteCompressedDataToStreamAsync(compressedStream, Data);
                using (var readFromStream = new MemoryStream(compressedStream.ToArray()))
                {
                    string data = await Zipper.GetDataFromCompressedStreamAsync(readFromStream);
                    data.Should().Be(Data);
                }
            }
        }
    }
}