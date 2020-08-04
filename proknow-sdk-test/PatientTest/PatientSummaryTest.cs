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
        private static readonly string _testClassName = nameof(PatientSummaryTest);
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Cleanup from previous test stoppage or failure, if necessary
            await ClassCleanup();
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            int testNumber = 1;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);
            var patientSummary = await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Id == patientItem.Id);

            // Get the patient item
            var createdPatientItem = await patientSummary.GetAsync();

            // Verify the returned patient item
            Assert.AreEqual(patientSummary.Id, createdPatientItem.Id);
            Assert.AreEqual(patientSummary.Mrn, createdPatientItem.Mrn);
            Assert.AreEqual(patientSummary.Name, createdPatientItem.Name);
            Assert.AreEqual(patientSummary.BirthDate, createdPatientItem.BirthDate);
            Assert.AreEqual(patientSummary.Sex, createdPatientItem.Sex);
        }

        [TestMethod]
        public async Task UploadAsyncTest_SingleFile()
        {
            int testNumber = 2;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);
            var patientSummary = await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Id == patientItem.Id);

            // Upload test file
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = patientItem.Name, Mrn = patientItem.Mrn }
            };
            await patientSummary.UploadAsync(uploadPath, overrides);

            // Verify file was uploaded
            while (true)
            {
                await patientItem.RefreshAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "plan");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    // Make sure the uploaded data is the same
                    Assert.AreEqual("2.16.840.1.114337.1.1.1535997926.0", entitySummaries[0].Uid);

                    break;
                }
            }
        }

        [TestMethod]
        public async Task UploadAsyncTest_SingleFolder()
        {
            int testNumber = 3;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);
            var patientSummary = await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Id == patientItem.Id);

            // Upload test folder
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = patientItem.Name, Mrn = patientItem.Mrn }
            };
            await patientSummary.UploadAsync(uploadPath, overrides);

            // Very all files were uploaded
            var uploadedFiles = Directory.GetFiles(uploadPath);
            while (true)
            {
                await patientItem.RefreshAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "image_set");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    var entityItem = await entitySummaries[0].GetAsync() as ImageSetItem;
                    if (entityItem.Data.Images.Count == uploadedFiles.Length)
                    {
                        break;
                    }
                }
            }
        }

        [TestMethod]
        public async Task UploadAsyncTest_MultipleFilesAndOrFolders()
        {
            int testNumber = 4;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);
            var patientSummary = await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Id == patientItem.Id);

            // Upload test folder and file
            var uploadPath1 = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var uploadPath2 = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            var uploadPaths = new List<string>() { uploadPath1, uploadPath2 };
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = patientItem.Name, Mrn = patientItem.Mrn }
            };
            await patientSummary.UploadAsync(uploadPaths, overrides);

            // Verify all CT files were uploaded
            var uploadedCtFiles = Directory.GetFiles(uploadPath1);
            while (true)
            {
                await patientItem.RefreshAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "image_set");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    var entityItem = await entitySummaries[0].GetAsync() as ImageSetItem;
                    if (entityItem.Data.Images.Count == uploadedCtFiles.Length)
                    {
                        break;
                    }
                }
            }

            // Verify plan file was uploaded
            while (true)
            {
                await patientItem.RefreshAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "plan");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    // Make sure the uploaded data is the same
                    Assert.AreEqual("2.16.840.1.114337.1.1.1535997926.0", entitySummaries[0].Uid);

                    break;
                }
            }
        }
    }
}
