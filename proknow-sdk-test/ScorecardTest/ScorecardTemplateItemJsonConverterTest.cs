using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.Json;

namespace ProKnow.Scorecard.Test
{
    [TestClass]
    public class ScorecardTemplateItemJsonConverterTest
    {
        [TestMethod]
        public void ReadTest_OnlyComputedMetrics()
        {
            string computedMetricObjectivesJson = "[{\"label\":\"IDEAL\",\"color\":[18,191,0],\"max\":85},{\"label\":\"UNACCEPTABLE\",\"color\":[255,0,0]}]";
            string computedMetricsJson = $"[{{\"type\":\"CUMULATIVE_METERSET\",\"roi_name\":null,\"arg_1\":null,\"arg_2\":null,\"objectives\":{computedMetricObjectivesJson}}}]";
            string json = $"{{\"id\":\"5e7363bacc00dfda9d5b8fba9ba56861\",\"name\":\"SDK Testing\",\"computed\":{computedMetricsJson},\"custom\":[]}}";
            var scorecardTemplateItem = JsonSerializer.Deserialize<ScorecardTemplateItem>(json);
            Assert.AreEqual("5e7363bacc00dfda9d5b8fba9ba56861", scorecardTemplateItem.Id);
            Assert.AreEqual("SDK Testing", scorecardTemplateItem.Name);
            Assert.AreEqual(1, scorecardTemplateItem.ComputedMetrics.Count);
            Assert.AreEqual("CUMULATIVE_METERSET", scorecardTemplateItem.ComputedMetrics[0].Type);
            Assert.IsNull(scorecardTemplateItem.ComputedMetrics[0].RoiName);
            Assert.IsNull(scorecardTemplateItem.ComputedMetrics[0].Arg1);
            Assert.IsNull(scorecardTemplateItem.ComputedMetrics[0].Arg2);
            Assert.AreEqual(2, scorecardTemplateItem.ComputedMetrics[0].Objectives.Count);
            Assert.AreEqual("IDEAL", scorecardTemplateItem.ComputedMetrics[0].Objectives[0].Label);
            Assert.AreEqual(18, scorecardTemplateItem.ComputedMetrics[0].Objectives[0].Color[0]);
            Assert.AreEqual(191, scorecardTemplateItem.ComputedMetrics[0].Objectives[0].Color[1]);
            Assert.AreEqual(0, scorecardTemplateItem.ComputedMetrics[0].Objectives[0].Color[2]);
            Assert.IsNull(scorecardTemplateItem.ComputedMetrics[0].Objectives[0].Min);
            Assert.AreEqual(85, scorecardTemplateItem.ComputedMetrics[0].Objectives[0].Max);
            Assert.AreEqual("UNACCEPTABLE", scorecardTemplateItem.ComputedMetrics[0].Objectives[1].Label);
            Assert.AreEqual(255, scorecardTemplateItem.ComputedMetrics[0].Objectives[1].Color[0]);
            Assert.AreEqual(0, scorecardTemplateItem.ComputedMetrics[0].Objectives[1].Color[1]);
            Assert.AreEqual(0, scorecardTemplateItem.ComputedMetrics[0].Objectives[1].Color[2]);
            Assert.IsNull(scorecardTemplateItem.ComputedMetrics[0].Objectives[1].Min);
            Assert.IsNull(scorecardTemplateItem.ComputedMetrics[0].Objectives[1].Max);
            Assert.AreEqual(0, scorecardTemplateItem.CustomMetrics.Count);
        }

        [TestMethod]
        public void ReadTest_OnlyCustomMetrics()
        {
            string customMetricObjectivesJson = "[{\"label\":\"IDEAL\",\"color\":[18,191,0],\"max\":90},{\"label\":\"UNACCEPTABLE\",\"color\":[255,0,0]}]";
            string customMetricsJson = $"[{{\"id\":\"5b9c00f90e40e073f43aec25ff9f851a\",\"objectives\":{customMetricObjectivesJson}}}]";
            string json = $"{{\"id\":\"5e7363bacc00dfda9d5b8fba9ba56861\",\"name\":\"SDK Testing\",\"computed\":[],\"custom\":{customMetricsJson}}}";
            var scorecardTemplateItem = JsonSerializer.Deserialize<ScorecardTemplateItem>(json);
            Assert.AreEqual("5e7363bacc00dfda9d5b8fba9ba56861", scorecardTemplateItem.Id);
            Assert.AreEqual("SDK Testing", scorecardTemplateItem.Name);
            Assert.AreEqual(0, scorecardTemplateItem.ComputedMetrics.Count);
            Assert.AreEqual(1, scorecardTemplateItem.CustomMetrics.Count);
            Assert.AreEqual("5b9c00f90e40e073f43aec25ff9f851a", scorecardTemplateItem.CustomMetrics[0].Id);
            Assert.IsNull(scorecardTemplateItem.CustomMetrics[0].Name);
            Assert.IsNull(scorecardTemplateItem.CustomMetrics[0].Context);
            Assert.IsNull(scorecardTemplateItem.CustomMetrics[0].Type);
            Assert.AreEqual(2, scorecardTemplateItem.CustomMetrics[0].Objectives.Count);
            Assert.AreEqual("IDEAL", scorecardTemplateItem.CustomMetrics[0].Objectives[0].Label);
            Assert.AreEqual(18, scorecardTemplateItem.CustomMetrics[0].Objectives[0].Color[0]);
            Assert.AreEqual(191, scorecardTemplateItem.CustomMetrics[0].Objectives[0].Color[1]);
            Assert.AreEqual(0, scorecardTemplateItem.CustomMetrics[0].Objectives[0].Color[2]);
            Assert.IsNull(scorecardTemplateItem.CustomMetrics[0].Objectives[0].Min);
            Assert.AreEqual(90, scorecardTemplateItem.CustomMetrics[0].Objectives[0].Max);
            Assert.AreEqual("UNACCEPTABLE", scorecardTemplateItem.CustomMetrics[0].Objectives[1].Label);
            Assert.AreEqual(255, scorecardTemplateItem.CustomMetrics[0].Objectives[1].Color[0]);
            Assert.AreEqual(0, scorecardTemplateItem.CustomMetrics[0].Objectives[1].Color[1]);
            Assert.AreEqual(0, scorecardTemplateItem.CustomMetrics[0].Objectives[1].Color[2]);
            Assert.IsNull(scorecardTemplateItem.CustomMetrics[0].Objectives[1].Min);
            Assert.IsNull(scorecardTemplateItem.CustomMetrics[0].Objectives[1].Max);
        }

        [TestMethod]
        public void ReadTest_ComputedAndCustomMetrics()
        {
            string computedMetricObjectivesJson = "[{\"label\":\"IDEAL\",\"color\":[18,191,0],\"max\":85},{\"label\":\"UNACCEPTABLE\",\"color\":[255,0,0]}]";
            string computedMetricsJson = $"[{{\"type\":\"CUMULATIVE_METERSET\",\"roi_name\":null,\"arg_1\":null,\"arg_2\":null,\"objectives\":{computedMetricObjectivesJson}}}]";
            string customMetricObjectivesJson = "[{\"label\":\"IDEAL\",\"color\":[18,191,0],\"max\":90},{\"label\":\"UNACCEPTABLE\",\"color\":[255,0,0]}]";
            string customMetricsJson = $"[{{\"id\":\"5b9c00f90e40e073f43aec25ff9f851a\",\"objectives\":{customMetricObjectivesJson}}}]";
            string json = $"{{\"id\":\"5e7363bacc00dfda9d5b8fba9ba56861\",\"name\":\"SDK Testing\",\"computed\":{computedMetricsJson},\"custom\":{customMetricsJson}}}";
            var scorecardTemplateItem = JsonSerializer.Deserialize<ScorecardTemplateItem>(json);
            Assert.AreEqual("5e7363bacc00dfda9d5b8fba9ba56861", scorecardTemplateItem.Id);
            Assert.AreEqual("SDK Testing", scorecardTemplateItem.Name);
            Assert.AreEqual(1, scorecardTemplateItem.ComputedMetrics.Count);
            Assert.AreEqual("CUMULATIVE_METERSET", scorecardTemplateItem.ComputedMetrics[0].Type);
            Assert.IsNull(scorecardTemplateItem.ComputedMetrics[0].RoiName);
            Assert.IsNull(scorecardTemplateItem.ComputedMetrics[0].Arg1);
            Assert.IsNull(scorecardTemplateItem.ComputedMetrics[0].Arg2);
            Assert.AreEqual(2, scorecardTemplateItem.ComputedMetrics[0].Objectives.Count);
            Assert.AreEqual("IDEAL", scorecardTemplateItem.ComputedMetrics[0].Objectives[0].Label);
            Assert.AreEqual(18, scorecardTemplateItem.ComputedMetrics[0].Objectives[0].Color[0]);
            Assert.AreEqual(191, scorecardTemplateItem.ComputedMetrics[0].Objectives[0].Color[1]);
            Assert.AreEqual(0, scorecardTemplateItem.ComputedMetrics[0].Objectives[0].Color[2]);
            Assert.IsNull(scorecardTemplateItem.ComputedMetrics[0].Objectives[0].Min);
            Assert.AreEqual(85, scorecardTemplateItem.ComputedMetrics[0].Objectives[0].Max);
            Assert.AreEqual("UNACCEPTABLE", scorecardTemplateItem.ComputedMetrics[0].Objectives[1].Label);
            Assert.AreEqual(255, scorecardTemplateItem.ComputedMetrics[0].Objectives[1].Color[0]);
            Assert.AreEqual(0, scorecardTemplateItem.ComputedMetrics[0].Objectives[1].Color[1]);
            Assert.AreEqual(0, scorecardTemplateItem.ComputedMetrics[0].Objectives[1].Color[2]);
            Assert.IsNull(scorecardTemplateItem.ComputedMetrics[0].Objectives[1].Min);
            Assert.IsNull(scorecardTemplateItem.ComputedMetrics[0].Objectives[1].Max);
            Assert.AreEqual(1, scorecardTemplateItem.CustomMetrics.Count);
            Assert.AreEqual("5b9c00f90e40e073f43aec25ff9f851a", scorecardTemplateItem.CustomMetrics[0].Id);
            Assert.IsNull(scorecardTemplateItem.CustomMetrics[0].Name);
            Assert.IsNull(scorecardTemplateItem.CustomMetrics[0].Context);
            Assert.IsNull(scorecardTemplateItem.CustomMetrics[0].Type);
            Assert.AreEqual(2, scorecardTemplateItem.CustomMetrics[0].Objectives.Count);
            Assert.AreEqual("IDEAL", scorecardTemplateItem.CustomMetrics[0].Objectives[0].Label);
            Assert.AreEqual(18, scorecardTemplateItem.CustomMetrics[0].Objectives[0].Color[0]);
            Assert.AreEqual(191, scorecardTemplateItem.CustomMetrics[0].Objectives[0].Color[1]);
            Assert.AreEqual(0, scorecardTemplateItem.CustomMetrics[0].Objectives[0].Color[2]);
            Assert.IsNull(scorecardTemplateItem.CustomMetrics[0].Objectives[0].Min);
            Assert.AreEqual(90, scorecardTemplateItem.CustomMetrics[0].Objectives[0].Max);
            Assert.AreEqual("UNACCEPTABLE", scorecardTemplateItem.CustomMetrics[0].Objectives[1].Label);
            Assert.AreEqual(255, scorecardTemplateItem.CustomMetrics[0].Objectives[1].Color[0]);
            Assert.AreEqual(0, scorecardTemplateItem.CustomMetrics[0].Objectives[1].Color[1]);
            Assert.AreEqual(0, scorecardTemplateItem.CustomMetrics[0].Objectives[1].Color[2]);
            Assert.IsNull(scorecardTemplateItem.CustomMetrics[0].Objectives[1].Min);
            Assert.IsNull(scorecardTemplateItem.CustomMetrics[0].Objectives[1].Max);
        }

        [TestMethod]
        public void WriteTest()
        {
            var computedMetricObjectives = new List<MetricBin>()
            {
                new MetricBin("IDEAL", new byte[] { 18, 191, 0 }, null, 85),
                new MetricBin("UNACCEPTABLE", new byte[] { 255, 0, 0 })
            };
            var computedMetric = new ComputedMetric("CUMULATIVE_METERSET", null, null, null, computedMetricObjectives);
            var computedMetrics = new List<ComputedMetric>() { computedMetric };
            var customMetricObjectives = new List<MetricBin>()
            {
                new MetricBin("IDEAL", new byte[] { 18, 191, 0 }, null, 90),
                new MetricBin("UNACCEPTABLE", new byte[] { 255, 0, 0 })
            };
            var customMetricItem = new CustomMetricItem() {
                Id = "5b9c00f90e40e073f43aec25ff9f851a",
                Objectives = customMetricObjectives
            };
            var customMetricItems = new List<CustomMetricItem>() { customMetricItem };
            var scorecardTemplateItem = new ScorecardTemplateItem(null, "SDK Testing", computedMetrics, customMetricItems);
            string computedMetricObjectivesJson = "[{\"label\":\"IDEAL\",\"color\":[18,191,0],\"max\":85},{\"label\":\"UNACCEPTABLE\",\"color\":[255,0,0]}]";
            string computedMetricsJson = $"[{{\"type\":\"CUMULATIVE_METERSET\",\"roi_name\":null,\"arg_1\":null,\"arg_2\":null,\"rx\":null,\"rx_scale\":null,\"objectives\":{computedMetricObjectivesJson}}}]";
            string customMetricObjectivesJson = "[{\"label\":\"IDEAL\",\"color\":[18,191,0],\"max\":90},{\"label\":\"UNACCEPTABLE\",\"color\":[255,0,0]}]";
            string customMetricsJson = $"[{{\"id\":\"5b9c00f90e40e073f43aec25ff9f851a\",\"objectives\":{customMetricObjectivesJson}}}]";
            string expected = $"{{\"name\":\"SDK Testing\",\"computed\":{computedMetricsJson},\"custom\":{customMetricsJson}}}";
            var actual = JsonSerializer.Serialize<ScorecardTemplateItem>(scorecardTemplateItem);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteTestWorkspaceAsync()
        {
            var computedMetricObjectives = new List<MetricBin>()
            {
                new MetricBin("IDEAL", new byte[] { 18, 191, 0 }, null, 85),
                new MetricBin("UNACCEPTABLE", new byte[] { 255, 0, 0 })
            };
            var computedMetric = new ComputedMetric("CUMULATIVE_METERSET", null, null, null, computedMetricObjectives);
            var computedMetrics = new List<ComputedMetric>() { computedMetric };
            var customMetricObjectives = new List<MetricBin>()
            {
                new MetricBin("IDEAL", new byte[] { 18, 191, 0 }, null, 90),
                new MetricBin("UNACCEPTABLE", new byte[] { 255, 0, 0 })
            };
            var customMetricItem = new CustomMetricItem()
            {
                Id = "5b9c00f90e40e073f43aec25ff9f851a",
                Objectives = customMetricObjectives
            };
            var customMetricItems = new List<CustomMetricItem>() { customMetricItem };
            var scorecardTemplateItem = new ScorecardTemplateItem(null, "SDK Testing", computedMetrics, customMetricItems, "Workspace Testing");
            string computedMetricObjectivesJson = "[{\"label\":\"IDEAL\",\"color\":[18,191,0],\"max\":85},{\"label\":\"UNACCEPTABLE\",\"color\":[255,0,0]}]";
            string computedMetricsJson = $"[{{\"type\":\"CUMULATIVE_METERSET\",\"roi_name\":null,\"arg_1\":null,\"arg_2\":null,\"rx\":null,\"rx_scale\":null,\"objectives\":{computedMetricObjectivesJson}}}]";
            string customMetricObjectivesJson = "[{\"label\":\"IDEAL\",\"color\":[18,191,0],\"max\":90},{\"label\":\"UNACCEPTABLE\",\"color\":[255,0,0]}]";
            string customMetricsJson = $"[{{\"id\":\"5b9c00f90e40e073f43aec25ff9f851a\",\"objectives\":{customMetricObjectivesJson}}}]";
            string expected = $"{{\"name\":\"SDK Testing\",\"workspace_id\":\"Workspace Testing\",\"computed\":{computedMetricsJson},\"custom\":{customMetricsJson}}}";
            var actual = JsonSerializer.Serialize<ScorecardTemplateItem>(scorecardTemplateItem);
            Assert.AreEqual(expected, actual);
        }
    }
}
