using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class EntitySummaryTest
    {
        private static readonly string _testClassName = nameof(EntitySummaryTest);
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
        public async Task DeleteAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a plan
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RP.dcm"));

            // Get the entity summary for the plan
            var entitySummary = patientItem.FindEntities(e => e.Type == "plan")[0];

            // Delete the entity
            await entitySummary.DeleteAsync();

            // Verify it was deleted
            while (true)
            {
                await patientItem.RefreshAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "plan");
                if (entitySummaries.Count == 0)
                {
                    break;
                }
            }
        }

        [TestMethod]
        public async Task ImageSet_GetAsyncTest()
        {
            var testNumber = 2;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "CT"));
            var entitySummary = patientItem.FindEntities(e => e.Type == "image_set")[0];

            // Get the image set item from the entity summary
            var imageSetItem = await entitySummary.GetAsync();

            // Verify the returned image set item
            Assert.AreEqual(imageSetItem.WorkspaceId, workspaceItem.Id);
            Assert.AreEqual(imageSetItem.PatientId, patientItem.Id);
            Assert.AreEqual(imageSetItem.Id, entitySummary.Id);
        }

        [TestMethod]
        public async Task StructureSet_GetAsyncTest()
        {
            var testNumber = 3;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"));
            var entitySummary = patientItem.FindEntities(e => e.Type == "structure_set")[0];

            // Get the structure set item from the entity summary
            var structureSetItem = await entitySummary.GetAsync();

            // Verify the returned structure set item
            Assert.AreEqual(structureSetItem.WorkspaceId, workspaceItem.Id);
            Assert.AreEqual(structureSetItem.PatientId, patientItem.Id);
            Assert.AreEqual(structureSetItem.Id, entitySummary.Id);
        }

        [TestMethod]
        public async Task Plan_GetAsyncTest()
        {
            var testNumber = 4;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a plan
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RP.dcm"));
            var entitySummary = patientItem.FindEntities(e => e.Type == "plan")[0];

            // Get the plan item from the entity summary
            var planItem = await entitySummary.GetAsync();

            // Verify the returned plan item
            Assert.AreEqual(planItem.WorkspaceId, workspaceItem.Id);
            Assert.AreEqual(planItem.PatientId, patientItem.Id);
            Assert.AreEqual(planItem.Id, entitySummary.Id);
        }

        [TestMethod]
        public async Task Dose_GetAsyncTest()
        {
            var testNumber = 5;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a dose
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"));
            var entitySummary = patientItem.FindEntities(e => e.Type == "dose")[0];

            // Get the dose item from the entity summary
            var doseItem = await entitySummary.GetAsync();

            // Verify the returned dose item
            Assert.AreEqual(doseItem.WorkspaceId, workspaceItem.Id);
            Assert.AreEqual(doseItem.PatientId, patientItem.Id);
            Assert.AreEqual(doseItem.Id, entitySummary.Id);
        }
    }
}
