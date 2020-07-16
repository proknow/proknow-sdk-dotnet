using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Patient.Entities;
using ProKnow.Test;
using ProKnow.Upload;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Test
{
    [TestClass]
    public class PatientSummaryTest
    {
        private static readonly string _patientMrnAndName = "SDK-PatientSummaryTest";
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static string _workspaceId;
        private static PatientSummary _patientSummary;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName);
            _workspaceId = workspaceItem.Id;

            // Create a test patient
            _patientSummary = await TestHelper.CreatePatientAsync(_patientMrnAndName);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspace
            await _proKnow.Workspaces.DeleteAsync(_workspaceId);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            // Get the patient item
            var patientItem = await _patientSummary.GetAsync();

            // Verify the returned patient item
            Assert.AreEqual(_patientSummary.Id, patientItem.Id);
            Assert.AreEqual(_patientSummary.Mrn, patientItem.Mrn);
            Assert.AreEqual(_patientSummary.Name, patientItem.Name);
            Assert.AreEqual(_patientSummary.BirthDate, patientItem.BirthDate);
            Assert.AreEqual(_patientSummary.Sex, patientItem.Sex);
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
        public async Task UploadAsyncTest_SingleFolder()
        {
            // Upload test folder
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            var uploadBatch = await _patientSummary.UploadAsync(uploadPath, overrides);

            // Wait until uploaded test files have processed
            var uploadedFiles = Directory.GetFiles(uploadPath);
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

        [TestMethod]
        public async Task UploadAsyncTest_MultipleFilesAndOrFolders()
        {
            // Upload test folder and file
            var uploadPath1 = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var uploadPath2 = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            var uploadPaths = new List<string>() { uploadPath1, uploadPath2 };
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            var uploadBatch = await _patientSummary.UploadAsync(uploadPaths, overrides);


            // Wait until uploaded CT files have processed
            var uploadedCtFiles = Directory.GetFiles(uploadPath1);
            PatientItem patientItem;
            while (true)
            {
                patientItem = await _patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "image_set");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    // Make sure the uploaded data is the same
                    var entityItem = await entitySummaries[0].GetAsync() as ImageSetItem;
                    if (entityItem.Data.Images.Count == uploadedCtFiles.Length)

                        // Cleanup (in case there are other tests using the same image set)
                        await entityItem.DeleteAsync();

                    break;
                }
            }

            // Wait until upload plan file has processed
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
    }
}
