using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Exceptions;
using ProKnow.Test;
using ProKnow.Upload;

namespace ProKnow.Patient.Entities.StructureSet.Test
{
    [TestClass]
    public class StructureSetVersionItemTest
    {
        private static readonly string _testClassName = nameof(StructureSetVersionItemTest);
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
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
        public async Task DeleteAsyncTest_InvalidOperationError()
        {
            var testNumber = 1;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew", 4);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Create a draft of the structure set
#pragma warning disable IDE0063 // Use simple 'using' statement
            using (var draft = await structureSetItem.DraftAsync())
#pragma warning restore IDE0063 // Use simple 'using' statement
            {
                // Get the version of the draft
                var versions = await draft.Versions.QueryAsync();
                var draftVersion = versions[0];

                // Try to delete the draft version and verify the exception
                try
                {
                    await draftVersion.DeleteAsync();
                    Assert.Fail();
                }
                catch (InvalidOperationError ex)
                {
                    Assert.AreEqual("Draft versions of structure sets cannot be deleted.", ex.Message);
                }
            }
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            var testNumber = 2;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew", 4);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Get the original version of that structure set and label it as such
            var versions = await structureSetItem.Versions.QueryAsync();
            var original = versions[0];
            original.Label = "original";
            await original.SaveAsync();

            // Create another version of that structure set, add an ROI, and commit the change
            using (var draft = await structureSetItem.DraftAsync())
            {
                await draft.CreateRoiAsync("thing1", Color.Magenta, "ORGAN");
                await draft.ApproveAsync("original + thing1");
            }
            await structureSetItem.RefreshAsync();

            // Create another version of that structure set, add another ROI, and commit the change
            using (var draft = await structureSetItem.DraftAsync())
            {
                await draft.CreateRoiAsync("thing2", Color.Azure, "ORGAN");
                await draft.ApproveAsync("original + thing1 + thing2");
            }
            await structureSetItem.RefreshAsync();

            // Verify there are three versions
            versions = await structureSetItem.Versions.QueryAsync();
            Assert.AreEqual(3, versions.Count);

            // Delete a version
            var version = versions.First(v => v.Label == "original + thing1");
            await version.DeleteAsync();

            // Verify the version was deleted
            versions = await structureSetItem.Versions.QueryAsync();
            Assert.AreEqual(2, versions.Count);
            Assert.IsTrue(versions.Any(v => v.Label == "original"));
            Assert.IsTrue(versions.Any(v => v.Label == "original + thing1 + thing2"));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_InvalidOperationError()
        {
            var testNumber = 3;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew", 4);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Create a draft of the structure set
#pragma warning disable IDE0063 // Use simple 'using' statement
            using (var draft = await structureSetItem.DraftAsync())
#pragma warning restore IDE0063 // Use simple 'using' statement
            {
                // Get the version of the draft
                var versions = await draft.Versions.QueryAsync();
                var draftVersion = versions[0];

                // Try to download the draft version and verify the exception
                try
                {
                    await draftVersion.DownloadAsync("doesn't matter");
                    Assert.Fail();
                }
                catch (InvalidOperationError ex)
                {
                    Assert.AreEqual("Draft versions of structure sets cannot be downloaded.", ex.Message);
                }
            }
        }

        [TestMethod]
        public async Task DownloadAsyncTest_Directory()
        {
            var testNumber = 4;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew", 4);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Create another version of that structure set, add an ROI, and commit the change
            using (var draft = await structureSetItem.DraftAsync())
            {
                await draft.CreateRoiAsync("thing1", Color.Magenta, "ORGAN");
                await draft.ApproveAsync("original + thing1");
            }

            // Get that version
            var versions = await structureSetItem.Versions.QueryAsync();
            var version = versions.First(v => v.Label == "original + thing1");

            // Download the version to an existing directory using the default filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            Directory.CreateDirectory(downloadFolder);
            string expectedDownloadPath = Path.Combine(downloadFolder, $"RS.{version.VersionId}.dcm");
            string actualDownloadPath = await version.DownloadAsync(downloadFolder);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Upload it to a new patient and wait for it to be processed
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = $"{patientItem.Mrn}2", Name = $"{patientItem.Name}2" }
            };
            await _proKnow.Uploads.UploadAsync(workspaceItem.Id, actualDownloadPath, overrides);

            // Get the uploaded structure set
            var patientSummary2 = await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Mrn == $"{patientItem.Mrn}2");
            var patientItem2 = await patientSummary2.GetAsync();
            var entitySummary2 = patientItem2.FindEntities(e => true)[0];
            var structureSetItem2 = (await entitySummary2.GetAsync()) as StructureSetItem;

            // Verify that the version downloaded (and uploaded to a new patient) was correct
            Assert.IsTrue(structureSetItem2.Rois.Any(r => r.Name == "thing1"));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_ExistingFileWithExistingParent()
        {
            var testNumber = 5;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew", 4);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Create another version of that structure set, add an ROI, and commit the change
            using (var draft = await structureSetItem.DraftAsync())
            {
                await draft.CreateRoiAsync("thing1", Color.Magenta, "ORGAN");
                await draft.ApproveAsync("original + thing1");
            }

            // Get that version
            var versions = await structureSetItem.Versions.QueryAsync();
            var version = versions.First(v => v.Label == "original + thing1");

            // Download the version to an existing filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            Directory.CreateDirectory(downloadFolder);
            string expectedDownloadPath = Path.Combine(downloadFolder, "RS.dcm");
            File.WriteAllText(expectedDownloadPath, "This is an existing file!");
            string actualDownloadPath = await version.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Upload it to a new patient and wait for it to be processed
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = $"{patientItem.Mrn}2", Name = $"{patientItem.Name}2" }
            };
            await _proKnow.Uploads.UploadAsync(workspaceItem.Id, actualDownloadPath, overrides);

            // Get the uploaded structure set
            var patientSummary2 = await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Mrn == $"{patientItem.Mrn}2");
            var patientItem2 = await patientSummary2.GetAsync();
            var entitySummary2 = patientItem2.FindEntities(e => true)[0];
            var structureSetItem2 = (await entitySummary2.GetAsync()) as StructureSetItem;

            // Verify that the version downloaded (and uploaded to a new patient) was correct
            Assert.IsTrue(structureSetItem2.Rois.Any(r => r.Name == "thing1"));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_NewFileWithExistingParent()
        {
            var testNumber = 6;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew", 4);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Create another version of that structure set, add an ROI, and commit the change
            using (var draft = await structureSetItem.DraftAsync())
            {
                await draft.CreateRoiAsync("thing1", Color.Magenta, "ORGAN");
                await draft.ApproveAsync("original + thing1");
            }

            // Get that version
            var versions = await structureSetItem.Versions.QueryAsync();
            var version = versions.First(v => v.Label == "original + thing1");

            // Download the version to an existing directory using a specified filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            string expectedDownloadPath = Path.Combine(downloadFolder, "RS.dcm");
            string actualDownloadPath = await version.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Upload it to a new patient and wait for it to be processed
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = $"{patientItem.Mrn}2", Name = $"{patientItem.Name}2" }
            };
            await _proKnow.Uploads.UploadAsync(workspaceItem.Id, actualDownloadPath, overrides);

            // Get the uploaded structure set
            var patientSummary2 = await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Mrn == $"{patientItem.Mrn}2");
            var patientItem2 = await patientSummary2.GetAsync();
            var entitySummary2 = patientItem2.FindEntities(e => true)[0];
            var structureSetItem2 = (await entitySummary2.GetAsync()) as StructureSetItem;

            // Verify that the version downloaded (and uploaded to a new patient) was correct
            Assert.IsTrue(structureSetItem2.Rois.Any(r => r.Name == "thing1"));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_NewFileWithNonexistingParent()
        {
            var testNumber = 7;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew", 4);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Create another version of that structure set, add an ROI, and commit the change
            using (var draft = await structureSetItem.DraftAsync())
            {
                await draft.CreateRoiAsync("thing1", Color.Magenta, "ORGAN");
                await draft.ApproveAsync("original + thing1");
            }

            // Get that version
            var versions = await structureSetItem.Versions.QueryAsync();
            var version = versions.First(v => v.Label == "original + thing1");

            // Download the entity to an nonexisting directory using a specified filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            string expectedDownloadPath = Path.Combine(downloadFolder, "grandparent", "parent", "RS.dcm");
            string actualDownloadPath = await version.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Upload it to a new patient and wait for it to be processed
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = $"{patientItem.Mrn}2", Name = $"{patientItem.Name}2" }
            };
            await _proKnow.Uploads.UploadAsync(workspaceItem.Id, actualDownloadPath, overrides);

            // Get the uploaded structure set
            var patientSummary2 = await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Mrn == $"{patientItem.Mrn}2");
            var patientItem2 = await patientSummary2.GetAsync();
            var entitySummary2 = patientItem2.FindEntities(e => true)[0];
            var structureSetItem2 = (await entitySummary2.GetAsync()) as StructureSetItem;

            // Verify that the version downloaded (and uploaded to a new patient) was correct
            Assert.IsTrue(structureSetItem2.Rois.Any(r => r.Name == "thing1"));
        }

        [TestMethod]
        public async Task GetAsyncTest_Draft()
        {
            var testNumber = 8;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew", 4);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Create a draft of that structure set
#pragma warning disable IDE0063 // Use simple 'using' statement
            using (var draft = await structureSetItem.DraftAsync())
#pragma warning restore IDE0063 // Use simple 'using' statement
            {
                // Get the version of that draft
                var versions = await draft.Versions.QueryAsync();
                var version = versions[0];

                // Get the corresponding structure set item
                var draftStructureSetItem = await version.GetAsync();

                // Verify that the expected structure set item was returned
                Assert.AreEqual(structureSetItem.Id, draftStructureSetItem.Id);
                Assert.AreNotEqual(structureSetItem.Uid, draftStructureSetItem.Uid);
                Assert.IsTrue(draftStructureSetItem.IsDraft);
            }
        }

        [TestMethod]
        public async Task GetAsyncTest_NotDraft()
        {
            var testNumber = 9;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew", 4);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Get the original version of that structure set and label it as such
            var versions = await structureSetItem.Versions.QueryAsync();
            var original = versions[0];
            original.Label = "original";
            await original.SaveAsync();
            var originalRoiCount = structureSetItem.Rois.Length;

            // Create another version of that structure set, add an ROI, and commit the change
            using (var draft = await structureSetItem.DraftAsync())
            {
                await draft.CreateRoiAsync("thing1", Color.Magenta, "ORGAN");
                await draft.ApproveAsync("original + thing1");
            }
            await structureSetItem.RefreshAsync();

            // Create another version of that structure set, add another ROI, and commit the change
            using (var draft = await structureSetItem.DraftAsync())
            {
                await draft.CreateRoiAsync("thing2", Color.Azure, "ORGAN");
                await draft.ApproveAsync("original + thing1 + thing2");
            }
            await structureSetItem.RefreshAsync();

            // Get all of the versions of the structure set
            versions = await structureSetItem.Versions.QueryAsync();

            // Get and verify the corresponding structure set items
            foreach (var version in versions)
            {
                var versionStructureSetItem = await version.GetAsync();
                if (version.Label == "original")
                {
                    Assert.AreEqual(originalRoiCount, versionStructureSetItem.Rois.Length);
                }
                else if (version.Label == "original + thing1")
                {
                    Assert.AreEqual(originalRoiCount + 1, versionStructureSetItem.Rois.Length);
                    Assert.IsTrue(versionStructureSetItem.Rois.Any(r => r.Name == "thing1"));
                }
                else if (version.Label == "original + thing1 + thing2")
                {
                    Assert.AreEqual(originalRoiCount + 2, versionStructureSetItem.Rois.Length);
                    Assert.IsTrue(versionStructureSetItem.Rois.Any(r => r.Name == "thing1"));
                    Assert.IsTrue(versionStructureSetItem.Rois.Any(r => r.Name == "thing2"));
                }
                else
                {
                    Assert.Fail("unexpected version");
                }
            }
        }

        [TestMethod]
        public async Task RevertAsyncTest_InvalidOperationError()
        {
            var testNumber = 10;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew", 4);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Create a draft of that structure set
#pragma warning disable IDE0063 // Use simple 'using' statement
            using (var draft = await structureSetItem.DraftAsync())
#pragma warning restore IDE0063 // Use simple 'using' statement
            {
                // Get the version of the draft
                var versions = await draft.Versions.QueryAsync();
                var draftVersion = versions[0];

                // Try to revert the structure set to the draft version and verify the exception
                try
                {
                    await draftVersion.RevertAsync();
                    Assert.Fail();
                }
                catch (InvalidOperationError ex)
                {
                    Assert.AreEqual("Structure sets cannot be reverted to draft versions.", ex.Message);
                }
            }
        }

        [TestMethod]
        public async Task RevertAsyncTest_NotDraft()
        {
            var testNumber = 11;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew", 4);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;
            var originalRoiCount = structureSetItem.Rois.Length;

            // Create another version of that structure set, add an ROI, and commit the change
            using (var draft = await structureSetItem.DraftAsync())
            {
                await draft.CreateRoiAsync("thing1", Color.Magenta, "ORGAN");
                await draft.ApproveAsync("original + thing1");
            }
            await structureSetItem.RefreshAsync();

            // Create another version of that structure set, add another ROI, and commit the change
            using (var draft = await structureSetItem.DraftAsync())
            {
                await draft.CreateRoiAsync("thing2", Color.Azure, "ORGAN");
                await draft.ApproveAsync("original + thing1 + thing2");
            }
            await structureSetItem.RefreshAsync();

            // Get the versions
            var versions = await structureSetItem.Versions.QueryAsync();

            // Revert the structure set to the second version
            var version = versions.First(v => v.Label == "original + thing1");
            var revertedStructureSetItem = await version.RevertAsync();

            // Verify the returned structure set item
            Assert.AreEqual(originalRoiCount + 1, revertedStructureSetItem.Rois.Length);
            Assert.IsTrue(revertedStructureSetItem.Rois.Any(r => r.Name == "thing1"));

            // Verify the structure set was reverted to the second version
            await structureSetItem.RefreshAsync();
            Assert.AreEqual(originalRoiCount + 1, structureSetItem.Rois.Length);
            Assert.IsTrue(structureSetItem.Rois.Any(r => r.Name == "thing1"));
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            var testNumber = 12;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set and structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew", 4);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Get the version of this structure set and verify is has no label or message
            var versions = await structureSetItem.Versions.QueryAsync();
            Assert.AreEqual(1, versions.Count);
            Assert.IsNull(versions[0].Label);
            Assert.IsNull(versions[0].Message);

            // Add a label and message and save it
            versions[0].Label = "original";
            versions[0].Message = "This is the original version.";
            await versions[0].SaveAsync();

            // Verify that the label and message were saved
            versions = await structureSetItem.Versions.QueryAsync();
            Assert.AreEqual(1, versions.Count);
            Assert.AreEqual("original", versions[0].Label);
            Assert.AreEqual("This is the original version.", versions[0].Message);
        }
    }
}
