using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

using ProKnow.Patient;
using ProKnow.Patient.Entities;
using ProKnow.Test;

namespace ProKnow.Upload.Test
{
    [TestClass]
    public class UploadsTest
    {
        private static string _patientMrnAndName = "UploadsTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = new Uploads(_proKnow);
        private WorkspaceItem _workspaceItem;
        private string _uploadPath;
        private PatientSummary _patientSummary;

        [TestInitialize]
        public async Task TestInitialize()
        {
            // Get the test workspace
            _workspaceItem = await _proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);

            // Delete test patient, if necessary
            await TestHelper.DeletePatientAsync(_workspaceItem.Id, _patientMrnAndName);

            // Create test patient
            await _proKnow.Patients.CreateAsync(TestSettings.TestWorkspaceName, _patientMrnAndName, _patientMrnAndName);
            _patientSummary = await _proKnow.Patients.FindAsync(_workspaceItem.Id, t => t.Name == _patientMrnAndName);
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            // Delete test patient
            await TestHelper.DeletePatientAsync(_workspaceItem.Id, _patientMrnAndName);
        }

        [TestMethod]
        public async Task UploadAsyncTest_SingleFile()
        {
            // Upload test file
            _uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientMetadata { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            var uploadBatch = await _uploads.UploadAsync(_workspaceItem.Id, _uploadPath, overrides);

            // Wait until uploaded test file has processed
            PatientItem patientItem;
            while (true)
            {
                patientItem = await _patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "plan");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    // Make sure the uploaded data is the same
                    Assert.AreEqual(entitySummaries[0].Uid, "2.16.840.1.114337.1.1.1535997926.0");

                    // Cleanup (in case there are other tests using the same patient)
                    //todo--delete the uploaded plan

                    break;
                }
            }
        }

        [TestMethod]
        public async Task UploadAsyncTest_Folder()
        {
            // Upload test folder
            _uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientMetadata { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            var uploadBatch = await _uploads.UploadAsync(_workspaceItem.Id, _uploadPath, overrides);
            var uploadedFiles = Directory.GetFiles(_uploadPath);

            // Wait until uploaded test file has processed
            PatientItem patientItem;
            while (true)
            {
                patientItem = await _patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "image_set");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    // Make sure the uploaded data is the same
                    var entityItem = await entitySummaries[0].GetAsync() as ImageSetItem;
                    if (entityItem.Data.Images.Count == uploadedFiles.Length)

                        // Cleanup (in case there are other tests using the same image set)
                        //todo--delete the uploaded image set

                        break;
                }
            }
        }
    }
}
