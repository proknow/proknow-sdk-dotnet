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
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "CT"), 1);
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
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "CT"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "image_set");
            var imageSetItem = await entitySummaries[0].GetAsync() as ImageSetItem;

            // Get the data for the first image
            var bytes = await imageSetItem.GetImageDataAsync(0);

            // Verify the data
            Assert.AreEqual(512 * 512 * 2, bytes.Length);
            Assert.AreEqual(32, bytes[401]);
            Assert.AreEqual(0, bytes[402]);
            Assert.AreEqual(41, bytes[403]);
            Assert.AreEqual(0, bytes[404]);
            Assert.AreEqual(46, bytes[405]);
            Assert.AreEqual(0, bytes[406]);
            Assert.AreEqual(47, bytes[407]);
            Assert.AreEqual(00, bytes[408]);
            Assert.AreEqual(48, bytes[409]);
            Assert.AreEqual(0, bytes[410]);
        }
    }
}
