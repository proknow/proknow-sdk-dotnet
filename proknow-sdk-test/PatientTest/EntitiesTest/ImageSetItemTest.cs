using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using ProKnow.CustomMetric;
using ProKnow.Patient;
using ProKnow.Patient.Entities;
using ProKnow.Test;
using ProKnow.Upload;

namespace ProKnow.Patients.Entities.Test
{
    [TestClass]
    public class ImageSetItemTest
    {
        private static string _patientMrnAndName = "SDK-ImageSetItemTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = _proKnow.Uploads;
        private static string _workspaceId;
        private static string[] _uploadedFiles;
        private static EntitySummary _entitySummary;
        private static ImageSetItem _imageSetItem;
        private static CustomMetricItem _customMetricItem;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete custom metrics, if necessary
            await TestHelper.DeleteCustomMetricAsync(_patientMrnAndName);

            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName);
            _workspaceId = workspaceItem.Id;

            // Create a test patient
            var patientSummary = await TestHelper.CreatePatientAsync(_patientMrnAndName);

            // Upload test files
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceId, uploadPath, overrides);
            _uploadedFiles = Directory.GetFiles(uploadPath);

            // Wait until uploaded test files have processed
            while (true)
            {
                var patientItem = await patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "image_set");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    _entitySummary = entitySummaries[0];
                    _imageSetItem = await _entitySummary.GetAsync() as ImageSetItem;
                    if (_imageSetItem.Data.Images.Count == _uploadedFiles.Length)
                    {
                        break;
                    }
                }
            }

            // Create custom metric for testing
            _customMetricItem = await _proKnow.CustomMetrics.CreateAsync(
                _patientMrnAndName, "image_set", "enum", new string[] { "one", "two", "three" });
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
            // Download the image set
            string downloadFolder = Path.Combine(Path.GetTempPath(), _patientMrnAndName);
            string downloadPath = await _imageSetItem.DownloadAsync(downloadFolder);
            var downloadedFiles = Directory.GetFiles(downloadPath);

            // Make sure the same number of images were downloaded
            Assert.AreEqual(_uploadedFiles.Length, downloadedFiles.Length);

            // Find one downloaded file that matches (don't need to check them all!)
            var doesMatch = false;
            foreach (var downloadedFile in downloadedFiles)
            {
                if (TestHelper.FileEquals(_uploadedFiles[0], downloadedFile))
                {
                    doesMatch = true;
                    break;
                }
            }
            Assert.IsTrue(doesMatch);

            // Cleanup
            Directory.Delete(downloadFolder, true);
        }

        [TestMethod]
        public async Task GetMetadataAsyncTest()
        {
            // Set test metadata
            _imageSetItem.Metadata.Add(_customMetricItem.Id, "three");

            // Get metadata
            var metadata = await _imageSetItem.GetMetadataAsync();

            // Verify metadata
            Assert.AreEqual(1, metadata.Keys.Count);
            Assert.AreEqual("three", metadata[_customMetricItem.Name]);

            // Cleanup
            _imageSetItem.Metadata.Clear();
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            // Set description and metadata
            _imageSetItem.Description = _patientMrnAndName;
            _imageSetItem.Metadata.Add(_customMetricItem.Id, "one");

            // Save entity changes
            await _imageSetItem.SaveAsync();

            // Refresh entity
            _imageSetItem = await _entitySummary.GetAsync() as ImageSetItem;

            // Verify changes were saved
            Assert.AreEqual(_patientMrnAndName, _imageSetItem.Description);
            Assert.AreEqual(1, _imageSetItem.Metadata.Keys.Count);
            Assert.AreEqual("one", _imageSetItem.Metadata[_customMetricItem.Id]);

            // Cleanup
            _imageSetItem.Description = _entitySummary.Description;
            _imageSetItem.Metadata.Clear();
        }

        [TestMethod]
        public async Task SetMetadataAsyncTest()
        {
            // Set metadata
            var metadata = new Dictionary<string, object>() { { _customMetricItem.Name, "two" } };
            await _imageSetItem.SetMetadataAsync(metadata);

            // Verify metadata was set
            Assert.AreEqual(1, _imageSetItem.Metadata.Keys.Count);
            Assert.AreEqual("two", _imageSetItem.Metadata[_customMetricItem.Id]);

            // Cleanup
            _imageSetItem.Metadata.Clear();
        }
    }
}
