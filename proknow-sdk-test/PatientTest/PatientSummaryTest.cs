using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

using ProKnow.Patient;
using ProKnow.Patient.Entities;
using ProKnow.Test;
using ProKnow.Upload;

namespace ProKnow.Patients.Test
{
    [TestClass]
    public class PatientSummaryTest
    {
        private static string _patientMrnAndName = "SDK-PatientSummaryTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static WorkspaceItem _workspaceItem;
        private static PatientSummary _patientSummary;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);

            // Create a test workspace
            _workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName);

            // Create a test patient
            _patientSummary = await TestHelper.CreatePatientAsync(_patientMrnAndName);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspace
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);
        }

        [TestMethod]
        public async Task UploadAsyncTest_SingleFile()
        {
            // Upload test file
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            var uploadBatch = await _patientSummary.UploadAsync(uploadPath, overrides);

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
                    await entitySummaries[0].DeleteAsync();

                    break;
                }
            }
        }

        [TestMethod]
        public async Task UploadAsyncTest_Folder()
        {
            // Upload test folder
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            var uploadBatch = await _patientSummary.UploadAsync(uploadPath, overrides);
            var uploadedFiles = Directory.GetFiles(uploadPath);

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
                        await entityItem.DeleteAsync();

                    break;
                }
            }
        }
    }
}
