using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ProKnow.Patient;
using ProKnow.Patient.Entities;
using ProKnow.Test;
using ProKnow.Upload;

namespace ProKnow.Patients.Test
{
    [TestClass]
    public class PatientItemTest
    {
        private static string _patientMrnAndName = "SDK-PatientItemTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = _proKnow.Uploads;
        private static string _workspaceId;
        private static PatientItem _patientItem;

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
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceId, uploadPath, overrides);

            // Wait until uploaded test files have processed
            while (true)
            {
                _patientItem = await patientSummary.GetAsync();
                var entitySummaries = _patientItem.FindEntities(e => true);
                if (entitySummaries.Count() >= 4)
                {
                    var statuses = entitySummaries.Select(e => e.Status).Distinct();
                    if (statuses.Count() == 1 && statuses.First() == "completed")
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
        public async Task DeleteAsyncTest()
        {
            // Delete this patient
            await _patientItem.DeleteAsync();

            // Verify that the patient was deleted
            while (true)
            {
                var patientSummaries = await _proKnow.Patients.LookupAsync(_workspaceId, new List<string>() { _patientItem.Mrn });
                if (patientSummaries[0] == null)
                {
                    break;
                }
            }

            // Restore the patient for other tests

            // Upload test files
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            var uploadBatch = await _uploads.UploadAsync(_workspaceId, uploadPath, overrides);

            // Wait until uploaded test files have processed
            while (true)
            {
                _patientItem = await _proKnow.Patients.GetAsync(_workspaceId, uploadBatch.Patients.First().Id);
                var entitySummaries = _patientItem.FindEntities(e => true);
                if (entitySummaries.Count() >= 4)
                {
                    var statuses = entitySummaries.Select(e => e.Status).Distinct();
                    if (statuses.Count() == 1 && statuses.First() == "completed")
                    {
                        break;
                    }
                }
            }
        }

        [TestMethod]
        public void FindEntitiesTest_Predicate_1st_Level()
        {
           var imageSetEntities = _patientItem.FindEntities(e => e.Type == "image_set");
            Assert.AreEqual(imageSetEntities.Count, 1);
        }

        [TestMethod]
        public void FindEntitiesTest_Predicate_2nd_Level()
        {
            var structureSetEntities = _patientItem.FindEntities(e => e.Type == "structure_set");
            Assert.AreEqual(structureSetEntities.Count, 1);
        }

        [TestMethod]
        public void FindEntitiesTest_Predicate_3rd_Level()
        {
            var planEntities = _patientItem.FindEntities(e => e.Type == "plan");
            Assert.AreEqual(planEntities.Count, 1);
        }

        [TestMethod]
        public void FindEntitiesTest_Predicate_4th_Level()
        {
            var doseEntities = _patientItem.FindEntities(e => e.Type == "dose");
            Assert.AreEqual(doseEntities.Count, 1);
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
            var uploadBatch = await _patientItem.UploadAsync(uploadPath, overrides);

            // Wait until uploaded test file has processed
            while (true)
            {
                await _patientItem.RefreshAsync();
                var entitySummaries = _patientItem.FindEntities(t => t.Type == "plan");
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
            var uploadBatch = await _patientItem.UploadAsync(uploadPath, overrides);
            var uploadedFiles = Directory.GetFiles(uploadPath);

            // Wait until uploaded test file has processed
            while (true)
            {
                await _patientItem.RefreshAsync();
                var entitySummaries = _patientItem.FindEntities(t => t.Type == "image_set");
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
