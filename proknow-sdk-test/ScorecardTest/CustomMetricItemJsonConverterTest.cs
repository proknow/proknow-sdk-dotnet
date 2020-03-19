using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.Json;

namespace ProKnow.Scorecard.Test
{
    [TestClass]
    public class CustomMetricItemJsonConverterTest
    {
        [TestMethod]
        public void ReadTest_Id()
        {
            string json = "{\"id\":\"abc123\"}";
            var customMetricItem = JsonSerializer.Deserialize<CustomMetricItem>(json);
            Assert.AreEqual("abc123", customMetricItem.Id);
            Assert.IsNull(customMetricItem.Name);
            Assert.IsNull(customMetricItem.Context);
            Assert.IsNull(customMetricItem.Type);
        }

        [TestMethod]
        public void ReadTest_IdNameContextTypeObjectives()
        {
            string objectivesJson = "[{\"label\":\"PASS\",\"color\":[18,191,0],\"max\":90},{\"label\":\"FAIL\",\"color\":[255,0,0]}]";
            string json = $"{{\"id\":\"5b980134bc004112c9946f5af5dc753a\",\"name\":\"Normalcy of Diet @ 6 mo. (0-100)\",\"context\":\"patient\",\"type\":{{\"number\":{{}}}},\"objectives\":{objectivesJson},\"created_at\":\"2018-09-11T17:54:48.344Z\"}}";
            var customMetricItem = JsonSerializer.Deserialize<CustomMetricItem>(json);
            Assert.AreEqual("5b980134bc004112c9946f5af5dc753a", customMetricItem.Id);
            Assert.AreEqual("Normalcy of Diet @ 6 mo. (0-100)", customMetricItem.Name);
            Assert.AreEqual("patient", customMetricItem.Context);
            Assert.IsNull(customMetricItem.Type.Enum);
            Assert.IsNotNull(customMetricItem.Type.Number);
            Assert.IsNull(customMetricItem.Type.String);
            Assert.AreEqual(2, customMetricItem.Objectives.Count);
            Assert.AreEqual("PASS", customMetricItem.Objectives[0].Label);
            Assert.AreEqual(18, customMetricItem.Objectives[0].Color[0]);
            Assert.AreEqual(191, customMetricItem.Objectives[0].Color[1]);
            Assert.AreEqual(0, customMetricItem.Objectives[0].Color[2]);
            Assert.IsNull(customMetricItem.Objectives[0].Min);
            Assert.AreEqual(90, customMetricItem.Objectives[0].Max);
            Assert.AreEqual("FAIL", customMetricItem.Objectives[1].Label);
            Assert.AreEqual(255, customMetricItem.Objectives[1].Color[0]);
            Assert.AreEqual(0, customMetricItem.Objectives[1].Color[1]);
            Assert.AreEqual(0, customMetricItem.Objectives[1].Color[2]);
            Assert.IsNull(customMetricItem.Objectives[1].Min);
            Assert.IsNull(customMetricItem.Objectives[1].Max);
        }

        [TestMethod]
        public void ReadTest_AllTypes()
        {
            string json1 = "{\"id\":\"5b98011868c06b40b35b88b14ac8894d\",\"name\":\"Genetic-Type\",\"context\":\"patient\",\"type\":{\"enum\":{\"values\":[\"TYPE I\",\"TYPE II\",\"TYPE III\",\"TYPE IV\",\"TYPE V\"]}},\"created_at\":\"2018-09-11T17:53:28.424Z\"}";
            string json2 = "{\"id\":\"5b980134bc004112c9946f5af5dc753a\",\"name\":\"Normalcy of Diet @ 6 mo. (0-100)\",\"context\":\"patient\",\"type\":{\"number\":{}},\"created_at\":\"2018-09-11T17:53:56.757Z\"}";
            string json3 = "{\"id\":\"5b98016854c00417f86e8d098ffc1e00\",\"name\":\"Planner Name\",\"context\":\"patient\",\"type\":{\"string\":{}},\"created_at\":\"2018-09-11T17:54:48.344Z\"}";
            string json = $"[{json1},{json2},{json3}]";
            var customMetricItems = JsonSerializer.Deserialize<IList<CustomMetricItem>>(json);
            Assert.AreEqual(3, customMetricItems.Count);

            // enum
            Assert.AreEqual("5b98011868c06b40b35b88b14ac8894d", customMetricItems[0].Id);
            Assert.AreEqual("Genetic-Type", customMetricItems[0].Name);
            Assert.AreEqual("patient", customMetricItems[0].Context);
            Assert.AreEqual(5, customMetricItems[0].Type.Enum.Values.Length);
            Assert.AreEqual("TYPE I", customMetricItems[0].Type.Enum.Values[0]);
            Assert.AreEqual("TYPE II", customMetricItems[0].Type.Enum.Values[1]);
            Assert.AreEqual("TYPE III", customMetricItems[0].Type.Enum.Values[2]);
            Assert.AreEqual("TYPE IV", customMetricItems[0].Type.Enum.Values[3]);
            Assert.AreEqual("TYPE V", customMetricItems[0].Type.Enum.Values[4]);
            Assert.IsNull(customMetricItems[0].Type.Number);
            Assert.IsNull(customMetricItems[0].Type.String);

            // number
            Assert.AreEqual("5b980134bc004112c9946f5af5dc753a", customMetricItems[1].Id);
            Assert.AreEqual("Normalcy of Diet @ 6 mo. (0-100)", customMetricItems[1].Name);
            Assert.AreEqual("patient", customMetricItems[1].Context);
            Assert.IsNull(customMetricItems[1].Type.Enum);
            Assert.IsNotNull(customMetricItems[1].Type.Number);
            Assert.IsNull(customMetricItems[1].Type.String);

            // string
            Assert.AreEqual("5b98016854c00417f86e8d098ffc1e00", customMetricItems[2].Id);
            Assert.AreEqual("Planner Name", customMetricItems[2].Name);
            Assert.AreEqual("patient", customMetricItems[2].Context);
            Assert.IsNull(customMetricItems[2].Type.Enum);
            Assert.IsNull(customMetricItems[2].Type.Number);
            Assert.IsNotNull(customMetricItems[2].Type.String);
        }

        [TestMethod]
        public void WriteTest_Id()
        {
            var customMetricItem = new CustomMetricItem("5b98016854c00417f86e8d098ffc1e00", null, null, null);
            string expected = "{\"id\":\"5b98016854c00417f86e8d098ffc1e00\"}";
            var actual = JsonSerializer.Serialize<CustomMetricItem>(customMetricItem);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteTest_NameContextTypeObjectives()
        {
            var objectives = new List<MetricBin>()
            {
                new MetricBin("PASS", new byte[] { 18, 191, 0 }, null, 90),
                new MetricBin("FAIL", new byte[] { 255, 0, 0 })
            };
            var customMetricItem = new CustomMetricItem(null, "Normalcy of Diet @ 6 mo. (0-100)", "patient", new CustomMetricType("number"), objectives);
            string objectivesJson = "[{\"label\":\"PASS\",\"color\":[18,191,0],\"max\":90},{\"label\":\"FAIL\",\"color\":[255,0,0]}]";
            string expected = $"{{\"name\":\"Normalcy of Diet @ 6 mo. (0-100)\",\"context\":\"patient\",\"type\":{{\"number\":{{}}}},\"objectives\":{objectivesJson}}}";
            var actual = JsonSerializer.Serialize<CustomMetricItem>(customMetricItem);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteTest_AllTypes()
        {
            var customMetricItems = new List<CustomMetricItem>() {
                new CustomMetricItem(null, "Genetic-Type", "patient", new CustomMetricType("enum", new string[] { "TYPE I", "TYPE II", "TYPE III", "TYPE IV", "TYPE V" })),
                new CustomMetricItem(null, "Normalcy of Diet @ 6 mo. (0-100)", "patient", new CustomMetricType("number")),
                new CustomMetricItem(null, "Planner Name", "patient", new CustomMetricType("string"))
            };
            string json1 = "{\"name\":\"Genetic-Type\",\"context\":\"patient\",\"type\":{\"enum\":{\"values\":[\"TYPE I\",\"TYPE II\",\"TYPE III\",\"TYPE IV\",\"TYPE V\"]}}}";
            string json2 = "{\"name\":\"Normalcy of Diet @ 6 mo. (0-100)\",\"context\":\"patient\",\"type\":{\"number\":{}}}";
            string json3 = "{\"name\":\"Planner Name\",\"context\":\"patient\",\"type\":{\"string\":{}}}";
            string expected = $"[{json1},{json2},{json3}]";
            var actual = JsonSerializer.Serialize<List<CustomMetricItem>>(customMetricItems);
            Assert.AreEqual(expected, actual);
        }
    }
}
