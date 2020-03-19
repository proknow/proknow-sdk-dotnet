using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

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
            Assert.AreEqual(_computedMetric.Objectives.Count, computedMetric.Objectives.Count);
            for (var i = 0; i < _computedMetric.Objectives.Count; i++)
            {
                Assert.AreEqual(_computedMetric.Objectives[i].Label, computedMetric.Objectives[i].Label);
                Assert.AreEqual(_computedMetric.Objectives[i].Color[0], computedMetric.Objectives[i].Color[0]);
                Assert.AreEqual(_computedMetric.Objectives[i].Color[1], computedMetric.Objectives[i].Color[1]);
                Assert.AreEqual(_computedMetric.Objectives[i].Color[2], computedMetric.Objectives[i].Color[2]);
                Assert.AreEqual(_computedMetric.Objectives[i].Min, computedMetric.Objectives[i].Min);
                Assert.AreEqual(_computedMetric.Objectives[i].Max, computedMetric.Objectives[i].Max);
            }
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
            _scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync(_baseName, _computedMetrics, _customMetricItems);
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
            Assert.AreEqual(_computedMetric.Objectives.Count, computedMetric.Objectives.Count);
            for (var i = 0; i < _computedMetric.Objectives.Count; i++)
            {
                Assert.AreEqual(_computedMetric.Objectives[i].Label, computedMetric.Objectives[i].Label);
                Assert.AreEqual(_computedMetric.Objectives[i].Color[0], computedMetric.Objectives[i].Color[0]);
                Assert.AreEqual(_computedMetric.Objectives[i].Color[1], computedMetric.Objectives[i].Color[1]);
                Assert.AreEqual(_computedMetric.Objectives[i].Color[2], computedMetric.Objectives[i].Color[2]);
                Assert.AreEqual(_computedMetric.Objectives[i].Min, computedMetric.Objectives[i].Min);
                Assert.AreEqual(_computedMetric.Objectives[i].Max, computedMetric.Objectives[i].Max);
            }
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
