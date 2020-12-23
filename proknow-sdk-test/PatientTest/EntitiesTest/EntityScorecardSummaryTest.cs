using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Scorecard;
using ProKnow.Test;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class EntityScorecardSummaryTest
    {
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static readonly string _testClassName = nameof(EntityScorecardSummaryTest);

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

            // Delete existing custom metrics
            await TestHelper.DeleteCustomMetricsAsync(_testClassName);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            var workspace = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a dose
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"));
            var entitySummary = patientItem.FindEntities(e => e.Type == "dose")[0];

            // Create entity scorecards object
            var entityScorecards = new EntityScorecards(_proKnow, workspace.Id, entitySummary.Id);

            // Create computed metric
            var computedMetric = new ComputedMetric("VOLUME_PERCENT_DOSE_RANGE_ROI", "PTV", 30, 60,
                new List<MetricBin>() {
                    new MetricBin("IDEAL", new byte[] { Color.Green.R, Color.Green.G, Color.Green.B }),
                    new MetricBin("GOOD", new byte[] { Color.LightGreen.R, Color.LightGreen.G, Color.LightGreen.B }, 20),
                    new MetricBin("ACCEPTABLE", new byte[] { Color.Yellow.R, Color.Yellow.G, Color.Yellow.B }, 40, 60),
                    new MetricBin("MARGINAL", new byte[] { Color.Orange.R, Color.Orange.G, Color.Orange.B }, null, 80),
                    new MetricBin("UNACCEPTABLE", new byte[] { Color.Red.R, Color.Red.G, Color.Red.B })
                });

            // Create custom metric
            var customMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}", "dose", "number");

            // Add objectives to custom metric
            customMetricItem.Objectives = new List<MetricBin>()
            {
                new MetricBin("PASS", new byte[] { 18, 191, 0 }, null, 90),
                new MetricBin("FAIL", new byte[] { 255, 0, 0 })
            };

            // Convert custom metric to schema expected by CreateAsync (name and objectives only)
            var customMetric = new CustomMetric(customMetricItem.Name, customMetricItem.Objectives);

            // Create an entity scorecard summary
            var computedMetrics = new List<ComputedMetric>() { computedMetric };
            var customMetrics = new List<CustomMetric>() { customMetric };
            var entityScorecardItem = await entityScorecards.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics);
            var entityScorecardSummary = await entityScorecards.FindAsync(t => t.Id == entityScorecardItem.Id);

            // Get the associated entity scorecard
            var createdEntityScorecardItem = await entityScorecardSummary.GetAsync();

            // Verify the contents of the returned entity scorecard
            Assert.AreEqual($"{_testClassName}-{testNumber}", createdEntityScorecardItem.Name);
            Assert.AreEqual(1, createdEntityScorecardItem.ComputedMetrics.Count);
            var createdComputedMetric = createdEntityScorecardItem.ComputedMetrics[0];
            Assert.AreEqual(computedMetric.Type, createdComputedMetric.Type);
            Assert.AreEqual(computedMetric.RoiName, createdComputedMetric.RoiName);
            Assert.AreEqual(computedMetric.Arg1, createdComputedMetric.Arg1);
            Assert.AreEqual(computedMetric.Arg2, createdComputedMetric.Arg2);
            Assert.AreEqual(computedMetric.Objectives.Count, createdComputedMetric.Objectives.Count);
            for (var i = 0; i < createdComputedMetric.Objectives.Count; i++)
            {
                Assert.AreEqual(computedMetric.Objectives[i].Label, createdComputedMetric.Objectives[i].Label);
                Assert.AreEqual(computedMetric.Objectives[i].Color[0], createdComputedMetric.Objectives[i].Color[0]);
                Assert.AreEqual(computedMetric.Objectives[i].Color[1], createdComputedMetric.Objectives[i].Color[1]);
                Assert.AreEqual(computedMetric.Objectives[i].Color[2], createdComputedMetric.Objectives[i].Color[2]);
                Assert.AreEqual(computedMetric.Objectives[i].Min, createdComputedMetric.Objectives[i].Min);
                Assert.AreEqual(computedMetric.Objectives[i].Max, createdComputedMetric.Objectives[i].Max);
            }
            Assert.AreEqual(1, createdEntityScorecardItem.CustomMetrics.Count);
            var createdCustomMetricItem = createdEntityScorecardItem.CustomMetrics[0];
            Assert.AreEqual(customMetricItem.Id, createdCustomMetricItem.Id);
            Assert.AreEqual(customMetricItem.Objectives.Count, createdCustomMetricItem.Objectives.Count);
            for (var i = 0; i < createdCustomMetricItem.Objectives.Count; i++)
            {
                Assert.AreEqual(customMetricItem.Objectives[i].Label, createdCustomMetricItem.Objectives[i].Label);
                Assert.AreEqual(customMetricItem.Objectives[i].Color[0], createdCustomMetricItem.Objectives[i].Color[0]);
                Assert.AreEqual(customMetricItem.Objectives[i].Color[1], createdCustomMetricItem.Objectives[i].Color[1]);
                Assert.AreEqual(customMetricItem.Objectives[i].Color[2], createdCustomMetricItem.Objectives[i].Color[2]);
                Assert.AreEqual(customMetricItem.Objectives[i].Min, createdCustomMetricItem.Objectives[i].Min);
                Assert.AreEqual(customMetricItem.Objectives[i].Max, createdCustomMetricItem.Objectives[i].Max);
            }
        }
    }
}
