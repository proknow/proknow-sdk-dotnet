using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Geometry;
using System.Text.Json;

namespace ProKnow.JsonConverters.Test
{
    [TestClass]
    public class Paths2DJsonConverterTest
    {
        private readonly Paths2DJsonConverter _paths2DJsonConverter = new Paths2DJsonConverter();

        [TestMethod]
        public void ReadTest()
        {
            var jsonString = "[[12345,23456,34567,45678,56789,67890],[54321,65432,76543,87654,98765,10987,21098,32109]]";
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(_paths2DJsonConverter);
            var paths = JsonSerializer.Deserialize<Point2D[][]>(jsonString, jsonSerializerOptions);
            Assert.AreEqual(2, paths.Length);
            var path1 = paths[0];
            Assert.AreEqual(3, path1.Length);
            Assert.AreEqual(12.345, path1[0].X, 1e-10);
            Assert.AreEqual(23.456, path1[0].Z, 1e-10);
            Assert.AreEqual(34.567, path1[1].X, 1e-10);
            Assert.AreEqual(45.678, path1[1].Z, 1e-10);
            Assert.AreEqual(56.789, path1[2].X, 1e-10);
            Assert.AreEqual(67.89,  path1[2].Z, 1e-10);
            var path2 = paths[1];
            Assert.AreEqual(4, path2.Length);
            Assert.AreEqual(54.321, path2[0].X, 1e-10);
            Assert.AreEqual(65.432, path2[0].Z, 1e-10);
            Assert.AreEqual(76.543, path2[1].X, 1e-10);
            Assert.AreEqual(87.654, path2[1].Z, 1e-10);
            Assert.AreEqual(98.765, path2[2].X, 1e-10);
            Assert.AreEqual(10.987, path2[2].Z, 1e-10);
            Assert.AreEqual(21.098, path2[3].X, 1e-10);
            Assert.AreEqual(32.109, path2[3].Z, 1e-10);
        }

        [TestMethod]
        public void WriteTest()
        {
            var paths = new Point2D[][] {
                new Point2D[] { new Point2D(12.345, 23.456), new Point2D(34.567, 45.678), new Point2D(56.789, 67.89) },
                new Point2D[] { new Point2D(54.321, 65.432), new Point2D(76.543, 87.654), new Point2D(98.765, 10.987), new Point2D(21.098, 32.109) }
            };
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(_paths2DJsonConverter);
            var jsonString = JsonSerializer.Serialize(paths, jsonSerializerOptions);
            Assert.AreEqual("[[12345,23456,34567,45678,56789,67890],[54321,65432,76543,87654,98765,10987,21098,32109]]", jsonString);
        }
    }
}
