using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using ProKnow.Upload;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class EntitySummaryTest
    {
        private static readonly string _patientMrnAndName = "SDK-EntitySummaryTest";
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static readonly Uploads _uploads = _proKnow.Uploads;
        private static string _workspaceId;
        private static PatientItem _patientItem;

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName);
            _workspaceId = workspaceItem.Id;

            // Create a test patient
            var patientSummary = await TestHelper.CreatePatientAsync(_patientMrnAndName);

            // Upload test files
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceId, uploadPath, overrides);

            // Wait until uploaded test files have processed
            while (true)
            {
                _patientItem = await patientSummary.GetAsync();
                var entitySummaries = _patientItem.FindEntities(e => e.PatientId == patientSummary.Id);
                if (entitySummaries.Count() >= 4)
                {
                    var statuses = entitySummaries.Select(e => e.Status).Distinct();
                    if (statuses.Count() == 1 && statuses.First() == "completed")
                    {
                        break;
                    }
                }
            }
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspace
            await _proKnow.Workspaces.DeleteAsync(_workspaceId);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            // Get existing entity summary
            var entitySummary = _patientItem.FindEntities(e => e.Type == "plan")[0];

            // Delete entity
            await entitySummary.DeleteAsync();

            // Verify it was deleted
            while (true)
            {
                await _patientItem.RefreshAsync();
                var entitySummaries = _patientItem.FindEntities(t => t.Type == "plan");
                if (entitySummaries.Count == 0)
                {
                    break;
                }
            }

            // Restore test file in case another test needs it
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceId, uploadPath, overrides);

            // Wait until uploaded test file has processed
            while (true)
            {
                await _patientItem.RefreshAsync();
                var entitySummaries = _patientItem.FindEntities(t => t.Type == "plan");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    break;
                }
            }
        }

        [TestMethod]
        public async Task ImageSet_GetAsyncTest()
        {
            var entitySummary = _patientItem.FindEntities(e => e.Type == "image_set")[0];
            var imageSetItem = await entitySummary.GetAsync();
            Assert.AreEqual(imageSetItem.WorkspaceId, _workspaceId);
            Assert.AreEqual(imageSetItem.PatientId, _patientItem.Id);
            Assert.AreEqual(imageSetItem.Id, entitySummary.Id);
        }

        [TestMethod]
        public async Task StructureSet_GetAsyncTest()
        {
            var entitySummary = _patientItem.FindEntities(e => e.Type == "structure_set")[0];
            var structureSetItem = await entitySummary.GetAsync();
            Assert.AreEqual(structureSetItem.WorkspaceId, _workspaceId);
            Assert.AreEqual(structureSetItem.PatientId, _patientItem.Id);
            Assert.AreEqual(structureSetItem.Id, entitySummary.Id);
        }

        [TestMethod]
        public async Task Plan_GetAsyncTest()
        {
            var entitySummary = _patientItem.FindEntities(e => e.Type == "plan")[0];
            var planItem = await entitySummary.GetAsync();
            Assert.AreEqual(planItem.WorkspaceId, _workspaceId);
            Assert.AreEqual(planItem.PatientId, _patientItem.Id);
            Assert.AreEqual(planItem.Id, entitySummary.Id);
        }

        [TestMethod]
        public async Task Dose_GetAsyncTest()
        {
            var entitySummary = _patientItem.FindEntities(e => e.Type == "dose")[0];
            var doseItem = await entitySummary.GetAsync();
            Assert.AreEqual(doseItem.WorkspaceId, _workspaceId);
            Assert.AreEqual(doseItem.PatientId, _patientItem.Id);
            Assert.AreEqual(doseItem.Id, entitySummary.Id);
        }
    }
}
