using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.StructureSet.Test
{
    [TestClass]
    internal class StructureSetRoiItemTest
    {
        private static readonly string _testClassName = nameof(StructureSetRoiItemTest);

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

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
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
            Assert.IsFalse(structureSetItem.Rois.Any(r => r.Name == "PTV"));
        }

        [TestMethod]
        public async Task GetDataAsyncTest_Contours()
        {
            var testNumber = 2;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Get the data for an ROI
            var structureSetRoiItem = structureSetItem.Rois.First(r => r.Name == "PTV");
            var structureSetRoiData = await structureSetRoiItem.GetDataAsync();

            // Verify the contour data
            Assert.AreEqual(3, structureSetRoiData.Contours.Length);
            Assert.AreEqual(297.5, structureSetRoiData.Contours[0].Position, 0.0005);
            Assert.AreEqual(1, structureSetRoiData.Contours[0].Paths.Length);
            Assert.AreEqual(52, structureSetRoiData.Contours[0].Paths[0].Length);
            Assert.AreEqual(46.8, structureSetRoiData.Contours[0].Paths[0][0].X, 0.0005);
            Assert.AreEqual(15.9, structureSetRoiData.Contours[0].Paths[0][0].Z, 0.0005);
        }

        [TestMethod]
        public async Task GetDataAsyncTest_Points()
        {
            var testNumber = 3;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("StructureSet", "RS.Points.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Get the data for an ROI
            var structureSetRoiItem = structureSetItem.Rois.First(r => r.Name == "COUCH_SU");
            var structureSetRoiData = await structureSetRoiItem.GetDataAsync();

            // Verify the point data
            Assert.AreEqual(1, structureSetRoiData.Points.Length);
            Assert.AreEqual(-1.629, structureSetRoiData.Points[0].X, 0.0005);
            Assert.AreEqual(-273.501, structureSetRoiData.Points[0].Y, 0.0005);
            Assert.AreEqual(-199.457, structureSetRoiData.Points[0].Z, 0.0005);
        }

        [TestMethod]
        public async Task IsEditableTest()
        {
            var testNumber = 4;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Get the PTV
            var roiItem = structureSetItem.Rois.First(r => r.Name == "PTV");

            // Verify that the PTV is not editable
            Assert.IsFalse(roiItem.IsEditable());

            // Get a draft of the structure set
#pragma warning disable IDE0063 // Use simple 'using' statement
            using (var draft = await structureSetItem.DraftAsync())
#pragma warning restore IDE0063 // Use simple 'using' statement
            {
                // Get the PTV
                roiItem = draft.Rois.First(r => r.Name == "PTV");

                // Verify that the PTV is editable
                Assert.IsTrue(roiItem.IsEditable());
            }
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            var testNumber = 5;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
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
            Assert.IsFalse(structureSetItem.Rois.Any(r => r.Name == "PTV"));
            var roiItem2 = structureSetItem.Rois.First(r => r.Name == "PTV2");
            Assert.AreEqual(roiItem0.Tag, roiItem2.Tag);
            Assert.AreEqual(Color.Blue.ToArgb(), roiItem2.Color.ToArgb());
            Assert.AreEqual("PTV", roiItem2.Type);
        }
    }
}
