using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using ProKnow.Upload;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class DoseItemTest
    {
        private static string _patientMrnAndName = "SDK-DoseItemTest";
        private static ProKnowApi _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = _proKnow.Uploads;
        private static string _workspaceId;
        private static string _uploadPath;
        private static DoseItem _doseItem;

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
            _uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceId, _uploadPath, overrides);

            // Wait until uploaded test file has processed
            while (true)
            {
                var patientItem = await patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "dose");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    _doseItem = await entitySummaries[0].GetAsync() as DoseItem;
                    break;
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
            // Download the entity
            string downloadFolder = Path.Combine(Path.GetTempPath(), _patientMrnAndName);
            string downloadPath = await _doseItem.DownloadAsync(downloadFolder);

            // Compare it to the uploaded one
            Assert.IsTrue(TestHelper.FileEquals(_uploadPath, downloadPath));

            // Cleanup
            Directory.Delete(downloadFolder, true);
        }
    }
}
