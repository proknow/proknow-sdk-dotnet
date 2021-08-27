using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Upload.Test
{
    [TestClass]
    public class UploadPatientSummaryTest
    {
        private static readonly string _testClassName = nameof(UploadPatientSummaryTest);
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
                Patient = new PatientOverridesSchema { Mrn = $"{testNumber}-Mrn", Name = $"{testNumber}-Name" }
            };
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath, overrides);
            var uploadProcessingResults = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
            var uploadBatch = new UploadBatch(_proKnow, workspaceItem.Id, uploadProcessingResults.Results);

            // Get the summary view of the patient in the upload response
            var uploadPatientSummary = uploadBatch.FindPatient(uploadPath);

            // Get the full representation of the patient
            var patientItem = await uploadPatientSummary.GetAsync();

            // Verify the contents
            Assert.AreEqual(workspaceItem.Id, patientItem.WorkspaceId);
            Assert.IsNotNull(patientItem.Id);
            Assert.AreEqual(overrides.Patient.Mrn, patientItem.Mrn);
            Assert.AreEqual(overrides.Patient.Name, patientItem.Name);
            Assert.AreEqual(1, patientItem.Studies.Count);
            Assert.AreEqual(workspaceItem.Id, patientItem.Studies[0].WorkspaceId);
            Assert.AreEqual(patientItem.Id, patientItem.Studies[0].PatientId);
            Assert.IsNotNull(patientItem.Studies[0].Id);
            Assert.AreEqual(1, patientItem.Studies[0].Entities.Count);
            Assert.AreEqual(0, patientItem.Studies[0].Sros.Count);
        }
    }
}
