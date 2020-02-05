using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

using ProKnow.Patient;
using ProKnow.Patient.Entities;
using ProKnow.Test;
using ProKnow.Upload;

namespace ProKnow.Patients.Entities.Test
{
    [TestClass]
    public class ImageSetItemTest
    {
        private static string _patientMrnAndName = "ImageSetItemTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = new Uploads(_proKnow);
        private WorkspaceItem _workspaceItem;
        private string[] _uploadedFiles;
        private PatientItem _patientItem;
        private ImageSetItem _entityItem;

        [TestInitialize]
        public async Task TestInitialize()
        {
            // Get the test workspace
            _workspaceItem = await _proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);

            // Delete test patient, if necessary
            await TestHelper.DeletePatientAsync(_workspaceItem.Id, _patientMrnAndName);

            // Create test patient
            await _proKnow.Patients.CreateAsync(TestSettings.TestWorkspaceName, _patientMrnAndName, _patientMrnAndName);
            var patientSummary = await _proKnow.Patients.FindAsync(_workspaceItem.Id, t => t.Name == _patientMrnAndName);

            // Upload test files
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientMetadata { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            _uploadedFiles = Directory.GetFiles(uploadPath);
            foreach (var file in _uploadedFiles)
            {
                await _uploads.UploadFileAsync(_workspaceItem.Id, file, overrides);
            }

            // Wait until uploaded test files have processed
            while (true)
            {
                _patientItem = await patientSummary.GetAsync();
                var entitySummaries = _patientItem.FindEntities(t => t.Type == "image_set");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    _entityItem = await entitySummaries[0].GetAsync() as ImageSetItem;
                    if (_entityItem.Data.Images.Count == _uploadedFiles.Length)
                    {
                        break;
                    }
                }
            }
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            // Delete test patient
            await TestHelper.DeletePatientAsync(_workspaceItem.Id, _patientMrnAndName);
        }

        [TestMethod]
        public async Task DownloadAsyncTest()
        {
            // Download the image set
            string downloadFolder = Path.Combine(Path.GetTempPath(), _patientMrnAndName);
            string downloadPath = await _entityItem.Download(downloadFolder);
            var downloadedFiles = Directory.GetFiles(downloadPath);

            //todo--this comparison takes a long time!
            // Compare the files
            foreach (var uploadedFile in _uploadedFiles)
            {
                var doesMatch = false;
                foreach (var downloadedFile in downloadedFiles)
                {
                    if (TestHelper.FileEquals(uploadedFile, downloadedFile))
                    {
                        doesMatch = true;
                        break;
                    }
                }
                Assert.IsTrue(doesMatch);
            }

            // Cleanup
            Directory.Delete(downloadFolder, true);
        }
    }
}
