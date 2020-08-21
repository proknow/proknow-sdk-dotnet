using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Text.Json;

namespace ProKnow.JsonConverters.Test
{
    [TestClass]
    public class ColorJsonConverterTest
    {
        private readonly ColorJsonConverter _colorJsonConverter = new ColorJsonConverter();

        [TestMethod]
        public void ReadTest()
        {
            var jsonString = "[119,128,23]";
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(_colorJsonConverter);
            var color = JsonSerializer.Deserialize<Color>(jsonString, jsonSerializerOptions);
            Assert.AreEqual(color.A, (byte)255);
            Assert.AreEqual(color.R, (byte)119);
            Assert.AreEqual(color.G, (byte)128);
            Assert.AreEqual(color.B, (byte)23);
        }

        [TestMethod]
        public void WriteTest()
        {
            var color = Color.FromArgb(119, 128, 23);
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(_colorJsonConverter);
            var jsonString = JsonSerializer.Serialize(color, jsonSerializerOptions);
            Assert.AreEqual("[119,128,23]", jsonString);
        }
    }
}
