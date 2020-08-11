using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.StructureSet.Test
{
    [TestClass]
    public class StructureSetDataTest
    {
        private static readonly string _testClassName = nameof(StructureSetDataTest);

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
        public async Task RoisTest()
        {
            var testNumber = 1;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;
            var structureSetData = structureSetItem.Data;

            var expectedNames = new string[] { "BODY", "PAROTID_LT", "PTV" }.ToHashSet();
            var actualNames = structureSetData.Rois.Select(r => r.Name).ToHashSet();
            Assert.IsTrue(expectedNames.SetEquals(actualNames));
        }
    }
}
