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
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static readonly string _testClassName = nameof(ScorecardTemplateItemTest);

        [TestInitialize]
        public async Task ClassInitialize()
        {
            // Cleanup from previous test stoppage or failure, if necessary
            await ClassCleanup();
        }

        [TestCleanup]
        public async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);

            // Delete scorecard templates and custom metrics created for this test
            await TestHelper.DeleteScorecardTemplatesAsync(_testClassName);
            await TestHelper.DeleteCustomMetricsAsync(_testClassName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            var testNumber = 1;

            // Create scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics);

            // Delete the scorecard template
            await _proKnow.ScorecardTemplates.DeleteAsync(scorecardTemplateItem.Id);

            // Verify the scorecard template was deleted
            var scorecardTemplateSummary = await _proKnow.ScorecardTemplates.FindAsync(t => t.Name == scorecardTemplateItem.Name);
            Assert.IsNull(scorecardTemplateSummary);
        }

        [TestMethod]
        public async Task DeleteWorkspaceAsyncTest()
        {
            var testNumber = 2;
            // Create a test workspace
            var workspace = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics, workspace.Id);

            // Delete the scorecard template
            await _proKnow.ScorecardTemplates.DeleteAsync(scorecardTemplateItem.Id);

            // Verify the scorecard template was deleted
            var scorecardTemplateSummary = await _proKnow.ScorecardTemplates.FindAsync(t => t.Name == scorecardTemplateItem.Name);
            Assert.IsNull(scorecardTemplateSummary);
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            var testNumber = 3;

            // Create computed metric for testing
            var computedMetric = new ComputedMetric("VOLUME_PERCENT_DOSE_RANGE_ROI", "PTV", 30, 60,
                new List<MetricBin>() {
                    new MetricBin("IDEAL", new byte[] { Color.Green.R, Color.Green.G, Color.Green.B }),
                    new MetricBin("GOOD", new byte[] { Color.LightGreen.R, Color.LightGreen.G, Color.LightGreen.B }, 20),
                    new MetricBin("ACCEPTABLE", new byte[] { Color.Yellow.R, Color.Yellow.G, Color.Yellow.B }, 40, 60),
                    new MetricBin("MARGINAL", new byte[] { Color.Orange.R, Color.Orange.G, Color.Orange.B }, null, 80),
                    new MetricBin("UNACCEPTABLE", new byte[] { Color.Red.R, Color.Red.G, Color.Red.B })
                });

            // Create custom metric for testing
            var customMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}", "patient", "number");

            // Add objectives to custom metric
            customMetricItem.Objectives = new List<MetricBin>()
            {
                new MetricBin("PASS", new byte[] { 18, 191, 0 }, null, 90),
                new MetricBin("FAIL", new byte[] { 255, 0, 0 })
            };

            // Create scorecard template
            var computedMetrics = new List<ComputedMetric>() { computedMetric };
            var customMetrics = new List<CustomMetric>() { new CustomMetric(customMetricItem.Name, customMetricItem.Objectives) };
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync(_testClassName, computedMetrics, customMetrics);

            // Modify name
            scorecardTemplateItem.Name = $"{_testClassName}-{testNumber}-2";
            scorecardTemplateItem.Workspace = null;

            // Modify computed metrics
            var computedMetric2 = new ComputedMetric("MIN_DOSE_ROI", "BODY");
            scorecardTemplateItem.ComputedMetrics = new List<ComputedMetric>() { computedMetric2 };

            // Modify custom metrics
            var customMetricItem2 = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-2", "patient", "number");
            scorecardTemplateItem.CustomMetrics = new List<CustomMetricItem>() { customMetricItem2 };

            // Save changes
            await scorecardTemplateItem.SaveAsync();

            // Verify that the changes were saved
            var scorecardTemplateSummary2 = await _proKnow.ScorecardTemplates.FindAsync(t => t.Id == scorecardTemplateItem.Id);
            var scorecardTemplateItem2 = await scorecardTemplateSummary2.GetAsync();
            Assert.AreEqual($"{_testClassName}-{testNumber}-2", scorecardTemplateItem2.Name);
            Assert.AreEqual(1, scorecardTemplateItem2.ComputedMetrics.Count);
            Assert.AreEqual(computedMetric2.Type, scorecardTemplateItem2.ComputedMetrics[0].Type);
            Assert.AreEqual(computedMetric2.RoiName, scorecardTemplateItem2.ComputedMetrics[0].RoiName);
            Assert.AreEqual(computedMetric2.Arg1, scorecardTemplateItem2.ComputedMetrics[0].Arg1);
            Assert.AreEqual(computedMetric2.Arg2, scorecardTemplateItem2.ComputedMetrics[0].Arg2);
            Assert.AreEqual(1, scorecardTemplateItem2.CustomMetrics.Count);
            Assert.AreEqual(customMetricItem2.Id, scorecardTemplateItem2.CustomMetrics[0].Id);
        }

        [TestMethod]
        public async Task SaveWorkspaceAsyncTest()
        {
            var testNumber = 4;
            // Create a test workspace
            var workspace = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create computed metric for testing
            var computedMetric = new ComputedMetric("VOLUME_PERCENT_DOSE_RANGE_ROI", "PTV", 30, 60,
                new List<MetricBin>() {
                    new MetricBin("IDEAL", new byte[] { Color.Green.R, Color.Green.G, Color.Green.B }),
                    new MetricBin("GOOD", new byte[] { Color.LightGreen.R, Color.LightGreen.G, Color.LightGreen.B }, 20),
                    new MetricBin("ACCEPTABLE", new byte[] { Color.Yellow.R, Color.Yellow.G, Color.Yellow.B }, 40, 60),
                    new MetricBin("MARGINAL", new byte[] { Color.Orange.R, Color.Orange.G, Color.Orange.B }, null, 80),
                    new MetricBin("UNACCEPTABLE", new byte[] { Color.Red.R, Color.Red.G, Color.Red.B })
                });

            // Create custom metric for testing
            var customMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}", "patient", "number");

            // Add objectives to custom metric
            customMetricItem.Objectives = new List<MetricBin>()
            {
                new MetricBin("PASS", new byte[] { 18, 191, 0 }, null, 90),
                new MetricBin("FAIL", new byte[] { 255, 0, 0 })
            };

            // Create scorecard template
            var computedMetrics = new List<ComputedMetric>() { computedMetric };
            var customMetrics = new List<CustomMetric>() { new CustomMetric(customMetricItem.Name, customMetricItem.Objectives) };
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync(_testClassName, computedMetrics, customMetrics, workspace.Id);

            // Modify name
            scorecardTemplateItem.Name = $"{_testClassName}-{testNumber}-2";
            scorecardTemplateItem.Workspace = null;

            // Modify computed metrics
            var computedMetric2 = new ComputedMetric("MIN_DOSE_ROI", "BODY");
            scorecardTemplateItem.ComputedMetrics = new List<ComputedMetric>() { computedMetric2 };

            // Modify custom metrics
            var customMetricItem2 = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-2", "patient", "number");
            scorecardTemplateItem.CustomMetrics = new List<CustomMetricItem>() { customMetricItem2 };

            // Save changes
            await scorecardTemplateItem.SaveAsync();

            // Verify that the changes were saved
            var scorecardTemplateSummary2 = await _proKnow.ScorecardTemplates.FindAsync(t => t.Id == scorecardTemplateItem.Id, workspace.Name);
            var scorecardTemplateItem2 = await scorecardTemplateSummary2.GetAsync();
            Assert.AreEqual($"{_testClassName}-{testNumber}-2", scorecardTemplateItem2.Name);
            Assert.AreEqual(1, scorecardTemplateItem2.ComputedMetrics.Count);
            Assert.AreEqual(computedMetric2.Type, scorecardTemplateItem2.ComputedMetrics[0].Type);
            Assert.AreEqual(computedMetric2.RoiName, scorecardTemplateItem2.ComputedMetrics[0].RoiName);
            Assert.AreEqual(computedMetric2.Arg1, scorecardTemplateItem2.ComputedMetrics[0].Arg1);
            Assert.AreEqual(computedMetric2.Arg2, scorecardTemplateItem2.ComputedMetrics[0].Arg2);
            Assert.AreEqual(1, scorecardTemplateItem2.CustomMetrics.Count);
            Assert.AreEqual(customMetricItem2.Id, scorecardTemplateItem2.CustomMetrics[0].Id);
        }
    }
}
