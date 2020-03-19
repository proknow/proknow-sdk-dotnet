using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Scorecard;
using ProKnow.Test;
using ProKnow.Upload;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class PlanItemTest
    {
        private static string _patientMrnAndName = "SDK-PlanItemTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = _proKnow.Uploads;
        private static string _workspaceId;
        private static string _uploadPath;
        private static EntitySummary _entitySummary;
        private static PlanItem _planItem;
        private static CustomMetricItem _customMetricItem;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete existing custom metrics, if necessary
            await TestHelper.DeleteCustomMetricsAsync(_patientMrnAndName);

            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName);
            _workspaceId = workspaceItem.Id;

            // Create a test patient
            var patientSummary = await TestHelper.CreatePatientAsync(_patientMrnAndName);

            // Upload test file
            _uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceId, _uploadPath, overrides);

            // Wait until uploaded test file has processed
            while (true)
            {
                var patientItem = await patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "plan");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    _entitySummary = entitySummaries[0];
                    _planItem = await _entitySummary.GetAsync() as PlanItem;
                    break;
                }
            }

            // Create custom metric for testing
            _customMetricItem = await _proKnow.CustomMetrics.CreateAsync(_patientMrnAndName, "plan", "string");
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspace
            await _proKnow.Workspaces.DeleteAsync(_workspaceId);

            // Delete custom metrics created for this test
            await TestHelper.DeleteCustomMetricsAsync(_patientMrnAndName);
        }

        [TestMethod]
        public async Task DownloadAsyncTest()
        {
            // Download the entity
            string downloadFolder = Path.Combine(Path.GetTempPath(), _patientMrnAndName);
            string downloadPath = await _planItem.DownloadAsync(downloadFolder);

            // Compare it to the uploaded one
            Assert.IsTrue(TestHelper.FileEquals(_uploadPath, downloadPath));

            // Cleanup
            Directory.Delete(downloadFolder, true);
        }

        [TestMethod]
        public async Task GetMetadataAsyncTest()
        {
            // Set test metadata
            _planItem.Metadata.Add(_customMetricItem.Id, "three");

            // Get metadata
            var metadata = await _planItem.GetMetadataAsync();

            // Verify metadata
            Assert.AreEqual(1, metadata.Keys.Count);
            Assert.AreEqual("three", metadata[_customMetricItem.Name]);

            // Cleanup
            _planItem.Metadata.Clear();
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            // Set description and metadata
            _planItem.Description = _patientMrnAndName;
            _planItem.Metadata.Add(_customMetricItem.Id, "one");

            // Save entity changes
            await _planItem.SaveAsync();

            // Refresh entity
            _planItem = await _entitySummary.GetAsync() as PlanItem;

            // Verify changes were saved
            Assert.AreEqual(_patientMrnAndName, _planItem.Description);
            Assert.AreEqual(1, _planItem.Metadata.Keys.Count);
            Assert.AreEqual("one", _planItem.Metadata[_customMetricItem.Id]);

            // Cleanup
            _planItem.Description = _entitySummary.Description;
            _planItem.Metadata.Clear();
        }

        [TestMethod]
        public async Task SetMetadataAsyncTest()
        {
            // Set metadata
            var metadata = new Dictionary<string, object>() { { _customMetricItem.Name, "two" } };
            await _planItem.SetMetadataAsync(metadata);

            // Verify metadata was set
            Assert.AreEqual(1, _planItem.Metadata.Keys.Count);
            Assert.AreEqual("two", _planItem.Metadata[_customMetricItem.Id]);

            // Cleanup
            _planItem.Metadata.Clear();
        }
    }
}
