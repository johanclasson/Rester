using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Rester.Tests
{
    public class ZipperTests : ResterTestBase
    {
        private string Data => SerializationTests.SerializedServiceConfigurations;

        [Fact]
        public async void SerializedConfiguration_ShouldBeMuchLessThanTheOriginalStream()
        {
            using (var compressedStream = new MemoryStream())
            {
                await Zipper.WriteCompressedDataToStream(compressedStream, Data);
                int uncompressedLength = Encoding.UTF8.GetBytes(Data).Length;
                int compressedLength = compressedStream.ToArray().Length;
                compressedLength.Should().BeLessThan(uncompressedLength / 4);
            }
        }

        [Fact]
        public async void DecompressingAComporessedStream_ShouldYieldTheOriginalData()
        {
            using (var compressedStream = new MemoryStream())
            {
                await Zipper.WriteCompressedDataToStream(compressedStream, Data);
                using (var readFromStream = new MemoryStream(compressedStream.ToArray()))
                {
                    string data = await Zipper.GetDataFromCompressedStream(readFromStream);
                    data.Should().Be(Data);
                }
            }
        }
    }
}