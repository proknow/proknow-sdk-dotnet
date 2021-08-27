using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ProKnow.Test;

namespace ProKnow.Upload.Test
{
    [TestClass]
    public class UploadBatchTest
    {
        private static readonly string _testClassName = nameof(UploadBatchTest);
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
        public async Task FindPatientTest()
        {
            var testNumber = 1;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload test data
            var mrUploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "mr.dcm");
            var sroUploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "reg.dcm");
            var uploadPaths = new List<String>() { mrUploadPath, sroUploadPath };
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientOverridesSchema { Mrn = $"{testNumber}-Mrn", Name = $"{testNumber}-Name" }
            };
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPaths, overrides);
            var uploadProcessingResults = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
            var uploadBatch = new UploadBatch(_proKnow, workspaceItem.Id, uploadProcessingResults.Results);

            // Get the summary view of the patient in the upload response
            var uploadPatientSummary = uploadBatch.FindPatient(mrUploadPath);

            // Verify the contents at the patient level
            Assert.AreEqual(workspaceItem.Id, uploadPatientSummary.WorkspaceId);
            Assert.IsNotNull(uploadPatientSummary.Id);
            Assert.AreEqual(overrides.Patient.Mrn, uploadPatientSummary.Mrn);
            Assert.AreEqual(overrides.Patient.Name, uploadPatientSummary.Name);
            Assert.AreEqual(1, uploadPatientSummary.Entities.Count);
            Assert.AreEqual(1, uploadPatientSummary.Sros.Count);

            // Verify the contents at the entity level
            var uploadEntitySummary = uploadPatientSummary.Entities[0];
            Assert.AreEqual(workspaceItem.Id, uploadEntitySummary.WorkspaceId);
            Assert.AreEqual(uploadPatientSummary.Id, uploadEntitySummary.PatientId);
            Assert.IsNotNull(uploadEntitySummary.Id);
            Assert.AreEqual("1.2.246.352.221.470938394496317011513892701464452657827", uploadEntitySummary.Uid);
            Assert.AreEqual("image_set", uploadEntitySummary.Type);
            Assert.AreEqual("MR", uploadEntitySummary.Modality);
            Assert.AreEqual("", uploadEntitySummary.Description);

            // Verify the contents at the SRO level
            var uploadSroSummary = uploadPatientSummary.Sros[0];
            Assert.AreEqual(workspaceItem.Id, uploadSroSummary.WorkspaceId);
            Assert.AreEqual(uploadPatientSummary.Id, uploadSroSummary.PatientId);
            Assert.IsNotNull(uploadSroSummary.StudyId);
            Assert.IsNotNull(uploadSroSummary.Id);
            Assert.AreEqual("1.2.246.352.221.52738008096457865345287404867971417272", uploadSroSummary.Uid);
        }

        [TestMethod]
        public async Task FindEntityTest()
        {
            var testNumber = 2;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload test data
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "mr.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientOverridesSchema { Mrn = $"{testNumber}-Mrn", Name = $"{testNumber}-Name" }
            };
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath, overrides);
            var uploadProcessingResults = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
            var uploadBatch = new UploadBatch(_proKnow, workspaceItem.Id, uploadProcessingResults.Results);

            // Get the summary views of the patient and an entity in the upload response
            var uploadPatientSummary = uploadBatch.FindPatient(uploadPath);
            var uploadEntitySummary = uploadBatch.FindEntity(uploadPath);

            // Verify the contents
            Assert.AreEqual(workspaceItem.Id, uploadEntitySummary.WorkspaceId);
            Assert.AreEqual(uploadPatientSummary.Id, uploadEntitySummary.PatientId);
            Assert.IsNotNull(uploadEntitySummary.Id);
            Assert.AreEqual("1.2.246.352.221.470938394496317011513892701464452657827", uploadEntitySummary.Uid);
            Assert.AreEqual("image_set", uploadEntitySummary.Type);
            Assert.AreEqual("MR", uploadEntitySummary.Modality);
            Assert.AreEqual("", uploadEntitySummary.Description);
        }

        [TestMethod]
        public async Task FindSroTest()
        {
            var testNumber = 3;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload test data
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "reg.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientOverridesSchema { Mrn = $"{testNumber}-Mrn", Name = $"{testNumber}-Name" }
            };
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath, overrides);
            var uploadProcessingResults = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
            var uploadBatch = new UploadBatch(_proKnow, workspaceItem.Id, uploadProcessingResults.Results);

            // Get the summary views of the patient and the SRO in the upload response
            var uploadPatientSummary = uploadBatch.FindPatient(uploadPath);
            var uploadSroSummary = uploadBatch.FindSro(uploadPath);

            // Verify the contents
            Assert.AreEqual(workspaceItem.Id, uploadSroSummary.WorkspaceId);
            Assert.AreEqual(uploadPatientSummary.Id, uploadSroSummary.PatientId);
            Assert.IsNotNull(uploadSroSummary.StudyId);
            Assert.IsNotNull(uploadSroSummary.Id);
            Assert.AreEqual("1.2.246.352.221.52738008096457865345287404867971417272", uploadSroSummary.Uid);
        }

        [TestMethod]
        public async Task GetStatusTest_Completed()
        {
            var testNumber = 4;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload test data
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "reg.dcm");
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath);
            var uploadProcessingResults = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
            var uploadBatch = new UploadBatch(_proKnow, workspaceItem.Id, uploadProcessingResults.Results);

            // Verify the status
            Assert.AreEqual("completed", uploadBatch.GetStatus(uploadPath));
        }

        [TestMethod]
        public async Task GetStatusTest_Pending_SingleUpload()
        {
            var testNumber = 5;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // In one upload, upload test data that has the same Patient ID, but different Patient's Name
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "PatientNameConflict");
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath);
            var uploadProcessingResults = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
            var uploadBatch = new UploadBatch(_proKnow, workspaceItem.Id, uploadProcessingResults.Results);

            // Verify that one of the two files shows a status of "pending" (conflict)
            var uploadPath1 = Path.Combine(TestSettings.TestDataRootDirectory, "PatientNameConflict", "CT.1.dcm");
            var uploadPath2 = Path.Combine(TestSettings.TestDataRootDirectory, "PatientNameConflict", "CT.2.dcm");
            Assert.IsTrue(uploadBatch.GetStatus(uploadPath1) == "pending" || uploadBatch.GetStatus(uploadPath2) == "pending");
        }

        [TestMethod]
        public async Task GetStatusTest_Pending_MultipleUploads()
        {
            var testNumber = 6;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // In two uploads, upload test data that has the same Patient ID, but different Patient's Name
            var uploadPath1 = Path.Combine(TestSettings.TestDataRootDirectory, "PatientNameConflict", "CT.1.dcm");
            var uploadResults1 = await _proKnow.Uploads.UploadAsync(workspaceItem.Id, uploadPath1);
            await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults1);
            var uploadPath2 = Path.Combine(TestSettings.TestDataRootDirectory, "PatientNameConflict", "CT.2.dcm");
            var uploadResults2 = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath2);
            var uploadProcessingResults2 = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults2);
            var uploadBatch2 = new UploadBatch(_proKnow, workspaceItem.Id, uploadProcessingResults2.Results);

            // Verify that the second file shows a status of "pending" (conflict)
            Assert.IsTrue(uploadBatch2.GetStatus(uploadPath2) == "pending");
        }

        [TestMethod]
        public async Task GetStatusTest_Failed()
        {
            var testNumber = 7;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload test data
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "RtImage", "RTIMAGE.dcm");
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath);
            var uploadProcessingResults = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
            var uploadBatch = new UploadBatch(_proKnow, workspaceItem.Id, uploadProcessingResults.Results);

            // Verify the status
            Assert.AreEqual("failed", uploadBatch.GetStatus(uploadPath));
        }

        [TestMethod]
        public async Task GetStatusTest_Duplicate()
        {
            var testNumber = 8;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload test data
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "reg.dcm");
            var uploadResults1 = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath);
            await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults1);

            // Upload the same data again
            var uploadResults2 = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath);
            var uploadProcessingResults2 = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults2);
            var uploadBatch2 = new UploadBatch(_proKnow, workspaceItem.Id, uploadProcessingResults2.Results);

            // Verify the status is still "completed"
            Assert.AreEqual("completed", uploadBatch2.GetStatus(uploadPath));
        }
    }
}
