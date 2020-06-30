using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
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
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_patientMrnAndName);

            // Delete download folder
            if (Directory.Exists(_downloadFolderRoot))
            {
                Directory.Delete(_downloadFolderRoot, true);
            }

            // Restore lock renewal buffer number in case it was changed
            _proKnow.LockRenewalBuffer = _lockRenewalBuffer;
        }

        [TestMethod]
        public async Task CreateRoiAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);
            _workspaceId = workspaceItem.Id;

            // Create a test patient with only an image set
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, Path.Combine("Becker^Matthew", "CT"), 1);

            // Create a new structure set
            var imageSetSummary = patientItem.FindEntities(e => e.Type == "image_set")[0];
            var structureSetSummary = await patientItem.CreateStructureSetAsync("New", imageSetSummary.Id);
            var structureSetItem = await structureSetSummary.GetAsync() as StructureSetItem;

            // Get a draft of the structure set
            using (var draft = await structureSetItem.DraftAsync())
            {
                // Create a new ROI
                var returnedRoi = await draft.CreateRoiAsync("new", new int[3] { 12, 34, 56 }, "ORGAN");

                // Verify that the returned ROI has the correct properties
                Assert.AreEqual("new", returnedRoi.Name);
                Assert.AreEqual(12, returnedRoi.Color[0]);
                Assert.AreEqual(34, returnedRoi.Color[1]);
                Assert.AreEqual(56, returnedRoi.Color[2]);
                Assert.AreEqual("ORGAN", returnedRoi.Type);

                // Verify that the new ROI was added to the structure set with the correct properties
                var structureSetRoi = draft.Rois.First(r => r.Id == returnedRoi.Id);
                Assert.AreEqual(returnedRoi.Name, structureSetRoi.Name);
                Assert.AreEqual(returnedRoi.Color[0], structureSetRoi.Color[0]);
                Assert.AreEqual(returnedRoi.Color[1], structureSetRoi.Color[1]);
                Assert.AreEqual(returnedRoi.Color[2], structureSetRoi.Color[2]);
                Assert.AreEqual(returnedRoi.Type, structureSetRoi.Type);
            }
        }

        [TestMethod]
        public async Task DownloadAsyncTest_Directory()
        {
            var testNumber = 2;

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
            var testNumber = 3;

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
            var testNumber = 4;

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
            var testNumber = 5;

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
            using (var draft = await structureSetItem.DraftAsync())
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
            var testNumber = 7;

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
            string originalLockId, newLockId;
            DateTime originalExpiresAt, newExpiresAt;
            using (var draft = await structureSetItem.DraftAsync())
            {
                // Save the original lock ID and expiration date
                originalLockId = draft.DraftLock.Id;
                originalExpiresAt = DateTime.ParseExact(draft.DraftLock.ExpiresAt, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

                // Try to get another draft of the structure set
                using (var draft2 = await structureSetItem.DraftAsync())
                {
                    // Save the new lock ID and expiration date
                    newLockId = draft2.DraftLock.Id;
                    newExpiresAt = DateTime.ParseExact(draft2.DraftLock.ExpiresAt, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                }
            }

            // Verify that a new lock was obtained
            Assert.AreNotEqual(originalLockId, newLockId);
            Assert.AreNotEqual(originalExpiresAt, newExpiresAt);
        }

        [TestMethod]
        public async Task IDisposableTest()
        {
            var testNumber = 8;

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
            draft.Dispose();

            // Verify that the draft lock was removed when the structure set was disposed
            Assert.IsNull(draft.DraftLock);
            Assert.IsFalse(draft.IsEditable);
        }
    }
}
