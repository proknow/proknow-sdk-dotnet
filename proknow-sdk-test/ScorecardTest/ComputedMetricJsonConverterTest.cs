using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.Json;

namespace ProKnow.Scorecard.Test
{
    [TestClass]
    public class ComputedMetricJsonConverterTest
    {
        [TestMethod]
        public void ReadTest_Type()
        {
            string json = "{\"type\":\"CUMULATIVE_METERSET\",\"roi_name\":null,\"arg_1\":null,\"arg_2\":null}";
            var computedMetric = JsonSerializer.Deserialize<ComputedMetric>(json);
            Assert.AreEqual("CUMULATIVE_METERSET", computedMetric.Type);
            Assert.IsNull(computedMetric.RoiName);
            Assert.IsNull(computedMetric.Arg1);
            Assert.IsNull(computedMetric.Arg2);
            Assert.IsNull(computedMetric.Objectives);
        }

        [TestMethod]
        public void ReadTest_TypeAndRoiName()
        {
            string json = "{\"type\":\"MEAN_DOSE_ROI\",\"roi_name\":\"PTV\",\"arg_1\":null,\"arg_2\":null}";
            var computedMetric = JsonSerializer.Deserialize<ComputedMetric>(json);
            Assert.AreEqual("MEAN_DOSE_ROI", computedMetric.Type);
            Assert.AreEqual("PTV", computedMetric.RoiName);
            Assert.IsNull(computedMetric.Arg1);
            Assert.IsNull(computedMetric.Arg2);
            Assert.IsNull(computedMetric.Objectives);
        }

        [TestMethod]
        public void ReadTest_TypeRoiNameAndArg1()
        {
            string json = "{\"type\":\"DOSE_VOLUME_CC_ROI\",\"roi_name\":\"PTV\",\"arg_1\":3.14,\"arg_2\":null}";
            var computedMetric = JsonSerializer.Deserialize<ComputedMetric>(json);
            Assert.AreEqual("DOSE_VOLUME_CC_ROI", computedMetric.Type);
            Assert.AreEqual("PTV", computedMetric.RoiName);
            Assert.AreEqual(3.14, computedMetric.Arg1);
            Assert.IsNull(computedMetric.Arg2);
            Assert.IsNull(computedMetric.Objectives);
        }

        [TestMethod]
        public void ReadTest_TypeRoiNameArg1AndArg2()
        {
            string json = "{\"type\":\"DOSE_VOLUME_CC_ROI\",\"roi_name\":\"PTV\",\"arg_1\":1.23,\"arg_2\":3.14}";
            var computedMetric = JsonSerializer.Deserialize<ComputedMetric>(json);
            Assert.AreEqual("DOSE_VOLUME_CC_ROI", computedMetric.Type);
            Assert.AreEqual("PTV", computedMetric.RoiName);
            Assert.AreEqual(1.23, computedMetric.Arg1);
            Assert.AreEqual(3.14, computedMetric.Arg2);
            Assert.IsNull(computedMetric.Objectives);
        }

        [TestMethod]
        public void ReadTest_TypeRoiNameArg1Arg2AndObjectives()
        {
            string objectivesJson = "[{\"label\":\"PASS\",\"color\":[18,191,0],\"max\":1},{\"label\":\"FAIL\",\"color\":[255,0,0]}]";
            string json = $"{{\"type\":\"DOSE_VOLUME_CC_ROI\",\"roi_name\":\"PTV\",\"arg_1\":1.23,\"arg_2\":3.14,\"objectives\":{objectivesJson}}}";
            var computedMetric = JsonSerializer.Deserialize<ComputedMetric>(json);
            Assert.AreEqual("DOSE_VOLUME_CC_ROI", computedMetric.Type);
            Assert.AreEqual("PTV", computedMetric.RoiName);
            Assert.AreEqual(1.23, computedMetric.Arg1);
            Assert.AreEqual(3.14, computedMetric.Arg2);
            Assert.AreEqual(2, computedMetric.Objectives.Count);
            Assert.AreEqual("PASS", computedMetric.Objectives[0].Label);
            Assert.AreEqual(18, computedMetric.Objectives[0].Color[0]);
            Assert.AreEqual(191, computedMetric.Objectives[0].Color[1]);
            Assert.AreEqual(0, computedMetric.Objectives[0].Color[2]);
            Assert.IsNull(computedMetric.Objectives[0].Min);
            Assert.AreEqual(1, computedMetric.Objectives[0].Max);
            Assert.AreEqual("FAIL", computedMetric.Objectives[1].Label);
            Assert.AreEqual(255, computedMetric.Objectives[1].Color[0]);
            Assert.AreEqual(0, computedMetric.Objectives[1].Color[1]);
            Assert.AreEqual(0, computedMetric.Objectives[1].Color[2]);
            Assert.IsNull(computedMetric.Objectives[1].Min);
            Assert.IsNull(computedMetric.Objectives[1].Max);
        }

        [TestMethod]
        public void WriteTest_Type()
        {
            var computedMetric = new ComputedMetric("CUMULATIVE_METERSET");
            string expected = "{\"type\":\"CUMULATIVE_METERSET\",\"roi_name\":null,\"arg_1\":null,\"arg_2\":null}";
            var actual = JsonSerializer.Serialize<ComputedMetric>(computedMetric);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteTest_TypeAndRoiName()
        {
            var computedMetric = new ComputedMetric("MEAN_DOSE_ROI", "PTV");
            string expected = "{\"type\":\"MEAN_DOSE_ROI\",\"roi_name\":\"PTV\",\"arg_1\":null,\"arg_2\":null}";
            var actual = JsonSerializer.Serialize<ComputedMetric>(computedMetric);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteTest_TypeRoiNameAndArg1()
        {
            var computedMetric = new ComputedMetric("DOSE_VOLUME_CC_ROI", "PTV", 3.14);
            string expected = "{\"type\":\"DOSE_VOLUME_CC_ROI\",\"roi_name\":\"PTV\",\"arg_1\":3.14,\"arg_2\":null}";
            var actual = JsonSerializer.Serialize<ComputedMetric>(computedMetric);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteTest_TypeRoiNameArg1AndArg2()
        {
            var computedMetric = new ComputedMetric("DOSE_VOLUME_CC_ROI", "PTV", 1.23, 3.14);
            string expected = "{\"type\":\"DOSE_VOLUME_CC_ROI\",\"roi_name\":\"PTV\",\"arg_1\":1.23,\"arg_2\":3.14}";
            var actual = JsonSerializer.Serialize<ComputedMetric>(computedMetric);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteTest_TypeRoiNameArg1Arg2AndObjectives()
        {
            var objectives = new List<MetricBin>()
            {
                new MetricBin("PASS", new byte[] { 18, 191, 0 }, null, 1),
                new MetricBin("FAIL", new byte[] { 255, 0, 0 })
            };
            var computedMetric = new ComputedMetric("DOSE_VOLUME_CC_ROI", "PTV", 1.23, 3.14, objectives);
            string objectivesJson = "[{\"label\":\"PASS\",\"color\":[18,191,0],\"max\":1},{\"label\":\"FAIL\",\"color\":[255,0,0]}]";
            string expected = $"{{\"type\":\"DOSE_VOLUME_CC_ROI\",\"roi_name\":\"PTV\",\"arg_1\":1.23,\"arg_2\":3.14,\"objectives\":{objectivesJson}}}";
            var actual = JsonSerializer.Serialize<ComputedMetric>(computedMetric);
            Assert.AreEqual(expected, actual);
        }
    }
}
