using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.StructureSet.Test
{
    [TestClass]
    public class StructureSetRoiItemTest
    {
        private static readonly string _patientMrnAndName = "SDK-StructureSetRoiItemTest";

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspaces, if necessary
            await TestHelper.DeleteWorkspacesAsync(_patientMrnAndName);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_patientMrnAndName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Get a draft of the structure set
            using (var draft = await structureSetItem.DraftAsync())
            {
                // Get the PTV
                var roiItem = draft.Rois.First(r => r.Name == "PTV");

                // Delete it
                await roiItem.DeleteAsync();

                // Commit the changes to the draft
                await draft.ApproveAsync();
            }

            // Refresh the structure set
            await structureSetItem.RefreshAsync();

            // Verify that the PTV was deleted
            Assert.IsFalse(structureSetItem.Rois.Where(r => r.Name == "PTV").Any());
        }

        [TestMethod]
        public async Task IsEditableTest()
        {
            var testNumber = 2;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Get the PTV
            var roiItem = structureSetItem.Rois.First(r => r.Name == "PTV");

            // Verify that the PTV is not editable
            Assert.IsFalse(roiItem.IsEditable());

            // Get a draft of the structure set
            using var draft = await structureSetItem.DraftAsync();
            // Get the PTV
            roiItem = draft.Rois.First(r => r.Name == "PTV");

            // Verify that the PTV is editable
            Assert.IsTrue(roiItem.IsEditable());
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            var testNumber = 3;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Get the original PTV
            var roiItem0 = structureSetItem.Rois.First(r => r.Name == "PTV");

            // Get a draft of the structure set
            using (var draft = await structureSetItem.DraftAsync())
            {
                // Get the PTV
                var roiItem1 = draft.Rois.First(r => r.Name == "PTV");

                // Change the name, color, and type and save the changes
                roiItem1.Name = "PTV2";
                roiItem1.Color = Color.Blue; // was Color.Red
                roiItem1.Type = "PTV"; // was "ORGAN"
                await roiItem1.SaveAsync();

                // Commit the changes to the draft
                await draft.ApproveAsync();
            }

            // Refresh the structure set
            await structureSetItem.RefreshAsync();

            // Verify that changes to the PTV were saved
            Assert.IsFalse(structureSetItem.Rois.Where(r => r.Name == "PTV").Any());
            var roiItem2 = structureSetItem.Rois.First(r => r.Name == "PTV2");
            Assert.AreEqual(roiItem0.Tag, roiItem2.Tag);
            Assert.AreEqual(Color.Blue.ToArgb(), roiItem2.Color.ToArgb());
            Assert.AreEqual("PTV", roiItem2.Type);
        }
    }
}
