using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using ProKnow.Upload;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class ImageSetItemTest
    {
        private static readonly string _patientMrnAndName = "SDK-ImageSetItemTest";
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static readonly Uploads _uploads = _proKnow.Uploads;
        private static string _workspaceId;
        private static string[] _uploadedFiles;
        private static ImageSetItem _imageSetItem;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
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
                    _imageSetItem = await entitySummaries[0].GetAsync() as ImageSetItem;
                    if (_imageSetItem.Data.Images.Count == _uploadedFiles.Length)
                    {
                        break;
                    }
                }
            }
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspace
            await _proKnow.Workspaces.DeleteAsync(_workspaceId);
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
    }
}
