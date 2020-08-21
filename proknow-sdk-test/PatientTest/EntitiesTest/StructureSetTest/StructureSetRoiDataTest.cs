using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Geometry;
using ProKnow.Test;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.StructureSet.Test
{
    [TestClass]
    public class StructureSetRoiDataTest
    {
        private static readonly string _testClassName = nameof(StructureSetRoiDataTest);

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
        public async Task SaveAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Get the tag for the PTV
            var ptvRoiItem0 = structureSetItem.Rois.First(r => r.Name == "PTV");
            var ptvTag0 = ptvRoiItem0.Tag;

            // Construct new contour and point data
            var contours = new StructureSetRoiContour[] {
                new StructureSetRoiContour() {
                    Position = 297.5,
                    Paths = new Point2D[][] { new Point2D[] { new Point2D(-20, 0), new Point2D(-20, 70), new Point2D(0, 70), new Point2D(0, 0) }}
                },
                new StructureSetRoiContour() {
                    Position = 300.5,
                    Paths = new Point2D[][] { new Point2D[] { new Point2D(-30, -5), new Point2D(-30, 80), new Point2D(10, 80), new Point2D(10, -5) }}
                },
                new StructureSetRoiContour() {
                    Position = 303.5,
                    Paths = new Point2D[][] { new Point2D[] { new Point2D(-40, -10), new Point2D(-40, 90), new Point2D(20, 90), new Point2D(20, -10) }}
                } };
            var points = new Point3D[] { new Point3D(-10, 300.5, 40) };

            // Get a draft of the structure set
            string ptvTag1, isoTag1;
            using (var draft = await structureSetItem.DraftAsync())
            {
                // Replace the PTV contour data, saving the new tag
                var ptvRoiItem = draft.Rois.First(r => r.Name == "PTV");
                var ptvRoiData = await ptvRoiItem.GetDataAsync();
                ptvRoiData.Contours = contours;
                await ptvRoiData.SaveAsync();
                ptvTag1 = ptvRoiItem.Tag;
                Assert.AreNotEqual(ptvTag0, ptvTag1);

                // Add an ISO point
                var isoRoiItem = await draft.CreateRoiAsync("ISO", Color.Yellow, "ISOCENTER");
                var isoRoiData = await isoRoiItem.GetDataAsync();
                isoRoiData.Points = points;
                await isoRoiData.SaveAsync();
                isoTag1 = isoRoiItem.Tag;

                // Commit the changes to the draft
                await draft.ApproveAsync();
            }

            // Refresh the structure set
            await structureSetItem.RefreshAsync();

            // Verify that the changes to the PTV were saved
            var ptvRoiItem2 = structureSetItem.Rois.First(r => r.Name == "PTV");
            var ptvRoiData2 = await ptvRoiItem2.GetDataAsync();
            Assert.AreEqual(contours.Length, ptvRoiData2.Contours.Length);
            for (int c = 0; c < contours.Length; c++)
            {
                Assert.AreEqual(contours[c].Position, ptvRoiData2.Contours[c].Position);
                Assert.AreEqual(contours[c].Paths.Length, ptvRoiData2.Contours[c].Paths.Length);
                for (int p = 0; p < contours[c].Paths.Length; p++)
                {
                    Assert.AreEqual(contours[c].Paths[p].Length, ptvRoiData2.Contours[c].Paths[p].Length);
                    for (int i = 0; i < contours[c].Paths[p].Length; i++)
                    {
                        Assert.AreEqual(contours[c].Paths[p][i].X, ptvRoiData2.Contours[c].Paths[p][i].X);
                        Assert.AreEqual(contours[c].Paths[p][i].Z, ptvRoiData2.Contours[c].Paths[p][i].Z);
                    }
                }
            }

            // Verify that the changes to the ISO were saved
            var isoRoiItem2 = structureSetItem.Rois.First(r => r.Name == "ISO");
            var isoRoiData2 = await isoRoiItem2.GetDataAsync();
            Assert.AreEqual(points.Length, isoRoiData2.Points.Length);
            for (int p = 0; p < points.Length; p++)
            {
                Assert.AreEqual(points[p].X, isoRoiData2.Points[p].X);
                Assert.AreEqual(points[p].Y, isoRoiData2.Points[p].Y);
                Assert.AreEqual(points[p].Z, isoRoiData2.Points[p].Z);
            }

            // Verify that the tags for the ROI items were updated
            Assert.AreEqual(ptvTag1, ptvRoiItem2.Tag);
            Assert.AreEqual(isoTag1, isoRoiItem2.Tag);
        }
    }
}
