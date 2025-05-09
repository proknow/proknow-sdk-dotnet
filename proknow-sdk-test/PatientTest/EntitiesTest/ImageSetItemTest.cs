using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class ImageSetItemTest
    {
        private static readonly string _testClassName = nameof(ImageSetItemTest);
        private static readonly string _downloadFolder = Path.Combine(Path.GetTempPath(), _testClassName);

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

            // Delete download directory
            if (Directory.Exists(_downloadFolder))
            {
                Directory.Delete(_downloadFolder, true);
            }
        }

        [TestMethod]
        public async Task DownloadAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an imageset
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "CT"));
            var entitySummaries = patientItem.FindEntities(e => e.Type == "image_set");
            var imageSetItem = await entitySummaries[0].GetAsync() as ImageSetItem;

            // Download the image set
            string downloadPath = await imageSetItem.DownloadAsync(_downloadFolder);
            var downloadedFiles = Directory.GetFiles(downloadPath);

            // Make sure the same number of images were downloaded
            Assert.AreEqual(5, downloadedFiles.Length);

            // Check contents of one downloaded file (don't need to check them all!)
            var uploadedFile = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT", "CT.1.dcm");
            var downloadedFile = Path.Combine(downloadPath, $"CT.1.3.6.1.4.1.22213.2.26558.2.57.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadedFile, downloadedFile));
        }

        [TestMethod]
        public async Task GetImageDataTest()
        {
            int testNumber = 2;

            // Create a workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient with an image set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "CT"));
            var entitySummaries = patientItem.FindEntities(e => e.Type == "image_set");
            var imageSetItem = await entitySummaries[0].GetAsync() as ImageSetItem;

            var intercept = imageSetItem.Data.Images[0].RescaleIntercept;
            var slope = imageSetItem.Data.Images[0].RescaleSlope;
            var tolerance = 0.5 * slope; // half of a pixel

            // Get the data for the first image (CT.5.dcm)
            // Verify the first image data
            var imageData1 = await imageSetItem.GetImageDataAsync(0);
            Assert.AreEqual(imageSetItem.Data.Images[0].Position, 294.5);
            Assert.AreEqual(imageSetItem.Data.NumberOfRows * imageSetItem.Data.NumberOfColumns, imageData1.Length);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0029", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData1[201], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("002e", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData1[202], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("002f", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData1[203], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0030", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData1[204], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0026", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData1[205], tolerance);

            // Verify the second image data
            var imageData2 = await imageSetItem.GetImageDataAsync(1);
            Assert.AreEqual(imageSetItem.Data.Images[1].Position, 297.5);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("002e", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData2[201], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0034", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData2[202], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0034", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData2[203], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0032", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData2[204], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0028", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData2[205], tolerance);

            // Verify the third image data
            var imageData3 = await imageSetItem.GetImageDataAsync(2);
            Assert.AreEqual(imageSetItem.Data.Images[2].Position, 300.5);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0032", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData3[201], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0036", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData3[202], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("003b", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData3[203], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0036", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData3[204], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0032", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData3[205], tolerance);

            // Verify the fourth image data
            var imageData4 = await imageSetItem.GetImageDataAsync(3);
            Assert.AreEqual(imageSetItem.Data.Images[3].Position, 303.5);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0048", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData4[201], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0050", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData4[202], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0058", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData4[203], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0052", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData4[204], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("004a", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData4[205], tolerance);

            // Verify the fifth image data
            var imageData5 = await imageSetItem.GetImageDataAsync(4);
            Assert.AreEqual(imageSetItem.Data.Images[4].Position, 306.5);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0040", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData5[201], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0044", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData5[202], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("004a", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData5[203], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0046", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData5[204], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("003e", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData5[205], tolerance);
        }
    }
}
