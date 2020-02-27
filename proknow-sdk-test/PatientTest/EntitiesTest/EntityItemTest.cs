using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

using ProKnow.Patient;
using ProKnow.Patient.Entities;
using ProKnow.Test;
using ProKnow.Upload;

namespace ProKnow.Patients.Entities.Test
{
    [TestClass]
    public class EntityItemTest
    {
        private static string _patientMrnAndName = "SDK-EntityItemTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = new Uploads(_proKnow);
        private static WorkspaceItem _workspaceItem;
        private static string _uploadPath;
        private static PatientSummary _patientSummary;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);

            // Create a test workspace
            _workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName);

            // Create a test patient
            _patientSummary = await TestHelper.CreatePatientAsync(_patientMrnAndName);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspace
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            // Upload test file
            _uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceItem.Id, _uploadPath, overrides);

            // Wait until uploaded test file has processed
            PlanItem planItem = null;
            while (true)
            {
                var patientItem = await _patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "plan");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    planItem = await entitySummaries[0].GetAsync() as PlanItem;
                    break;
                }
            }

            // Delete entity
            await planItem.DeleteAsync();

            // Verify it was deleted
            while (true)
            {
                var patientItem = await _patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "plan");
                if (entitySummaries.Count == 0)
                {
                    break;
                }
            }
        }
    }
}
