using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace ProKnow.Scorecard.Test
{
    [TestClass]
    public class MetricBinJsonConverterTest
    {
        [TestMethod]
        public void Read_WithoutMinOrMax()
        {
            string json = "{\"label\":\"UNACCEPTABLE\",\"color\":[128,32,96]}";
            var metricBin = JsonSerializer.Deserialize<MetricBin>(json);
            Assert.AreEqual("UNACCEPTABLE", metricBin.Label);
            Assert.AreEqual(128, metricBin.Color[0]);
            Assert.AreEqual(32, metricBin.Color[1]);
            Assert.AreEqual(96, metricBin.Color[2]);
            Assert.IsNull(metricBin.Min);
            Assert.IsNull(metricBin.Max);
        }

        [TestMethod]
        public void Read_WithMin()
        {
            string json = "{\"label\":\"UNACCEPTABLE\",\"color\":[128,32,96],\"min\":3.14}";
            var metricBin = JsonSerializer.Deserialize<MetricBin>(json);
            Assert.AreEqual("UNACCEPTABLE", metricBin.Label);
            Assert.AreEqual(128, metricBin.Color[0]);
            Assert.AreEqual(32, metricBin.Color[1]);
            Assert.AreEqual(96, metricBin.Color[2]);
            Assert.AreEqual(3.14, metricBin.Min);
            Assert.IsNull(metricBin.Max);
        }

        [TestMethod]
        public void Read_WithMax()
        {
            string json = "{\"label\":\"UNACCEPTABLE\",\"color\":[128,32,96],\"min\":1.23,\"max\":3.14}";
            var metricBin = JsonSerializer.Deserialize<MetricBin>(json);
            Assert.AreEqual("UNACCEPTABLE", metricBin.Label);
            Assert.AreEqual(128, metricBin.Color[0]);
            Assert.AreEqual(32, metricBin.Color[1]);
            Assert.AreEqual(96, metricBin.Color[2]);
            Assert.AreEqual(1.23, metricBin.Min);
            Assert.AreEqual(3.14, metricBin.Max);
        }

        [TestMethod]
        public void Read_WithMinAndMax()
        {
            string json = "{\"label\":\"UNACCEPTABLE\",\"color\":[128,32,96],\"max\":3.14}";
            var metricBin = JsonSerializer.Deserialize<MetricBin>(json);
            Assert.AreEqual("UNACCEPTABLE", metricBin.Label);
            Assert.AreEqual(128, metricBin.Color[0]);
            Assert.AreEqual(32, metricBin.Color[1]);
            Assert.AreEqual(96, metricBin.Color[2]);
            Assert.IsNull(metricBin.Min);
            Assert.AreEqual(3.14, metricBin.Max);
        }

        [TestMethod]
        public void Write_WithoutMinOrMax()
        {
            var metricBin = new MetricBin("UNACCEPTABLE", new byte[] { 128, 32, 96 });
            string expected = "{\"label\":\"UNACCEPTABLE\",\"color\":[128,32,96]}";
            var actual = JsonSerializer.Serialize<MetricBin>(metricBin);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Write_WithMin()
        {
            var metricBin = new MetricBin("UNACCEPTABLE", new byte[] { 128, 32, 96 }, 3.14);
            string expected = "{\"label\":\"UNACCEPTABLE\",\"color\":[128,32,96],\"min\":3.14}";
            var actual = JsonSerializer.Serialize<MetricBin>(metricBin);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Write_WithMax()
        {
            var metricBin = new MetricBin("UNACCEPTABLE", new byte[] { 128, 32, 96 }, null, 3.14);
            string expected = "{\"label\":\"UNACCEPTABLE\",\"color\":[128,32,96],\"max\":3.14}";
            var actual = JsonSerializer.Serialize<MetricBin>(metricBin);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Write_WithMinAndMax()
        {
            var metricBin = new MetricBin("UNACCEPTABLE", new byte[] { 128, 32, 96 }, 1.23, 3.14);
            string expected = "{\"label\":\"UNACCEPTABLE\",\"color\":[128,32,96],\"min\":1.23,\"max\":3.14}";
            var actual = JsonSerializer.Serialize<MetricBin>(metricBin);
            Assert.AreEqual(expected, actual);
        }
    }
}
