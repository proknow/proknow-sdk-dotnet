using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ProKnow.Scorecard.Test
{
    [TestClass]
    public class ScorecardTemplateSummaryTest
    {
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static readonly string _testClassName = nameof(ScorecardTemplateSummaryTest);

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

            // Delete scorecard template and custom metric created for this test
            await TestHelper.DeleteScorecardTemplatesAsync(_testClassName);
            await TestHelper.DeleteCustomMetricsAsync(_testClassName);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            var testNumber = 1;

            // Create computed metric for testing
            var expectedComputedMetric = new ComputedMetric("VOLUME_PERCENT_DOSE_RANGE_ROI", "PTV", 30, 60, null, null,
                new List<MetricBin>() {
                    new MetricBin("IDEAL", new byte[] { Color.Green.R, Color.Green.G, Color.Green.B }),
                    new MetricBin("GOOD", new byte[] { Color.LightGreen.R, Color.LightGreen.G, Color.LightGreen.B }, 20),
                    new MetricBin("ACCEPTABLE", new byte[] { Color.Yellow.R, Color.Yellow.G, Color.Yellow.B }, 40, 60),
                    new MetricBin("MARGINAL", new byte[] { Color.Orange.R, Color.Orange.G, Color.Orange.B }, null, 80),
                    new MetricBin("UNACCEPTABLE", new byte[] { Color.Red.R, Color.Red.G, Color.Red.B })
                });

            // Create custom metric for testing
            var expectedCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}", "patient", "number");

            // Add objectives to custom metric
            expectedCustomMetricItem.Objectives = new List<MetricBin>()
            {
                new MetricBin("PASS", new byte[] { 18, 191, 0 }, null, 90),
                new MetricBin("FAIL", new byte[] { 255, 0, 0 })
            };

            // Create a scorecard template
            var expectedComputedMetrics = new List<ComputedMetric>() { expectedComputedMetric };
            var expectedCustomMetrics = new List<CustomMetric>() { new CustomMetric(expectedCustomMetricItem.Name, expectedCustomMetricItem.Objectives) };
            var expectedScorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", expectedComputedMetrics, expectedCustomMetrics);
            var expectedScorecardTemplateSummary = await _proKnow.ScorecardTemplates.FindAsync(t => t.Id == expectedScorecardTemplateItem.Id);

            // Get that scorecard template from its summary
            var actualScorecardTemplateItem = await expectedScorecardTemplateSummary.GetAsync();

            // Verify the returned scorecard template
            Assert.AreEqual(expectedScorecardTemplateItem.Name, actualScorecardTemplateItem.Name);
            Assert.AreEqual(1, actualScorecardTemplateItem.ComputedMetrics.Count);
            var actualComputedMetric = actualScorecardTemplateItem.ComputedMetrics[0];
            Assert.AreEqual(expectedComputedMetric.Type, actualComputedMetric.Type);
            Assert.AreEqual(expectedComputedMetric.RoiName, actualComputedMetric.RoiName);
            Assert.AreEqual(expectedComputedMetric.Arg1, actualComputedMetric.Arg1);
            Assert.AreEqual(expectedComputedMetric.Arg2, actualComputedMetric.Arg2);
            Assert.AreEqual(expectedComputedMetric.Objectives.Count, actualComputedMetric.Objectives.Count);
            for (var i = 0; i < actualComputedMetric.Objectives.Count; i++)
            {
                Assert.AreEqual(expectedComputedMetric.Objectives[i].Label, actualComputedMetric.Objectives[i].Label);
                Assert.AreEqual(expectedComputedMetric.Objectives[i].Color[0], actualComputedMetric.Objectives[i].Color[0]);
                Assert.AreEqual(expectedComputedMetric.Objectives[i].Color[1], actualComputedMetric.Objectives[i].Color[1]);
                Assert.AreEqual(expectedComputedMetric.Objectives[i].Color[2], actualComputedMetric.Objectives[i].Color[2]);
                Assert.AreEqual(expectedComputedMetric.Objectives[i].Min, actualComputedMetric.Objectives[i].Min);
                Assert.AreEqual(expectedComputedMetric.Objectives[i].Max, actualComputedMetric.Objectives[i].Max);
            }
            Assert.AreEqual(1, actualScorecardTemplateItem.CustomMetrics.Count);
            var actualCustomMetricItem = expectedScorecardTemplateItem.CustomMetrics[0];
            Assert.AreEqual(expectedCustomMetricItem.Id, actualCustomMetricItem.Id);
        }

        [TestMethod]
        public async Task GetWorkspaceAsyncTest()
        {
            var testNumber = 2;
            // Create a test workspace
            var workspace = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create computed metric for testing
            var expectedComputedMetric = new ComputedMetric("VOLUME_PERCENT_DOSE_RANGE_ROI", "PTV", 30, 60, null, null,
                new List<MetricBin>() {
                    new MetricBin("IDEAL", new byte[] { Color.Green.R, Color.Green.G, Color.Green.B }),
                    new MetricBin("GOOD", new byte[] { Color.LightGreen.R, Color.LightGreen.G, Color.LightGreen.B }, 20),
                    new MetricBin("ACCEPTABLE", new byte[] { Color.Yellow.R, Color.Yellow.G, Color.Yellow.B }, 40, 60),
                    new MetricBin("MARGINAL", new byte[] { Color.Orange.R, Color.Orange.G, Color.Orange.B }, null, 80),
                    new MetricBin("UNACCEPTABLE", new byte[] { Color.Red.R, Color.Red.G, Color.Red.B })
                });

            // Create custom metric for testing
            var expectedCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}", "patient", "number");

            // Add objectives to custom metric
            expectedCustomMetricItem.Objectives = new List<MetricBin>()
            {
                new MetricBin("PASS", new byte[] { 18, 191, 0 }, null, 90),
                new MetricBin("FAIL", new byte[] { 255, 0, 0 })
            };

            // Create a scorecard template
            var expectedComputedMetrics = new List<ComputedMetric>() { expectedComputedMetric };
            var expectedCustomMetrics = new List<CustomMetric>() { new CustomMetric(expectedCustomMetricItem.Name, expectedCustomMetricItem.Objectives) };
            var expectedScorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", expectedComputedMetrics, expectedCustomMetrics,
                workspace.Name);
            var expectedScorecardTemplateSummary = await _proKnow.ScorecardTemplates.FindAsync(t => t.Id == expectedScorecardTemplateItem.Id, workspace.Name);

            // Get that scorecard template from its summary
            var actualScorecardTemplateItem = await expectedScorecardTemplateSummary.GetAsync();

            // Verify the returned scorecard template
            Assert.AreEqual(expectedScorecardTemplateItem.Name, actualScorecardTemplateItem.Name);
            Assert.AreEqual(1, actualScorecardTemplateItem.ComputedMetrics.Count);
            var actualComputedMetric = actualScorecardTemplateItem.ComputedMetrics[0];
            Assert.AreEqual(expectedComputedMetric.Type, actualComputedMetric.Type);
            Assert.AreEqual(expectedComputedMetric.RoiName, actualComputedMetric.RoiName);
            Assert.AreEqual(expectedComputedMetric.Arg1, actualComputedMetric.Arg1);
            Assert.AreEqual(expectedComputedMetric.Arg2, actualComputedMetric.Arg2);
            Assert.AreEqual(expectedComputedMetric.Objectives.Count, actualComputedMetric.Objectives.Count);
            for (var i = 0; i < actualComputedMetric.Objectives.Count; i++)
            {
                Assert.AreEqual(expectedComputedMetric.Objectives[i].Label, actualComputedMetric.Objectives[i].Label);
                Assert.AreEqual(expectedComputedMetric.Objectives[i].Color[0], actualComputedMetric.Objectives[i].Color[0]);
                Assert.AreEqual(expectedComputedMetric.Objectives[i].Color[1], actualComputedMetric.Objectives[i].Color[1]);
                Assert.AreEqual(expectedComputedMetric.Objectives[i].Color[2], actualComputedMetric.Objectives[i].Color[2]);
                Assert.AreEqual(expectedComputedMetric.Objectives[i].Min, actualComputedMetric.Objectives[i].Min);
                Assert.AreEqual(expectedComputedMetric.Objectives[i].Max, actualComputedMetric.Objectives[i].Max);
            }
            Assert.AreEqual(1, actualScorecardTemplateItem.CustomMetrics.Count);
            var actualCustomMetricItem = expectedScorecardTemplateItem.CustomMetrics[0];
            Assert.AreEqual(expectedCustomMetricItem.Id, actualCustomMetricItem.Id);
        }
    }
}
