using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.StructureSet.Test
{
    [TestClass]
    public class StructureSetVersionsTest
    {
        private static readonly string _testClassName = nameof(StructureSetVersionsTest);

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

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Get the original version of that structure set and label it as such
            var versions = await structureSetItem.Versions.QueryAsync();
            var original = versions[0];
            original.Label = "original";
            await original.SaveAsync();

            // Create another version of that structure set, add an ROI, and commit the change
            using (var draft = await structureSetItem.DraftAsync())
            {
                await draft.CreateRoiAsync("thing1", Color.Magenta, "ORGAN");
                await draft.ApproveAsync("original + thing1");
            }
            await structureSetItem.RefreshAsync();

            // Create another version of that structure set, add another ROI, and commit the change
            using (var draft = await structureSetItem.DraftAsync())
            {
                await draft.CreateRoiAsync("thing2", Color.Azure, "ORGAN");
                await draft.ApproveAsync("original + thing1 + thing2");
            }
            await structureSetItem.RefreshAsync();

            // Verify there are three versions
            versions = await structureSetItem.Versions.QueryAsync();
            Assert.AreEqual(3, versions.Count);

            // Delete a version
            var version = versions.First(v => v.Label == "original + thing1");
            await structureSetItem.Versions.DeleteAsync(version.VersionId);

            // Verify the version was deleted
            versions = await structureSetItem.Versions.QueryAsync();
            Assert.AreEqual(2, versions.Count);
            Assert.IsTrue(versions.Any(v => v.Label == "original"));
            Assert.IsTrue(versions.Any(v => v.Label == "original + thing1 + thing2"));
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            var testNumber = 2;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"));
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem1 = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Get the versions of that structure set
            var structureSetVersionItems = await structureSetItem1.Versions.QueryAsync();

            // Verify that there is only one
            Assert.AreEqual(1, structureSetVersionItems.Count);

            // Get the corresponding structure set item for that version
            var structureSetVersionItem = structureSetVersionItems[0];
            var structureSetItem2 = await structureSetVersionItem.GetAsync();

            // Verify the structure set item returned
            Assert.AreEqual(structureSetItem1.Id, structureSetItem2.Id);
            Assert.AreEqual(structureSetItem1.WorkspaceId, structureSetItem2.WorkspaceId);
        }

        [TestMethod]
        public async Task QueryAsyncTest()
        {
            var testNumber = 3;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"));
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Create another version of the structure set, add an ROI, and commit the change
            using (var draft = await structureSetItem.DraftAsync())
            {
                await draft.CreateRoiAsync("thing1", Color.Magenta, "ORGAN");
                await draft.ApproveAsync("original + thing1");
            }
            await structureSetItem.RefreshAsync();

            // Create another version of the structure set, add another ROI, and commit the change
            using (var draft = await structureSetItem.DraftAsync())
            {
                await draft.CreateRoiAsync("thing2", Color.Azure, "ORGAN");
                await draft.ApproveAsync("original + thing1 + thing2");
            }
            await structureSetItem.RefreshAsync();

            // Query the versions and verify the results
            var structureSetVersionItems = await structureSetItem.Versions.QueryAsync();
            Assert.AreEqual(3, structureSetVersionItems.Count);
            Assert.IsTrue(structureSetVersionItems.Any(v => v.Label == null));
            Assert.IsTrue(structureSetVersionItems.Any(v => v.Label == "original + thing1"));
            Assert.IsTrue(structureSetVersionItems.Any(v => v.Label == "original + thing1 + thing2"));
        }
    }
}
