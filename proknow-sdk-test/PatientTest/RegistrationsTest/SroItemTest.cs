using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Registrations.Test
{
    [TestClass]
    public class SroItemTest
    {
        private static readonly string _testClassName = nameof(SroItemTest);
        private static readonly string _downloadFolderRoot = Path.Combine(Path.GetTempPath(), _testClassName);

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Cleanup from previous test stoppage or failure, if necessary
            await ClassCleanup();

            // Create download folder root
            Directory.CreateDirectory(_downloadFolderRoot);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);

            // Delete download folder
            if (Directory.Exists(_downloadFolderRoot))
            {
                Directory.Delete(_downloadFolderRoot, true);
            }
        }

        [TestMethod]
        public async Task DownloadAsyncTest_Directory()
        {
            var testNumber = 1;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Sro", "reg.dcm"), 1);
            var sroSummary = patientItem.Studies[0].Sros[0];
            var sroItem = await sroSummary.GetAsync();

            // Download the SRO to an existing directory using the default filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            Directory.CreateDirectory(downloadFolder);
            string expectedDownloadPath = Path.Combine(downloadFolder, $"REG.{sroItem.Uid}.dcm");
            string actualDownloadPath = await sroItem.DownloadAsync(downloadFolder);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "reg.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_ExistingFileWithExistingParent()
        {
            var testNumber = 2;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Sro", "reg.dcm"), 1);
            var sroSummary = patientItem.Studies[0].Sros[0];
            var sroItem = await sroSummary.GetAsync();

            // Download the SRO to an existing filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            Directory.CreateDirectory(downloadFolder);
            string expectedDownloadPath = Path.Combine(downloadFolder, "REG.dcm");
            File.WriteAllText(expectedDownloadPath, "This is an existing file!");
            string actualDownloadPath = await sroItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "reg.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_NewFileWithExistingParent()
        {
            var testNumber = 3;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Sro", "reg.dcm"), 1);
            var sroSummary = patientItem.Studies[0].Sros[0];
            var sroItem = await sroSummary.GetAsync();

            // Download the SRO to an existing directory using a specified filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            string expectedDownloadPath = Path.Combine(downloadFolder, "REG.dcm");
            string actualDownloadPath = await sroItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "reg.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_NewFileWithNonexistingParent()
        {
            var testNumber = 4;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Sro", "reg.dcm"), 1);
            var sroSummary = patientItem.Studies[0].Sros[0];
            var sroItem = await sroSummary.GetAsync();

            // Download the SRO to an nonexisting directory using a specified filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            string expectedDownloadPath = Path.Combine(downloadFolder, "grandparent", "parent", "REG.dcm");
            string actualDownloadPath = await sroItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "REG.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }
    }
}
