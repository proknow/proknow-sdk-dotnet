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
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static readonly string _testClassName = nameof(ScorecardTemplatesTest);

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
        public async Task CreateAsyncTest()
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
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", expectedComputedMetrics, expectedCustomMetrics, null);

            // Verify the returned scorecard template
            Assert.AreEqual($"{_testClassName}-{testNumber}", scorecardTemplateItem.Name);
            Assert.AreEqual(1, scorecardTemplateItem.ComputedMetrics.Count);
            var actualComputedMetric = scorecardTemplateItem.ComputedMetrics[0];
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
            Assert.AreEqual(1, scorecardTemplateItem.CustomMetrics.Count);
            var actualCustomMetricItem = scorecardTemplateItem.CustomMetrics[0];
            Assert.AreEqual(expectedCustomMetricItem.Id, actualCustomMetricItem.Id);
        }

        [TestMethod]
        public async Task CreateAsyncWorkspaceTest()
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
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", expectedComputedMetrics, expectedCustomMetrics, workspace.Id);

            // Verify the returned scorecard template
            Assert.AreEqual($"{_testClassName}-{testNumber}", scorecardTemplateItem.Name);
            Assert.AreEqual(1, scorecardTemplateItem.ComputedMetrics.Count);
            var actualComputedMetric = scorecardTemplateItem.ComputedMetrics[0];
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
            Assert.AreEqual(1, scorecardTemplateItem.CustomMetrics.Count);
            var actualCustomMetricItem = scorecardTemplateItem.CustomMetrics[0];
            Assert.AreEqual(expectedCustomMetricItem.Id, actualCustomMetricItem.Id);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            var testNumber = 3;

            // Create a scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics, null);

            // Delete the scorecard template
            await _proKnow.ScorecardTemplates.DeleteAsync(scorecardTemplateItem.Id);

            // Verify the scorecard template was deleted from a workspace
            var scorecardTemplateSummary = await _proKnow.ScorecardTemplates.FindAsync(t => t.Name == _testClassName);
            Assert.IsNull(scorecardTemplateSummary);
        }

        [TestMethod]
        public async Task DeleteAsyncWorkspaceTest()
        {
            var testNumber = 4;
            // Create a test workspace
            var workspace = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics, workspace.Id);

            // Delete the scorecard template
            await _proKnow.ScorecardTemplates.DeleteAsync(scorecardTemplateItem.Id);

            // Verify the scorecard template was deleted
            var scorecardTemplateSummary = await _proKnow.ScorecardTemplates.FindAsync(t => t.Name == _testClassName);
            Assert.IsNull(scorecardTemplateSummary);
        }

        [TestMethod]
        public async Task FindAsyncTest()
        {
            var testNumber = 5;

            // Create a scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics, null);

            // Find the created scorecard template
            var scorecardTemplateSummary = await _proKnow.ScorecardTemplates.FindAsync(t => t.Name == scorecardTemplateItem.Name);

            // Verify the returned scorecard template summary
            Assert.AreEqual(scorecardTemplateItem.Id, scorecardTemplateSummary.Id);
            Assert.AreEqual(scorecardTemplateItem.Name, scorecardTemplateSummary.Name);
        }

        [TestMethod]
        public async Task FindAsyncWorkspaceTest()
        {
            var testNumber = 6;
            // Create a test workspace
            var workspace = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics, workspace.Id);

            // Find the created scorecard template
            var scorecardTemplateSummary = await _proKnow.ScorecardTemplates.FindAsync(t => t.Name == scorecardTemplateItem.Name, workspace.Id);

            // Verify the returned scorecard template summary
            Assert.AreEqual(scorecardTemplateItem.Id, scorecardTemplateSummary.Id);
            Assert.AreEqual(scorecardTemplateItem.Name, scorecardTemplateSummary.Name);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            var testNumber = 7;

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
            var expectedScorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", expectedComputedMetrics, expectedCustomMetrics, null);

            // Get that scorecard template
            var actualScorecardTemplateItem = await _proKnow.ScorecardTemplates.GetAsync(expectedScorecardTemplateItem.Id);

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
        public async Task GetAsyncWorkspaceTest()
        {
            var testNumber = 8;
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

            // Get that scorecard template
            var actualScorecardTemplateItem = await _proKnow.ScorecardTemplates.GetAsync(expectedScorecardTemplateItem.Id);

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
        public async Task QueryAsyncTest()
        {
            var testNumber = 9;

            // Create a scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics);

            // Query for scorecard templates
            var scorecardTemplates = await _proKnow.ScorecardTemplates.QueryAsync();

            // Verify that a summary of the created scorecard template was returned
            var scorecardTemplateSummary = scorecardTemplates.First(t => t.Id == scorecardTemplateItem.Id);
            Assert.AreEqual(scorecardTemplateItem.Name, scorecardTemplateSummary.Name);
        }

        [TestMethod]
        public async Task QueryAsyncWorkspaceTest()
        {
            var testNumber = 10;
            // Create a test workspace
            var workspace = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics, workspace.Id);

            // Query for scorecard templates
            var scorecardTemplates = await _proKnow.ScorecardTemplates.QueryAsync(workspace.Id);

            // Verify that a summary of the created scorecard template was returned
            var scorecardTemplateSummary = scorecardTemplates.First(t => t.Id == scorecardTemplateItem.Id);
            Assert.AreEqual(scorecardTemplateItem.Name, scorecardTemplateSummary.Name);
        }

        [TestMethod]
        public async Task ResolveAsyncTest_Id()
        {
            var testNumber = 11;

            // Create a scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics, null);

            // Resolve that scorecard template by providing the ID
            var scorecardTemplateSummary = await _proKnow.ScorecardTemplates.ResolveAsync(scorecardTemplateItem.Id);

            // Verify the returned scorecard template summary
            Assert.AreEqual(scorecardTemplateItem.Id, scorecardTemplateSummary.Id);
            Assert.AreEqual(scorecardTemplateItem.Name, scorecardTemplateSummary.Name);
        }

        [TestMethod]
        public async Task ResolveAsyncWorkspaceTest_Id()
        {
            var testNumber = 12;
            // Create a test workspace
            var workspace = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);
            // Create a scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics, workspace.Name);

            // Resolve that scorecard template by providing the ID
            var scorecardTemplateSummary = await _proKnow.ScorecardTemplates.ResolveAsync(scorecardTemplateItem.Id, workspace.Name);

            // Verify the returned scorecard template summary
            Assert.AreEqual(scorecardTemplateItem.Id, scorecardTemplateSummary.Id);
            Assert.AreEqual(scorecardTemplateItem.Name, scorecardTemplateSummary.Name);
        }

        [TestMethod]
        public async Task ResolveAsyncTest_Name()
        {
            var testNumber = 13;

            // Create scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics, null);

            // Resolve that scorecard template by providing the name
            var scorecardTemplateSummary = await _proKnow.ScorecardTemplates.ResolveAsync(scorecardTemplateItem.Name);

            // Verify the returned scorecard template summary
            Assert.AreEqual(scorecardTemplateItem.Id, scorecardTemplateSummary.Id);
            Assert.AreEqual(scorecardTemplateItem.Name, scorecardTemplateSummary.Name);
        }

        [TestMethod]
        public async Task ResolveAsyncWorkspaceTest_Name()
        {
            var testNumber = 14;
            // Create a test workspace
            var workspace = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics, workspace.Name);

            // Resolve that scorecard template by providing the name
            var scorecardTemplateSummary = await _proKnow.ScorecardTemplates.ResolveAsync(scorecardTemplateItem.Name, workspace.Name);

            // Verify the returned scorecard template summary
            Assert.AreEqual(scorecardTemplateItem.Id, scorecardTemplateSummary.Id);
            Assert.AreEqual(scorecardTemplateItem.Name, scorecardTemplateSummary.Name);
        }

        [TestMethod]
        public async Task ResolveByIdAsyncTest()
        {
            var testNumber = 15;

            // Create a scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics);

            // Resolve that scorecard template by ID
            var scorecardTemplateSummary = await _proKnow.ScorecardTemplates.ResolveByIdAsync(scorecardTemplateItem.Id);

            // Verify the returned scorecard template summary
            Assert.AreEqual(scorecardTemplateItem.Id, scorecardTemplateSummary.Id);
            Assert.AreEqual(scorecardTemplateItem.Name, scorecardTemplateSummary.Name);
        }

        [TestMethod]
        public async Task ResolveByIdWorkspaceAsyncTest()
        {
            var testNumber = 16;
            // Create a test workspace
            var workspace = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics, workspace.Id);

            // Resolve that scorecard template by ID
            var scorecardTemplateSummary = await _proKnow.ScorecardTemplates.ResolveByIdAsync(scorecardTemplateItem.Id, workspace.Id);

            // Verify the returned scorecard template summary
            Assert.AreEqual(scorecardTemplateItem.Id, scorecardTemplateSummary.Id);
            Assert.AreEqual(scorecardTemplateItem.Name, scorecardTemplateSummary.Name);
        }
        [TestMethod]
        public async Task ResolveByNameAsyncTest()
        {
            var testNumber = 17;
          
            // Create a scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics);

            // Resolve that scorecard template by name
            var scorecardTemplateSummary = await _proKnow.ScorecardTemplates.ResolveByNameAsync(scorecardTemplateItem.Name);

            // Verify the returned scorecard template summary
            Assert.AreEqual(scorecardTemplateItem.Id, scorecardTemplateSummary.Id);
            Assert.AreEqual(scorecardTemplateItem.Name, scorecardTemplateSummary.Name);
        }

        [TestMethod]
        public async Task ResolveByNameWorkspaceAsyncTest()
        {
            var testNumber = 18;
            // Create a test workspace
            var workspace = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a scorecard template
            var computedMetrics = new List<ComputedMetric>();
            var customMetrics = new List<CustomMetric>();
            var scorecardTemplateItem = await _proKnow.ScorecardTemplates.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics, workspace.Name);

            // Resolve that scorecard template by name
            var scorecardTemplateSummary = await _proKnow.ScorecardTemplates.ResolveByNameAsync(scorecardTemplateItem.Name, workspace.Name);

            // Verify the returned scorecard template summary
            Assert.AreEqual(scorecardTemplateItem.Id, scorecardTemplateSummary.Id);
            Assert.AreEqual(scorecardTemplateItem.Name, scorecardTemplateSummary.Name);
        }
    }
}
