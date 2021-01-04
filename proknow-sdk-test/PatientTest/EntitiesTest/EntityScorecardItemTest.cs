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
    public class EntityScorecardItemTest
    {
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static readonly string _testClassName = nameof(EntityScorecardItemTest);

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
        public async Task DeleteAsyncTest()
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

            // Create entity scorecard
            var computedMetrics = new List<ComputedMetric>() { computedMetric };
            var customMetrics = new List<CustomMetric>() { customMetric };
            var entityScorecardItem = await entityScorecards.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics);

            // Delete the entity scorecard
            await entityScorecards.DeleteAsync(entityScorecardItem.Id);

            // Verify the entity scorecard was deleted
            var entityScorecardSummary = await entityScorecards.FindAsync(t => t.Name == $"{_testClassName}-{testNumber}");
            Assert.IsNull(entityScorecardSummary);
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            var testNumber = 2;

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

            // Create entity scorecard
            var computedMetrics = new List<ComputedMetric>() { computedMetric };
            var customMetrics = new List<CustomMetric>() { customMetric };
            var entityScorecardItem = await entityScorecards.CreateAsync($"{_testClassName}-{testNumber}", computedMetrics, customMetrics);

            // Modify name
            entityScorecardItem.Name = $"{_testClassName}-{testNumber}-2";

            // Modify computed metrics
            var computedMetric2 = new ComputedMetric("MIN_DOSE_ROI", "PTV", null, null,
                new List<MetricBin>() {
                    new MetricBin("BAD", new byte[] { 0, 255, 0 }),
                    new MetricBin("ACCEPTABLE", new byte[] { 0, 0, 0 }, 60, 63),
                    new MetricBin("GOOD", new byte[] { 255, 0, 0 })
                });
            entityScorecardItem.ComputedMetrics = new List<ComputedMetric>() { computedMetric2 };

            // Modify custom metrics
            var customMetricItem2 = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-2", "patient", "number");
            customMetricItem2.Objectives = new List<MetricBin>()
            {
                new MetricBin("PASS", new byte[] { 0, 255, 0 }, null, 85),
                new MetricBin("FAIL", new byte[] { 255, 0, 0 })
            };
            entityScorecardItem.CustomMetrics = new List<CustomMetricItem>() { customMetricItem2 };
            //TODO--Modify custom metric value and verify that it is saved

            // Save changes
            await entityScorecardItem.SaveAsync();

            // Verify that the changes were saved
            var entityScorecardSummary2 = await entityScorecards.FindAsync(t => t.Id == entityScorecardItem.Id);
            var entityScorecardItem2 = await entityScorecardSummary2.GetAsync();
            Assert.AreEqual($"{_testClassName}-{testNumber}-2", entityScorecardItem2.Name);
            Assert.AreEqual(1, entityScorecardItem2.ComputedMetrics.Count);
            Assert.AreEqual(computedMetric2.Type, entityScorecardItem2.ComputedMetrics[0].Type);
            Assert.AreEqual(computedMetric2.RoiName, entityScorecardItem2.ComputedMetrics[0].RoiName);
            Assert.AreEqual(computedMetric2.Arg1, entityScorecardItem2.ComputedMetrics[0].Arg1);
            Assert.AreEqual(computedMetric2.Arg2, entityScorecardItem2.ComputedMetrics[0].Arg2);
            Assert.AreEqual(computedMetric2.Objectives.Count, entityScorecardItem2.ComputedMetrics[0].Objectives.Count);
            for (int i = 0; i < computedMetric2.Objectives.Count; i++)
            {
                Assert.AreEqual(computedMetric2.Objectives[i].Label, entityScorecardItem2.ComputedMetrics[0].Objectives[i].Label);
                Assert.AreEqual(computedMetric2.Objectives[i].Color[0], entityScorecardItem2.ComputedMetrics[0].Objectives[i].Color[0]);
                Assert.AreEqual(computedMetric2.Objectives[i].Color[1], entityScorecardItem2.ComputedMetrics[0].Objectives[i].Color[1]);
                Assert.AreEqual(computedMetric2.Objectives[i].Color[2], entityScorecardItem2.ComputedMetrics[0].Objectives[i].Color[2]);
                Assert.AreEqual(computedMetric2.Objectives[i].Min, entityScorecardItem2.ComputedMetrics[0].Objectives[i].Min);
                Assert.AreEqual(computedMetric2.Objectives[i].Max, entityScorecardItem2.ComputedMetrics[0].Objectives[i].Max);
            }
            Assert.AreEqual(1, entityScorecardItem2.CustomMetrics.Count);
            Assert.AreEqual(customMetricItem2.Id, entityScorecardItem2.CustomMetrics[0].Id);
            Assert.AreEqual(customMetricItem2.Objectives.Count, entityScorecardItem2.CustomMetrics[0].Objectives.Count);
            for (int i = 0; i < customMetricItem2.Objectives.Count; i++)
            {
                Assert.AreEqual(customMetricItem2.Objectives[i].Label, entityScorecardItem2.CustomMetrics[0].Objectives[i].Label);
                Assert.AreEqual(customMetricItem2.Objectives[i].Color[0], entityScorecardItem2.CustomMetrics[0].Objectives[i].Color[0]);
                Assert.AreEqual(customMetricItem2.Objectives[i].Color[1], entityScorecardItem2.CustomMetrics[0].Objectives[i].Color[1]);
                Assert.AreEqual(customMetricItem2.Objectives[i].Color[2], entityScorecardItem2.CustomMetrics[0].Objectives[i].Color[2]);
                Assert.AreEqual(customMetricItem2.Objectives[i].Min, entityScorecardItem2.CustomMetrics[0].Objectives[i].Min);
                Assert.AreEqual(customMetricItem2.Objectives[i].Max, entityScorecardItem2.CustomMetrics[0].Objectives[i].Max);
            }
        }
    }
}
