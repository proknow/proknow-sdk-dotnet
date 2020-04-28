using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Patient;
using ProKnow.Patient.Entities;
using ProKnow.Test;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Upload.Test
{
    [TestClass]
    public class UploadEntitySummaryTest
    {
        private static string _patientMrnAndName = "SDK-UploadEntitySummaryTest";
        private static ProKnowApi _proKnow = TestSettings.ProKnow;

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
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "ct.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = $"{testNumber}-Mrn", Name = $"{testNumber}-Name" }
            };
            var uploadBatch = await _proKnow.Uploads.UploadAsync(workspaceItem.Id, uploadPath, overrides);

            // Get the summary views of the patient and entity in the upload response
            var uploadPatientSummary = uploadBatch.FindPatient(uploadPath);
            var uploadEntitySummary = uploadBatch.FindEntity(uploadPath);

            // Get the full representation of the entity
            var imageSetItem = await uploadEntitySummary.GetAsync() as ImageSetItem;

            // Verify the contents
            Assert.AreEqual(workspaceItem.Id, imageSetItem.WorkspaceId);
            Assert.AreEqual(uploadPatientSummary.Id, imageSetItem.PatientId);
            Assert.AreEqual("1.2.246.352.221.5093062159960566210763150553104377477", imageSetItem.FrameOfReferenceUid);
            Assert.IsNotNull(imageSetItem.Id);
            Assert.AreEqual("image_set", imageSetItem.Type);
            Assert.AreEqual("1.2.246.352.221.563569281719761951014104635106765053066", imageSetItem.Uid);
            Assert.AreEqual("CT", imageSetItem.Modality);
            Assert.AreEqual("", imageSetItem.Description);
            Assert.IsTrue(imageSetItem.Metadata.Count == 0);
            Assert.AreEqual("completed", imageSetItem.Status);
        }
    }
}
