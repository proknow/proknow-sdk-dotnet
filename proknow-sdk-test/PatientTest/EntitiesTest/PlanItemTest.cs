﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class PlanItemTest
    {
        private static string _patientMrnAndName = "SDK-PlanItemTest";
        private static ProKnowApi _proKnow = TestSettings.ProKnow;
        private static string _workspaceId;
        private static string _downloadFolderRoot = Path.Combine(Path.GetTempPath(), _patientMrnAndName);

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
            // Delete test workspace
            await _proKnow.Workspaces.DeleteAsync(_workspaceId);

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
            _workspaceId = workspaceItem.Id;

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RP.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "plan");
            var planItem = await entitySummaries[0].GetAsync() as PlanItem;

            // Download the entity to an existing directory using the default filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            Directory.CreateDirectory(downloadFolder);
            string expectedDownloadPath = Path.Combine(downloadFolder, $"RP.{planItem.Uid}.dcm");
            string actualDownloadPath = await planItem.DownloadAsync(downloadFolder);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_ExistingFileWithExistingParent()
        {
            var testNumber = 2;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);
            _workspaceId = workspaceItem.Id;

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RP.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "plan");
            var planItem = await entitySummaries[0].GetAsync() as PlanItem;

            // Download the entity to an existing filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            Directory.CreateDirectory(downloadFolder);
            string expectedDownloadPath = Path.Combine(downloadFolder, "RP.dcm");
            File.WriteAllText(expectedDownloadPath, "This is an existing file!");
            string actualDownloadPath = await planItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_NewFileWithExistingParent()
        {
            var testNumber = 3;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);
            _workspaceId = workspaceItem.Id;

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RP.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "plan");
            var planItem = await entitySummaries[0].GetAsync() as PlanItem;

            // Download the entity to an existing directory using a specified filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            string expectedDownloadPath = Path.Combine(downloadFolder, "RP.dcm");
            string actualDownloadPath = await planItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_NewFileWithNonexistingParent()
        {
            var testNumber = 4;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);
            _workspaceId = workspaceItem.Id;

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RP.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "plan");
            var planItem = await entitySummaries[0].GetAsync() as PlanItem;

            // Download the entity to an nonexisting directory using a specified filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            string expectedDownloadPath = Path.Combine(downloadFolder, "grandparent", "parent", "RP.dcm");
            string actualDownloadPath = await planItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }
    }
}
