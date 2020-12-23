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
            var downloadedFile = Path.Combine(downloadPath, $"CT.1.3.6.1.4.1.22213.2.26558.2.57");
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

            // Get the data for the first image (CT.5.dcm)
            var imageData = await imageSetItem.GetImageDataAsync(0);

            // Verify the data
            Assert.AreEqual(imageSetItem.Data.NumberOfRows * imageSetItem.Data.NumberOfColumns, imageData.Length);
            var intercept = imageSetItem.Data.Images[0].RescaleIntercept;
            var slope = imageSetItem.Data.Images[0].RescaleSlope;
            var tolerance = 0.5 * slope; // half of a pixel
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0029", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData[201], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("002e", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData[202], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("002f", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData[203], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0030", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData[204], tolerance);
            Assert.AreEqual(-1024 + 1.001 * ushort.Parse("0026", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * imageData[205], tolerance);
        }
    }
}
