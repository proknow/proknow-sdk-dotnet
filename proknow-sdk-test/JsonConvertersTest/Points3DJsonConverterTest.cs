using ProKnow.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace ProKnow.JsonConverters.Test
{
    [TestClass]
    public class Points3DJsonConverterTest
    {
        private readonly Points3DJsonConverter _points3DJsonConverter = new Points3DJsonConverter();

        [TestMethod]
        public void ReadTest()
        {
            var jsonString = "[[12345,23456,34567],[54321,65432,76543]]";
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(_points3DJsonConverter);
            var points = JsonSerializer.Deserialize<Point3D[]>(jsonString, jsonSerializerOptions);
            Assert.AreEqual(2, points.Length);
            var point1 = points[0];
            Assert.AreEqual(12.345, point1.X, 1e-10);
            Assert.AreEqual(23.456, point1.Y, 1e-10);
            Assert.AreEqual(34.567, point1.Z, 1e-10);
            var point2 = points[1];
            Assert.AreEqual(54.321, point2.X, 1e-10);
            Assert.AreEqual(65.432, point2.Y, 1e-10);
            Assert.AreEqual(76.543, point2.Z, 1e-10);
        }

        [TestMethod]
        public void WriteTest()
        {
            var points = new Point3D[] { new Point3D(12.345, 23.456, 34.567), new Point3D(54.321, 65.432, 76.543) };
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(_points3DJsonConverter);
            var jsonString = JsonSerializer.Serialize(points, jsonSerializerOptions);
            Assert.AreEqual("[[12345,23456,34567],[54321,65432,76543]]", jsonString);
        }
    }
}
