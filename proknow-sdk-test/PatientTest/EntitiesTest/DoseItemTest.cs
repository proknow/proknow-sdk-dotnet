using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class DoseItemTest
    {
        private static readonly string _patientMrnAndName = "SDK-DoseItemTest";
        private static readonly string _downloadFolderRoot = Path.Combine(Path.GetTempPath(), _patientMrnAndName);

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspacesAsync(_patientMrnAndName);

            // Create download folder root
            if (Directory.Exists(_downloadFolderRoot))
            {
                Directory.Delete(_downloadFolderRoot, true);
            }
            Directory.CreateDirectory(_downloadFolderRoot);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_patientMrnAndName);

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
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "dose");
            var doseItem = await entitySummaries[0].GetAsync() as DoseItem;

            // Download the entity to an existing directory using the default filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            Directory.CreateDirectory(downloadFolder);
            string expectedDownloadPath = Path.Combine(downloadFolder, $"RD.{doseItem.Uid}.dcm");
            string actualDownloadPath = await doseItem.DownloadAsync(downloadFolder);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_ExistingFileWithExistingParent()
        {
            var testNumber = 2;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "dose");
            var doseItem = await entitySummaries[0].GetAsync() as DoseItem;

            // Download the entity to an existing filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            Directory.CreateDirectory(downloadFolder);
            string expectedDownloadPath = Path.Combine(downloadFolder, "RD.dcm");
            File.WriteAllText(expectedDownloadPath, "This is an existing file!");
            string actualDownloadPath = await doseItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_NewFileWithExistingParent()
        {
            var testNumber = 3;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "dose");
            var doseItem = await entitySummaries[0].GetAsync() as DoseItem;

            // Download the entity to an existing directory using a specified filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            string expectedDownloadPath = Path.Combine(downloadFolder, "RD.dcm");
            string actualDownloadPath = await doseItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_NewFileWithNonexistingParent()
        {
            var testNumber = 4;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "dose");
            var doseItem = await entitySummaries[0].GetAsync() as DoseItem;

            // Download the entity to an nonexisting directory using a specified filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            string expectedDownloadPath = Path.Combine(downloadFolder, "grandparent", "parent", "RD.dcm");
            string actualDownloadPath = await doseItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }
    }
}
