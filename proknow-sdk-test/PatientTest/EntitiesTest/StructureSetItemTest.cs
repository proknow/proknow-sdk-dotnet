using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Patient;
using ProKnow.Patient.Entities;
using ProKnow.Scorecard;
using ProKnow.Test;
using ProKnow.Upload;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patients.Entities.Test
{
    [TestClass]
    public class StructureSetItemTest
    {
        private static string _patientMrnAndName = "SDK-StructureSetItemTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = _proKnow.Uploads;
        private static string _workspaceId;
        private static string _uploadPath;
        private static EntitySummary _entitySummary;
        private static StructureSetItem _structureSetItem;
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
            _uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RS.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceId, _uploadPath, overrides);

            // Wait until uploaded test file has processed
            while (true)
            {
                var patientItem = await patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "structure_set");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    _entitySummary = entitySummaries[0];
                    _structureSetItem = await _entitySummary.GetAsync() as StructureSetItem;
                    break;
                }
            }

            // Create custom metric for testing
            _customMetricItem = await _proKnow.CustomMetrics.CreateAsync(
                _patientMrnAndName, "structure_set", "number");
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
            string downloadPath = await _structureSetItem.DownloadAsync(downloadFolder);

            // Compare it to the uploaded one
            Assert.IsTrue(TestHelper.FileEquals(_uploadPath, downloadPath));

            // Cleanup
            Directory.Delete(downloadFolder, true);
        }

        [TestMethod]
        public async Task GetMetadataAsyncTest()
        {
            // Set test metadata
            _structureSetItem.Metadata.Add(_customMetricItem.Id, 3.141);

            // Get metadata
            var metadata = await _structureSetItem.GetMetadataAsync();

            // Verify metadata
            Assert.AreEqual(1, metadata.Keys.Count);
            Assert.AreEqual(3.141, metadata[_customMetricItem.Name]);

            // Cleanup
            _structureSetItem.Metadata.Clear();
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            // Set description and metadata
            _structureSetItem.Description = _patientMrnAndName;
            _structureSetItem.Metadata.Add(_customMetricItem.Id, 1);

            // Save entity changes
            await _structureSetItem.SaveAsync();

            // Refresh entity
            _structureSetItem = await _entitySummary.GetAsync() as StructureSetItem;

            // Verify changes were saved
            Assert.AreEqual(_patientMrnAndName, _structureSetItem.Description);
            Assert.AreEqual(1, _structureSetItem.Metadata.Keys.Count);
            Assert.AreEqual(1, _structureSetItem.Metadata[_customMetricItem.Id]);

            // Cleanup
            _structureSetItem.Description = _entitySummary.Description;
            _structureSetItem.Metadata.Clear();
        }

        [TestMethod]
        public async Task SetMetadataAsyncTest()
        {
            // Set metadata
            var metadata = new Dictionary<string, object>() { { _customMetricItem.Name, 2 } };
            await _structureSetItem.SetMetadataAsync(metadata);

            // Verify metadata was set
            Assert.AreEqual(1, _structureSetItem.Metadata.Keys.Count);
            Assert.AreEqual(2, _structureSetItem.Metadata[_customMetricItem.Id]);

            // Cleanup
            _structureSetItem.Metadata.Clear();
        }
    }
}
