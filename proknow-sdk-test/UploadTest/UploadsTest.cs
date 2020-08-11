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
        private static readonly string _testClassName = nameof(UploadsTest);
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
            await _proKnow.Uploads.UploadAsync(workspaceItem.Id, uploadPath, overrides);

            // Verify file was uploaded
            await patientItem.RefreshAsync();
            var entitySummaries = patientItem.FindEntities(t => true);
            Assert.AreEqual(1, entitySummaries.Count);
            Assert.AreEqual("2.16.840.1.114337.1.1.1535997926.0", entitySummaries[0].Uid);
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
            await _proKnow.Uploads.UploadAsync(workspaceItem.Id, uploadPath, overrides);

            // Verify files were uploaded
            await patientItem.RefreshAsync();
            var entitySummaries = patientItem.FindEntities(t => true);
            Assert.AreEqual(1, entitySummaries.Count);
            var entityItem = await entitySummaries[0].GetAsync() as ImageSetItem;
            Assert.AreEqual(5, entityItem.Data.Images.Count);
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
            await _proKnow.Uploads.UploadAsync(workspaceItem.Id, uploadPaths, overrides);

            // Verify test folder and test file were uploaded
            await patientItem.RefreshAsync();
            var entitySummaries = patientItem.FindEntities(t => true);
            Assert.AreEqual(2, entitySummaries.Count);
            var imageSetSummary = patientItem.FindEntities(t => t.Type == "image_set")[0];
            var entityItem = await imageSetSummary.GetAsync() as ImageSetItem;
            Assert.AreEqual(5, entityItem.Data.Images.Count);
        }
    }
}
