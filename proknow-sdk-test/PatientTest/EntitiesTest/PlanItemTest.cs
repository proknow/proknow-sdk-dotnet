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
    public class PlanItemTest
    {
        private static string _patientMrnAndName = "PlanItemTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = new Uploads(_proKnow);
        private WorkspaceItem _workspaceItem;
        private string _uploadPath;
        private PatientItem _patientItem;
        private EntityItem _entityItem;

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

            // Upload test file
            _uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientMetadata { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceItem.Id, _uploadPath, overrides);

            // Wait until uploaded test file has processed
            while (true)
            {
                _patientItem = await patientSummary.GetAsync();
                var entitySummaries = _patientItem.FindEntities(t => t.Type == "plan");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    _entityItem = await entitySummaries[0].GetAsync();
                    break;
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
            // Download the entity
            string downloadFolder = Path.Combine(Path.GetTempPath(), _patientMrnAndName);
            string downloadPath = await _entityItem.Download(downloadFolder);

            // Compare it to the uploaded one
            Assert.IsTrue(TestHelper.FileEquals(_uploadPath, downloadPath));

            // Cleanup
            Directory.Delete(downloadFolder, true);
        }
    }
}
