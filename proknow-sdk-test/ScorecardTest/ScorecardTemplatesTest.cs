using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ProKnow.CustomMetric;
using ProKnow.Test;

namespace ProKnow.Scorecard.Test
{
    [TestClass]
    public class ScorecardTemplatesTest
    {
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static string _baseName = "SDK-ScorecardTemplatesTest";
        private static ComputedMetric _computedMetric;
        private static CustomMetricItem _customMetricItem;
        private static ScorecardTemplateItem _scorecardTemplateItem;

        [TestInitialize]
        public async Task ClassInitialize()
        {
            // Cleanup from previous test failure, if necessary
            await ClassCleanup();

            // Create computed metric for testing
            _computedMetric = new ComputedMetric("VOLUME_PERCENT_DOSE_RANGE_ROI", "BODY", 0, 0.05);

            // Create custom metric for testing
            _customMetricItem = await _proKnow.CustomMetrics.CreateAsync(_baseName, "patient", "number");

            // Create scorecard template for testing
            var computedMetrics = new List<ComputedMetric>() { _computedMetric };
            var customMetricNames = new List<string>() { _customMetricItem.Name };

            //TODO--THE CUSTOM METRICS COME BACK AS IDS
            _scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync(_baseName, computedMetrics, customMetricNames);
        }

        [TestCleanup]
        public async Task ClassCleanup()
        {
            // Delete scorecard template and custom metric created for this test
            await TestHelper.DeleteScorecardTemplatesAsync(_baseName);
            await TestHelper.DeleteCustomMetricsAsync(_baseName);
        }

        [TestMethod]
        public void CreateAsyncTest()
        {
            // Verify creation of scorecard template in test initialization
            Assert.AreEqual(_baseName, _scorecardTemplateItem.Name);
            Assert.AreEqual(1, _scorecardTemplateItem.ComputedMetrics.Count);
            var computedMetric = _scorecardTemplateItem.ComputedMetrics[0];
            Assert.AreEqual(_computedMetric.Type, computedMetric.Type);
            Assert.AreEqual(_computedMetric.RoiName, computedMetric.RoiName);
            Assert.AreEqual(_computedMetric.Arg1, computedMetric.Arg1);
            Assert.AreEqual(_computedMetric.Arg2, computedMetric.Arg2);
            Assert.AreEqual(1, _scorecardTemplateItem.CustomMetrics.Count);
            var customMetricItem = _scorecardTemplateItem.CustomMetrics[0];
            Assert.AreEqual(_customMetricItem.Id, customMetricItem.Id);
        }
    }
}
