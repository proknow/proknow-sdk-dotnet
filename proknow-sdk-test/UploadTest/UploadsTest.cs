using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Patient;
using ProKnow.Patient.Entities;
using ProKnow.Test;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Upload.Test
{
    [TestClass]
    public class UploadsTest
    {
        private static string _testClassName = "SDK-UploadsTest";
        private static ProKnowApi _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = _proKnow.Uploads;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspaces, if necessary
            await TestHelper.DeleteWorkspacesAsync(_testClassName);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);
        }

        [TestMethod]
        public async Task UploadAsyncTest_FilePath()
        {
            int testNumber = 1;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Upload test file
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = patientItem.Mrn, Name = patientItem.Name }
            };
            var uploadBatch = await _uploads.UploadAsync(workspaceItem.Id, uploadPath, overrides);

            // Wait until uploaded test file has processed
            while (true)
            {
                await patientItem.RefreshAsync();
                var entitySummaries = patientItem.FindEntities(t => true);
                if (entitySummaries.Count == 1 && entitySummaries[0].Status == "completed")
                {
                    // Make sure the uploaded data is the same
                    Assert.AreEqual(entitySummaries[0].Uid, "2.16.840.1.114337.1.1.1535997926.0");

                    break;
                }
            }
        }

        [TestMethod]
        public async Task UploadAsyncTest_FolderPath()
        {
            int testNumber = 2;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Upload test folder
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = patientItem.Mrn, Name = patientItem.Name }
            };
            var uploadBatch = await _uploads.UploadAsync(workspaceItem.Id, uploadPath, overrides);

            // Wait until uploaded test files have processed
            while (true)
            {
                await patientItem.RefreshAsync();
                var entitySummaries = patientItem.FindEntities(t => true);
                if (entitySummaries.Count == 1 && entitySummaries[0].Status == "completed")
                {
                    // Make sure the uploaded data is the same
                    var entityItem = await entitySummaries[0].GetAsync() as ImageSetItem;
                    if (entityItem.Data.Images.Count == 5)
                    {
                        break;
                    }
                }
            }
        }

        [TestMethod]
        public async Task UploadAsyncTest_MultiplePaths()
        {
            int testNumber = 3;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient without data
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Upload test folder and test file
            var uploadPath1 = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var uploadPath2 = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RS.dcm");
            var uploadPaths = new List<string>() { uploadPath1, uploadPath2 };
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = patientItem.Mrn, Name = patientItem.Name }
            };
            var uploadBatch = await _uploads.UploadAsync(workspaceItem.Id, uploadPaths, overrides);

            // Verify test folder and test file were uploaded (may have to wait for files to be processed)
            while (true)
            {
                await patientItem.RefreshAsync();
                var entitySummaries = patientItem.FindEntities(t => true);
                if (entitySummaries.Count == 2 && entitySummaries[0].Status == "completed" && entitySummaries[1].Status == "completed")
                {
                    var imageSetSummary = patientItem.FindEntities(t => t.Type == "image_set")[0];
                    var entityItem = await imageSetSummary.GetAsync() as ImageSetItem;
                    if (entityItem.Data.Images.Count == 5)
                    {
                        break;
                    }
                }
            }
        }
    }
}
