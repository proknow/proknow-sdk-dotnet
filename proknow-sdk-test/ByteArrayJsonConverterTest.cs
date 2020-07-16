using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace ProKnow.Test
{
    [TestClass]
    public class ByteArrayJsonConverterTest
    {
        private readonly ByteArrayJsonConverter _byteArrayJsonConverter = new ByteArrayJsonConverter();

        [TestMethod]
        public void ReadTest()
        {
            var jsonString = "[255,128,0]";
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(_byteArrayJsonConverter);
            var bytes = JsonSerializer.Deserialize<byte[]>(jsonString, jsonSerializerOptions);
            Assert.AreEqual(3, bytes.Length);
            Assert.AreEqual(255, bytes[0]);
            Assert.AreEqual(128, bytes[1]);
            Assert.AreEqual(0, bytes[2]);
        }

        [TestMethod]
        public void WriteTest()
        {
            var bytes = new byte[] { 255, 128, 0 };
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(_byteArrayJsonConverter);
            var jsonString = JsonSerializer.Serialize<byte[]>(bytes, jsonSerializerOptions);
            Assert.AreEqual("[255,128,0]", jsonString);
        }
    }
}
