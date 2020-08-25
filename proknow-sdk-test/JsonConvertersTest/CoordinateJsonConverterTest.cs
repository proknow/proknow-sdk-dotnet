using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace ProKnow.JsonConverters.Test
{
    [TestClass]
    public class CoordinateJsonConverterTest
    {
        private readonly CoordinateJsonConverter _coordinateJsonConverter = new CoordinateJsonConverter();

        [TestMethod]
        public void ReadTest()
        {
            var jsonString = "12345";
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(_coordinateJsonConverter);
            var coordinate = JsonSerializer.Deserialize<double>(jsonString, jsonSerializerOptions);
            Assert.AreEqual(12.345, coordinate, 1e-10);
        }

        [TestMethod]
        public void WriteTest()
        {
            var coordinate = 12.345;
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(_coordinateJsonConverter);
            var jsonString = JsonSerializer.Serialize(coordinate, jsonSerializerOptions);
            Assert.AreEqual("12345", jsonString);
        }
    }
}
