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
        private static List<ComputedMetric> _computedMetrics;
        private static List<string> _customMetricNames;
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
            _computedMetrics = new List<ComputedMetric>() { _computedMetric };
            _customMetricNames = new List<string>() { _customMetricItem.Name };
            _scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync(_baseName, _computedMetrics, _customMetricNames);
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

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            // Delete the scorecard template created in initialization
            await _proKnow.ScorecardTemplates.DeleteAsync(_scorecardTemplateItem.Id);

            // Verify the scorecard template was deleted
            var scorecardTemplate = await _proKnow.ScorecardTemplates.FindAsync(t => t.Name == _baseName);
            Assert.IsNull(scorecardTemplate);

            // Restore the deleted scorecard template
            _scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync(_baseName, _computedMetrics, _customMetricNames);
        }

        [TestMethod]
        public async Task FindAsyncTest()
        {
            var scorecardTemplate = await _proKnow.ScorecardTemplates.FindAsync(t => t.Name == _baseName);
            Assert.AreEqual(_scorecardTemplateItem.Id, scorecardTemplate.Id);
            Assert.AreEqual(_scorecardTemplateItem.Name, scorecardTemplate.Name);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.GetAsync(_scorecardTemplateItem.Id);
            Assert.AreEqual(_baseName, scorecardTemplateItem.Name);
            Assert.AreEqual(1, scorecardTemplateItem.ComputedMetrics.Count);
            var computedMetric = scorecardTemplateItem.ComputedMetrics[0];
            Assert.AreEqual(_computedMetric.Type, computedMetric.Type);
            Assert.AreEqual(_computedMetric.RoiName, computedMetric.RoiName);
            Assert.AreEqual(_computedMetric.Arg1, computedMetric.Arg1);
            Assert.AreEqual(_computedMetric.Arg2, computedMetric.Arg2);
            Assert.AreEqual(1, scorecardTemplateItem.CustomMetrics.Count);
            var customMetricItem = scorecardTemplateItem.CustomMetrics[0];
            Assert.AreEqual(_customMetricItem.Id, customMetricItem.Id);
        }

        [TestMethod]
        public async Task ResolveAsyncTest_Id()
        {
            var scorecardTemplate = await _proKnow.ScorecardTemplates.ResolveAsync(_scorecardTemplateItem.Id);
            Assert.AreEqual(_scorecardTemplateItem.Id, scorecardTemplate.Id);
            Assert.AreEqual(_scorecardTemplateItem.Name, scorecardTemplate.Name);
        }

        [TestMethod]
        public async Task ResolveAsyncTest_Name()
        {
            var scorecardTemplate = await _proKnow.ScorecardTemplates.ResolveAsync(_scorecardTemplateItem.Name);
            Assert.AreEqual(_scorecardTemplateItem.Id, scorecardTemplate.Id);
            Assert.AreEqual(_scorecardTemplateItem.Name, scorecardTemplate.Name);
        }

        [TestMethod]
        public async Task ResolveByIdAsyncTest()
        {
            var scorecardTemplate = await _proKnow.ScorecardTemplates.ResolveByIdAsync(_scorecardTemplateItem.Id);
            Assert.AreEqual(_scorecardTemplateItem.Id, scorecardTemplate.Id);
            Assert.AreEqual(_scorecardTemplateItem.Name, scorecardTemplate.Name);
        }

        [TestMethod]
        public async Task ResolveByNameAsyncTest()
        {
            var scorecardTemplate = await _proKnow.ScorecardTemplates.ResolveByNameAsync(_scorecardTemplateItem.Name);
            Assert.AreEqual(_scorecardTemplateItem.Id, scorecardTemplate.Id);
            Assert.AreEqual(_scorecardTemplateItem.Name, scorecardTemplate.Name);
        }

        [TestMethod]
        public async Task QueryAsyncTest()
        {
            var scorecardTemplates = await _proKnow.ScorecardTemplates.QueryAsync();
            var scorecardTemplate = scorecardTemplates.First(t => t.Id == _scorecardTemplateItem.Id);
            Assert.AreEqual(_scorecardTemplateItem.Name, scorecardTemplate.Name);
        }
    }
}
