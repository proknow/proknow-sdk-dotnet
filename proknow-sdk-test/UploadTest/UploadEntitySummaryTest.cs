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
        private static readonly string _testClassName = nameof(UploadEntitySummaryTest);
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
            var testNumber = 1;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload test data
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "ct.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = $"{testNumber}-Mrn", Name = $"{testNumber}-Name" }
            };
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath, overrides);
            var uploadProcessingResults = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
            var uploadBatch = new UploadBatch(_proKnow, workspaceItem.Id, uploadProcessingResults);

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
