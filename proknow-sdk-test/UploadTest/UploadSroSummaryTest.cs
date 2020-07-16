﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Patient;
using ProKnow.Test;
using System.IO;
using System.Threading.Tasks;
using ProKnow.Patient.Registrations;

namespace ProKnow.Upload.Test
{
    [TestClass]
    public class UploadSroSummaryTest
    {
        private static readonly string _patientMrnAndName = "SDK-UploadSroSummaryTest";
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspacesAsync(_patientMrnAndName);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_patientMrnAndName);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);

            // Upload test data
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = $"{testNumber}-Mrn", Name = $"{testNumber}-Name" }
            };
            var uploadBatch = await _proKnow.Uploads.UploadAsync(workspaceItem.Id, uploadPath, overrides);

            // Get the summary views of the patient, entities, and SRO in the upload response
            var uploadPatientSummary = uploadBatch.FindPatient(Path.Combine(uploadPath, "reg.dcm"));
            var uploadCtEntitySummary = uploadBatch.FindEntity(Path.Combine(uploadPath, "ct.dcm"));
            var uploadMrEntitySummary = uploadBatch.FindEntity(Path.Combine(uploadPath, "mr.dcm"));
            var uploadSroSummary = uploadBatch.FindSro(Path.Combine(uploadPath, "reg.dcm"));

            // Get the full representation of the patient, entities, and SRO
            var ctImageSetItem = await uploadCtEntitySummary.GetAsync();
            var mrImageSetItem = await uploadMrEntitySummary.GetAsync();
            var sroItem = await uploadSroSummary.GetAsync() as SroItem;

            // Verify the contents
            Assert.AreEqual(workspaceItem.Id, sroItem.WorkspaceId);
            Assert.AreEqual(uploadPatientSummary.Id, sroItem.PatientId);
            Assert.AreEqual(ctImageSetItem.StudyId, sroItem.StudyId);
            Assert.IsNotNull(sroItem.Id);
            Assert.AreEqual("1.2.246.352.221.52738008096457865345287404867971417272", sroItem.Uid);
            Assert.AreEqual("2018-02-19 09:06:05", sroItem.Name);
            Assert.AreEqual("rigid", sroItem.Type);
            Assert.AreEqual(mrImageSetItem.FrameOfReferenceUid, sroItem.Source.FrameOfReferenceUid);
            Assert.AreEqual(mrImageSetItem.Id, sroItem.Source.ImageSetId);
            Assert.AreEqual(ctImageSetItem.FrameOfReferenceUid, sroItem.Target.FrameOfReferenceUid);
            Assert.AreEqual(ctImageSetItem.Id, sroItem.Target.ImageSetId);
        }
    }
}
