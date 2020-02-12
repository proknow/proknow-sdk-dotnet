using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ProKnow.Patient;
using ProKnow.Test;
using ProKnow.Upload;

namespace ProKnow.Patients.Test
{
    [TestClass]
    public class PatientItemTest
    {
        private static string _patientMrnAndName = "SDK-PatientItemTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = new Uploads(_proKnow);
        private static WorkspaceItem _workspaceItem;
        private static PatientItem _patientItem;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);

            // Create a test workspace
            _workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName);

            // Create a test patient
            var patientSummary = await TestHelper.CreatePatientAsync(_patientMrnAndName);

            // Upload test files
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientMetadata { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceItem.Id, uploadPath, overrides);

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
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);
        }

        [TestMethod]
        public void FindEntitiesTest_Predicate_1st_Level()
        {
           var imageSetEntities = _patientItem.FindEntities(e => e.Type == "image_set");
            Assert.AreEqual(imageSetEntities.Count, 1);
        }

        [TestMethod]
        public void FindEntitiesTest_Predicate_2nd_Level()
        {
            var structureSetEntities = _patientItem.FindEntities(e => e.Type == "structure_set");
            Assert.AreEqual(structureSetEntities.Count, 1);
        }

        [TestMethod]
        public void FindEntitiesTest_Predicate_3rd_Level()
        {
            var planEntities = _patientItem.FindEntities(e => e.Type == "plan");
            Assert.AreEqual(planEntities.Count, 1);
        }

        [TestMethod]
        public void FindEntitiesTest_Predicate_4th_Level()
        {
            var doseEntities = _patientItem.FindEntities(e => e.Type == "dose");
            Assert.AreEqual(doseEntities.Count, 1);
        }
    }
}
