using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ProKnow.Patient;
using ProKnow.Test;

namespace ProKnow.Upload.Test
{
    [TestClass]
    public class UploadBatchTest
    {
        private static readonly string _patientMrnAndName = "SDK-UploadBatchTest";
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspaces, if necessary
            await TestHelper.DeleteWorkspacesAsync(_patientMrnAndName);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_patientMrnAndName);
        }

        [TestMethod]
        public async Task FindPatientTest()
        {
            var testNumber = 1;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);

            // Upload test data
            var mrUploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "mr.dcm");
            var sroUploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "reg.dcm");
            var uploadPaths = new List<String>() { mrUploadPath, sroUploadPath };
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = $"{testNumber}-Mrn", Name = $"{testNumber}-Name" }
            };
            var uploadBatch = await _proKnow.Uploads.UploadAsync(workspaceItem.Id, uploadPaths, overrides);

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
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);

            // Upload test data
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "mr.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = $"{testNumber}-Mrn", Name = $"{testNumber}-Name" }
            };
            var uploadBatch = await _proKnow.Uploads.UploadAsync(workspaceItem.Id, uploadPath, overrides);

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
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);

            // Upload test data
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "reg.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = $"{testNumber}-Mrn", Name = $"{testNumber}-Name" }
            };
            var uploadBatch = await _proKnow.Uploads.UploadAsync(workspaceItem.Id, uploadPath, overrides);

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
    }
}
