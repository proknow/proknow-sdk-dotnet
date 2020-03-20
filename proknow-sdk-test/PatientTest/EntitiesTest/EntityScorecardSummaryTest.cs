using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Scorecard;
using ProKnow.Test;
using ProKnow.Upload;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class EntityScorecardSummaryTest
    {
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static string _patientMrnAndName = "SDK-EntityScorecardSummaryTest";
        private static string _workspaceId;
        private static EntitySummary _entitySummary;
        private static EntityScorecards _entityScorecards;
        private static ComputedMetric _computedMetric;
        private static CustomMetricItem _customMetricItem;
        private static List<ComputedMetric> _computedMetrics;
        private static List<CustomMetricItem> _customMetricItems;
        private static EntityScorecardItem _entityScorecardItem;

        [TestInitialize]
        public async Task ClassInitialize()
        {
            // Cleanup from previous test failure, if necessary
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);
            await TestHelper.DeleteCustomMetricsAsync(_patientMrnAndName);

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName);
            _workspaceId = workspaceItem.Id;

            // Create a test patient
            var patientSummary = await TestHelper.CreatePatientAsync(_patientMrnAndName);

            // Upload test file
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _proKnow.Uploads.UploadAsync(_workspaceId, uploadPath, overrides);

            // Wait until uploaded test file has processed
            while (true)
            {
                var patientItem = await patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "dose");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    _entitySummary = entitySummaries[0];
                    await _entitySummary.GetAsync();
                    break;
                }
            }

            // Create entity scorecards object
            _entityScorecards = new EntityScorecards(_proKnow, _workspaceId, _entitySummary.Id);

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
            _customMetricItem = await _proKnow.CustomMetrics.CreateAsync(_patientMrnAndName, "dose", "number");

            // Add objectives to custom metric
            var objectives = new List<MetricBin>()
            {
                new MetricBin("PASS", new byte[] { 18, 191, 0 }, null, 90),
                new MetricBin("FAIL", new byte[] { 255, 0, 0 })
            };
            _customMetricItem.Objectives = objectives;

            // Convert custom metric to schema expected by CreateAsync (name and objectives only)
            var customMetricItem = new CustomMetricItem(_patientMrnAndName, objectives);

            // Create entity scorecard for testing
            _computedMetrics = new List<ComputedMetric>() { _computedMetric };
            _customMetricItems = new List<CustomMetricItem>() { customMetricItem };
            _entityScorecardItem = await _entityScorecards.CreateAsync(_patientMrnAndName, _computedMetrics, _customMetricItems);
        }

        [TestCleanup]
        public async Task ClassCleanup()
        {
            // Delete test workspace
            await _proKnow.Workspaces.DeleteAsync(_workspaceId);

            // Delete custom metrics created for this test
            await TestHelper.DeleteCustomMetricsAsync(_patientMrnAndName);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            var entityScorecardSummary = await _entityScorecards.FindAsync(t => t.Id == _entityScorecardItem.Id);
            var entityScorecardItem = await entityScorecardSummary.GetAsync();
            Assert.AreEqual(_patientMrnAndName, entityScorecardItem.Name);
            Assert.AreEqual(1, entityScorecardItem.ComputedMetrics.Count);
            var computedMetric = entityScorecardItem.ComputedMetrics[0];
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
            Assert.AreEqual(1, entityScorecardItem.CustomMetrics.Count);
            var customMetricItem = entityScorecardItem.CustomMetrics[0];
            Assert.AreEqual(_customMetricItem.Id, customMetricItem.Id);
            Assert.AreEqual(_customMetricItem.Objectives.Count, customMetricItem.Objectives.Count);
            for (var i = 0; i < _customMetricItem.Objectives.Count; i++)
            {
                Assert.AreEqual(_customMetricItem.Objectives[i].Label, customMetricItem.Objectives[i].Label);
                Assert.AreEqual(_customMetricItem.Objectives[i].Color[0], customMetricItem.Objectives[i].Color[0]);
                Assert.AreEqual(_customMetricItem.Objectives[i].Color[1], customMetricItem.Objectives[i].Color[1]);
                Assert.AreEqual(_customMetricItem.Objectives[i].Color[2], customMetricItem.Objectives[i].Color[2]);
                Assert.AreEqual(_customMetricItem.Objectives[i].Min, customMetricItem.Objectives[i].Min);
                Assert.AreEqual(_customMetricItem.Objectives[i].Max, customMetricItem.Objectives[i].Max);
            }
        }
    }
}
