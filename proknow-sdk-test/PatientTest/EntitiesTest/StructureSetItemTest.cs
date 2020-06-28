using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class StructureSetItemTest
    {
        private static string _patientMrnAndName = "SDK-StructureSetItemTest";
        private static ProKnowApi _proKnow = TestSettings.ProKnow;
        private static int _lockRenewalBuffer = _proKnow.LockRenewalBuffer;
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
            // Restore lock renewal buffer number in case it was changed
            _proKnow.LockRenewalBuffer = _lockRenewalBuffer;

            // Delete test workspace
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
            _workspaceId = workspaceItem.Id;

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Download the entity to an existing directory using the default filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            Directory.CreateDirectory(downloadFolder);
            string expectedDownloadPath = Path.Combine(downloadFolder, $"RS.{structureSetItem.Uid}.dcm");
            string actualDownloadPath = await structureSetItem.DownloadAsync(downloadFolder);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RS.dcm");
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
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Download the entity to an existing filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            Directory.CreateDirectory(downloadFolder);
            string expectedDownloadPath = Path.Combine(downloadFolder, "RS.dcm");
            File.WriteAllText(expectedDownloadPath, "This is an existing file!");
            string actualDownloadPath = await structureSetItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RS.dcm");
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
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Download the entity to an existing directory using a specified filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            string expectedDownloadPath = Path.Combine(downloadFolder, "RS.dcm");
            string actualDownloadPath = await structureSetItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RS.dcm");
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
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Download the entity to an nonexisting directory using a specified filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            string expectedDownloadPath = Path.Combine(downloadFolder, "grandparent", "parent", "RS.dcm");
            string actualDownloadPath = await structureSetItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RS.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }

        [TestMethod]
        public async Task DraftAsyncTest_CreateNewDraft()
        {
            var testNumber = 5;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);
            _workspaceId = workspaceItem.Id;

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Set the lock renewal buffer to a large value for testing so that the lock is renewed quickly (expires_in will be 360000 ms)
            _proKnow.LockRenewalBuffer = 360;

            // Get a draft of the structure set
            await using (var draft = await structureSetItem.DraftAsync())
            {
                // Verify that the same structure set was returned
                Assert.AreEqual(structureSetItem.Id, draft.Id);

                // Verify that the structure set is now editable
                Assert.IsTrue(draft.IsEditable);

                // Verify that the structure set is a draft
                Assert.IsTrue(draft.IsDraft);

                // Verify that the structure set has a draft lock
                Assert.IsNotNull(draft.DraftLock);

                // Verify that the structure set has a draft lock renewer
                DateTime originalExpiresAt = DateTime.ParseExact(draft.DraftLock.ExpiresAt, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                while (true)
                {
                    DateTime newExpiresAt = DateTime.ParseExact(draft.DraftLock.ExpiresAt, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                    if (newExpiresAt != originalExpiresAt)
                    {
                        break;
                    }
                    Thread.Sleep(10);
                }
            }
        }

        [TestMethod]
        public async Task DraftAsyncTest_RenewDraftLock()
        {
            var testNumber = 6;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);
            _workspaceId = workspaceItem.Id;

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Set the lock renewal buffer to a large value for testing so that the lock is renewed quickly (expires_in will be 360000 ms)
            _proKnow.LockRenewalBuffer = 360;

            // Get a draft of the structure set
            DateTime originalExpiresAt;
            await using (var draft = await structureSetItem.DraftAsync())
            {
                // Save the original expiration date
                originalExpiresAt = DateTime.ParseExact(draft.DraftLock.ExpiresAt, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            }

            // Try to get another draft of the structure set
            DateTime newExpiresAt;
            await using (var draft = await structureSetItem.DraftAsync())
            {
                // Save the new expiration date
                newExpiresAt = DateTime.ParseExact(draft.DraftLock.ExpiresAt, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            }

            // Verify that the expiration dates are different
            Assert.AreNotEqual(newExpiresAt, originalExpiresAt);
        }

        [TestMethod]
        public async Task IDisposableAsyncTest()
        {
            var testNumber = 7;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);
            _workspaceId = workspaceItem.Id;

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Get a draft of the structure set
            var draft = await structureSetItem.DraftAsync();

            // Verify that the structure set has a draft lock
            Assert.IsNotNull(draft.DraftLock);

            // Dispose of the structure set
            await draft.DisposeAsync();

            // Verify that the draft lock was removed when the structure set was disposed
            Assert.IsNull(draft.DraftLock);
            Assert.IsFalse(draft.IsEditable);
        }
    }
}
