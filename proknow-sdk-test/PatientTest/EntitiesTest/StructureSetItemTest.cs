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
    public class StructureSetItemTest
    {
        private static string _patientMrnAndName = "SDK-StructureSetItemTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = _proKnow.Uploads;
        private static WorkspaceItem _workspaceItem;
        private static string _uploadPath;
        private static PatientItem _patientItem;
        private static StructureSetItem _structureSetItem;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);

            // Create a test workspace
            _workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName);

            // Create a test patient
            var patientSummary = await TestHelper.CreatePatientAsync(_patientMrnAndName);

            // Upload test file
            _uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RS.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceItem.Id, _uploadPath, overrides);

            // Wait until uploaded test file has processed
            while (true)
            {
                _patientItem = await patientSummary.GetAsync();
                var entitySummaries = _patientItem.FindEntities(t => t.Type == "structure_set");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    _structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;
                    break;
                }
            }
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspace
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);
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
    }
}
