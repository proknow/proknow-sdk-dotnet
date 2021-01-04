using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Exceptions;
using ProKnow.Test;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class StructureSetItemTest
    {
        private static readonly string _testClassName = nameof(StructureSetItemTest);
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static readonly int _lockRenewalBuffer = _proKnow.LockRenewalBuffer;
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

            // Restore lock renewal buffer number in case it was changed
            _proKnow.LockRenewalBuffer = _lockRenewalBuffer;
        }

        [TestMethod]
        public async Task ApproveAsyncTest_InvalidOperationError()
        {
            var testNumber = 1;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Try to approve that (non-editable) structure set item and verify the exception
            try
            {
                await structureSetItem.ApproveAsync();
                Assert.Fail();
            }
            catch (InvalidOperationError ex)
            {
                Assert.AreEqual("Item is not editable.", ex.Message);
            }
        }

        [TestMethod]
        public async Task ApproveAsyncTest()
        {
            var testNumber = 2;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;
            var originalRoiCount = structureSetItem.Rois.Length;

            // Set the lock renewal buffer to a large value for testing so that the lock is renewed quickly (expires_in will be 360000 ms)
            _proKnow.LockRenewalBuffer = 360;

            // Create a draft of that structure set
#pragma warning disable IDE0063 // Use simple 'using' statement
            using (var draft = await structureSetItem.DraftAsync())
#pragma warning restore IDE0063 // Use simple 'using' statement
            {
                // Add an ROI and commit (approve) the change
                await draft.CreateRoiAsync("thing1", Color.Magenta, "ORGAN");
                var approvedStructureSetItem = await draft.ApproveAsync("original + thing1");

                // Verify that the draft is no longer editable
                Assert.IsFalse(draft.IsEditable);

                // Verify that the structure set item is no longer a draft
                Assert.IsFalse(draft.IsDraft);

                // Verify that the draft lock has been removed
                Assert.IsNull(draft.DraftLock);

                // Verify that the renewer was stopped
                await Task.Delay(10);
                Assert.IsNull(draft.DraftLock);

                // Verify the returned structure set item
                Assert.AreEqual(workspaceItem.Id, approvedStructureSetItem.WorkspaceId);
                Assert.AreEqual(originalRoiCount + 1, approvedStructureSetItem.Rois.Length);
                Assert.IsTrue(approvedStructureSetItem.Rois.Any(r => r.Name == "thing1"));
            }
        }

        [TestMethod]
        public async Task CreateRoiAsyncTest_InvalidOperationError()
        {
            var testNumber = 3;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);


            // Create a test patient with only an image set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "CT"));

            // Create a new structure set
            var imageSetSummary = patientItem.FindEntities(e => e.Type == "image_set")[0];
            var structureSetSummary = await patientItem.CreateStructureSetAsync("New", imageSetSummary.Id);
            var structureSetItem = await structureSetSummary.GetAsync() as StructureSetItem;

            // Try to create a new ROI and verify the exception
            try
            {
                await structureSetItem.CreateRoiAsync("new", Color.Magenta, "ORGAN");
                Assert.Fail();
            }
            catch (InvalidOperationError ex)
            {
                Assert.AreEqual("Item is not editable.", ex.Message);
            }
        }

        [TestMethod]
        public async Task CreateRoiAsyncTest()
        {
            var testNumber = 4;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with only an image set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "CT"));

            // Create a new structure set
            var imageSetSummary = patientItem.FindEntities(e => e.Type == "image_set")[0];
            var structureSetSummary = await patientItem.CreateStructureSetAsync("New", imageSetSummary.Id);
            var structureSetItem = await structureSetSummary.GetAsync() as StructureSetItem;

            // Get a draft of the structure set
            using (var draft = await structureSetItem.DraftAsync())
            {
                // Create a new ROI
                var returnedRoi = await draft.CreateRoiAsync("new", Color.Magenta, "ORGAN");

                // Verify that the returned ROI has the correct properties
                Assert.AreEqual("new", returnedRoi.Name);
                Assert.AreEqual(Color.Magenta.ToArgb(), returnedRoi.Color.ToArgb());
                Assert.AreEqual("ORGAN", returnedRoi.Type);

                // Verify that the new ROI was added to the draft structure set with the correct properties
                var draftRoi = draft.Rois.First(r => r.Id == returnedRoi.Id);
                Assert.AreEqual(returnedRoi.Name, draftRoi.Name);
                Assert.AreEqual(returnedRoi.Color, draftRoi.Color);
                Assert.AreEqual(returnedRoi.Type, draftRoi.Type);

                // Save the draft
                await draft.ApproveAsync();
            }

            // Refresh the structure set and verify that the new ROI was added
            await structureSetItem.RefreshAsync();
            var structureSetRoi = structureSetItem.Rois.First(r => r.Name == "new");
            Assert.AreEqual(Color.Magenta.ToArgb(), structureSetRoi.Color.ToArgb());
            Assert.AreEqual("ORGAN", structureSetRoi.Type);
        }

        [TestMethod]
        public async Task DiscardAsyncTest_InvalidOperationError()
        {
            var testNumber = 5;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Try to discard that (non-editable) structure set item and verify the exception
            try
            {
                await structureSetItem.DiscardAsync();
                Assert.Fail();
            }
            catch (InvalidOperationError ex)
            {
                Assert.AreEqual("Item is not editable.", ex.Message);
            }
        }

        [TestMethod]
        public async Task DiscardAsyncTest()
        {
            var testNumber = 6;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Set the lock renewal buffer to a large value for testing so that the lock is renewed quickly (expires_in will be 360000 ms)
            _proKnow.LockRenewalBuffer = 360;

            // Get a draft of the structure set
#pragma warning disable IDE0063 // Use simple 'using' statement
            using (var draft = await structureSetItem.DraftAsync())
#pragma warning restore IDE0063 // Use simple 'using' statement
            {
                // Discard the draft
                await draft.DiscardAsync();

                // Verify that the draft is no longer editable
                Assert.IsFalse(draft.IsEditable);

                // Verify that the draft lock has been removed
                Assert.IsNull(draft.DraftLock);

                // Verify that the renewer was stopped
                await Task.Delay(10);
                Assert.IsNull(draft.DraftLock);
            }
        }

        [TestMethod]
        public async Task DownloadAsyncTest_InvalidOperationError()
        {
            var testNumber = 7;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Get a draft of the structure set
#pragma warning disable IDE0063 // Use simple 'using' statement
            using (var draft = await structureSetItem.DraftAsync())
#pragma warning restore IDE0063 // Use simple 'using' statement
            {
                // Try to download the draft and verify the exception
                try
                {
                    await draft.DownloadAsync("doesn't matter");
                    Assert.Fail();
                }
                catch (InvalidOperationError ex)
                {
                    Assert.AreEqual("Structure set drafts cannot be downloaded.", ex.Message);
                }
            }
        }

        [TestMethod]
        public async Task DownloadAsyncTest_Directory()
        {
            var testNumber = 8;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"));
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
            var testNumber = 9;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"));
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
            var testNumber = 10;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"));
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
            var testNumber = 11;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"));
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
            var testNumber = 12;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"));
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Set the lock renewal buffer to a large value for testing so that the lock is renewed quickly (expires_in will be 360000 ms)
            _proKnow.LockRenewalBuffer = 360;

            // Get a draft of the structure set
#pragma warning disable IDE0063 // Use simple 'using' statement
            using (var draft = await structureSetItem.DraftAsync())
#pragma warning restore IDE0063 // Use simple 'using' statement
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
                    await Task.Delay(10);
                }
            }
        }

        [TestMethod]
        public async Task DraftAsyncTest_RenewDraftLock()
        {
            var testNumber = 13;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"));
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
#pragma warning disable IDE0063 // Use simple 'using' statement
                using (var draft2 = await structureSetItem.DraftAsync())
#pragma warning restore IDE0063 // Use simple 'using' statement
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
        public async Task RefreshAsyncTest_InvalidOperationError()
        {
            var testNumber = 14;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Create a draft of the structure set
#pragma warning disable IDE0063 // Use simple 'using' statement
            using (var draft = await structureSetItem.DraftAsync())
#pragma warning restore IDE0063 // Use simple 'using' statement
            {
                // Try to refresh the draft and verify the exception
                try
                {
                    await draft.RefreshAsync();
                    Assert.Fail();
                }
                catch (InvalidOperationError ex)
                {
                    Assert.AreEqual("Structure set drafts cannot be refreshed.", ex.Message);
                }
            }
        }

        [TestMethod]
        public async Task RefreshAsyncTest()
        {
            var testNumber = 15;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;
            var originalRoiCount = structureSetItem.Rois.Length;

            // Create a draft of that structure set
#pragma warning disable IDE0063 // Use simple 'using' statement
            using (var draft = await structureSetItem.DraftAsync())
#pragma warning restore IDE0063 // Use simple 'using' statement
            {
                // Add an ROI and commit (approve) the change
                await draft.CreateRoiAsync("thing1", Color.Magenta, "ORGAN");
                await draft.ApproveAsync("original + thing1");

                // Refresh the original structure set item
                await structureSetItem.RefreshAsync();

                // Verify the refreshed structure set item
                Assert.AreEqual(originalRoiCount + 1, structureSetItem.Rois.Length);
                Assert.IsTrue(structureSetItem.Rois.Any(r => r.Name == "thing1"));
            }
        }

        [TestMethod]
        public async Task ReleaseLockTest_NotEditable()
        {
            var testNumber = 16;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Try to release the (non-existent) draft lock (should not throw an exception)
            structureSetItem.ReleaseLock();
        }

        [TestMethod]
        public async Task ReleaseLockTest_Editable()
        {
            var testNumber = 17;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Set the lock renewal buffer to a large value for testing so that the lock is renewed quickly (expires_in will be 360000 ms)
            _proKnow.LockRenewalBuffer = 360;

            // Create a draft of that structure set
#pragma warning disable IDE0063 // Use simple 'using' statement
            using (var draft = await structureSetItem.DraftAsync())
#pragma warning restore IDE0063 // Use simple 'using' statement
            {
                // Release the lock
                draft.ReleaseLock();

                // Verify that the draft is no longer editable
                Assert.IsFalse(draft.IsEditable);

                // Verify that the draft lock has been removed
                Assert.IsNull(draft.DraftLock);

                // Verify that the renewer was stopped
                await Task.Delay(10);
                Assert.IsNull(draft.DraftLock);
            }
        }

        [TestMethod]
        public async Task StartRenewerTest_NotEditable()
        {
            var testNumber = 18;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Try to start the renewer for the (non-existent) draft lock (should not throw an exception)
            structureSetItem.StartRenewer();
        }

        [TestMethod]
        public async Task StartRenewerTest_Editable()
        {
            var testNumber = 19;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Set the lock renewal buffer to a large value for testing so that the lock is renewed quickly (expires_in will be 360000 ms)
            _proKnow.LockRenewalBuffer = 360;

            // Create a draft of that structure set
            var draft = await structureSetItem.DraftAsync();

            // Stop the renewer
            draft.StopRenewer();

            // Verify that the draft lock renewer is not running
            var originalExpiresAt = DateTime.ParseExact(draft.DraftLock.ExpiresAt, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            await Task.Delay(10);
            var currentExpiresAt = DateTime.ParseExact(draft.DraftLock.ExpiresAt, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            Assert.AreEqual(originalExpiresAt, currentExpiresAt);

            // Start the draft lock renewer
            draft.StartRenewer();

            // Verify that the draft lock renewer is running
            originalExpiresAt = DateTime.ParseExact(draft.DraftLock.ExpiresAt, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            while (true)
            {
                DateTime newExpiresAt = DateTime.ParseExact(draft.DraftLock.ExpiresAt, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                if (newExpiresAt != originalExpiresAt)
                {
                    break;
                }
                await Task.Delay(10);
            }

            // Cleanup
            draft.StopRenewer();
            draft.ReleaseLock();
        }

        [TestMethod]
        public async Task StopRenewerTest_NotEditable()
        {
            var testNumber = 20;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Try to stop the renewer for the (non-existent) draft lock (should not throw an exception)
            structureSetItem.StopRenewer();
        }

        [TestMethod]
        public async Task StopRenewerTest_Editable()
        {
            var testNumber = 21;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Set the lock renewal buffer to a large value for testing so that the lock is renewed quickly (expires_in will be 360000 ms)
            _proKnow.LockRenewalBuffer = 360;

            // Create a draft of that structure set
            var draft = await structureSetItem.DraftAsync();

            // Stop the renewer
            draft.StopRenewer();

            // Verify that the draft lock renewer is not running
            var originalExpiresAt = DateTime.ParseExact(draft.DraftLock.ExpiresAt, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            await Task.Delay(10);
            var currentExpiresAt = DateTime.ParseExact(draft.DraftLock.ExpiresAt, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            Assert.AreEqual(originalExpiresAt, currentExpiresAt);

            // Cleanup
            draft.ReleaseLock();
        }

        [TestMethod]
        public async Task IDisposableTest()
        {
            var testNumber = 22;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"));
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
