using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ProKnow.Scorecard.Test
{
    [TestClass]
    public class ScorecardTemplateItemTest
    {
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static string _baseName = "SDK-ScorecardTemplateItemTest";
        private static ComputedMetric _computedMetric;
        private static CustomMetricItem _customMetricItem;
        private static List<ComputedMetric> _computedMetrics;
        private static List<CustomMetricItem> _customMetricItems;
        private static ScorecardTemplateItem _scorecardTemplateItem;

        [TestInitialize]
        public async Task ClassInitialize()
        {
            // Cleanup from previous test failure, if necessary
            await ClassCleanup();

            // Create computed metric for testing
            _computedMetric = new ComputedMetric("VOLUME_PERCENT_DOSE_RANGE_ROI", "PTV", 30, 60,
                new List<MetricBin>() {
                    new MetricBin("IDEAL", new byte[] { Color.Green.R, Color.Green.G, Color.Green.B }),
                    new MetricBin("GOOD", new byte[] { Color.LightGreen.R, Color.LightGreen.G, Color.LightGreen.B }, 20),
                    new MetricBin("ACCEPTABLE", new byte[] { Color.Yellow.R, Color.Yellow.G, Color.Yellow.B }, 40, 60),
                    new MetricBin("MARGINAL", new byte[] { Color.Orange.R, Color.Orange.G, Color.Orange.B }, null, 80),
                    new MetricBin("UNACCEPTABLE", new byte[] { Color.Red.R, Color.Red.G, Color.Red.B })
                });

            // Create custom metric for testing
            _customMetricItem = await _proKnow.CustomMetrics.CreateAsync(_baseName, "patient", "number");

            // Create scorecard template for testing
            _computedMetrics = new List<ComputedMetric>() { _computedMetric };
            _customMetricItems = new List<CustomMetricItem>() { new CustomMetricItem(_customMetricItem.Name, _customMetricItem.Objectives) };
            _scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync(_baseName, _computedMetrics, _customMetricItems);
        }

        [TestCleanup]
        public async Task ClassCleanup()
        {
            // Delete scorecard template and custom metric created for this test
            await TestHelper.DeleteScorecardTemplatesAsync(_baseName);
            await TestHelper.DeleteCustomMetricsAsync(_baseName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            // Delete the scorecard template created in initialization
            await _proKnow.ScorecardTemplates.DeleteAsync(_scorecardTemplateItem.Id);

            // Verify the scorecard template was deleted
            var scorecardTemplate = await _proKnow.ScorecardTemplates.FindAsync(t => t.Name == _baseName);
            Assert.IsNull(scorecardTemplate);

            // Restore the deleted scorecard template
            _scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync(_baseName, _computedMetrics, _customMetricItems);
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            // Modify name
            _scorecardTemplateItem.Name = $"{_baseName}-2";

            // Modify computed metrics
            var computedMetric = new ComputedMetric("MIN_DOSE_ROI", "BODY");
            _scorecardTemplateItem.ComputedMetrics = new List<ComputedMetric>() { computedMetric };

            // Modify custom metrics
            var customMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_baseName}-2", "patient", "number");
            _scorecardTemplateItem.CustomMetrics = new List<CustomMetricItem>() { customMetricItem };

            // Save changes
            await _scorecardTemplateItem.SaveAsync();

            // Verify that the changes were saved
            var scorecardTemplate = await _proKnow.ScorecardTemplates.FindAsync(t => t.Id == _scorecardTemplateItem.Id);
            var scorecardTemplateItem = await scorecardTemplate.GetAsync();
            Assert.AreEqual($"{_baseName}-2", scorecardTemplateItem.Name);
            Assert.AreEqual(1, scorecardTemplateItem.ComputedMetrics.Count);
            Assert.AreEqual(computedMetric.Type, scorecardTemplateItem.ComputedMetrics[0].Type);
            Assert.AreEqual(computedMetric.RoiName, scorecardTemplateItem.ComputedMetrics[0].RoiName);
            Assert.AreEqual(computedMetric.Arg1, scorecardTemplateItem.ComputedMetrics[0].Arg1);
            Assert.AreEqual(computedMetric.Arg2, scorecardTemplateItem.ComputedMetrics[0].Arg2);
            Assert.AreEqual(1, scorecardTemplateItem.CustomMetrics.Count);
            Assert.AreEqual(customMetricItem.Id, scorecardTemplateItem.CustomMetrics[0].Id);

            // Restore the modified scorecard template
            _scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync(_baseName, _computedMetrics, _customMetricItems);
        }
    }
}
