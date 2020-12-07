using Dicom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Patient;
using ProKnow.Patient.Entities;
using ProKnow.Test;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Upload.Test
{
    [TestClass]
    public class UploadsTest
    {
        private static readonly string _testClassName = nameof(UploadsTest);
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;

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
        }

        [TestMethod]
        public async Task UploadAsyncTest_WorkspaceName_FilePath()
        {
            int testNumber = 1;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Upload test file
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = patientItem.Mrn, Name = patientItem.Name }
            };
            await _proKnow.Uploads.UploadAsync(workspaceItem.Name, uploadPath, overrides);

            // Verify file was uploaded
            await patientItem.RefreshAsync();
            var entitySummaries = patientItem.FindEntities(t => true);
            Assert.AreEqual(1, entitySummaries.Count);
            Assert.AreEqual("2.16.840.1.114337.1.1.1535997926.0", entitySummaries[0].Uid);
        }

        [TestMethod]
        public async Task UploadAsyncTest_WorkspaceId_FolderPath()
        {
            int testNumber = 2;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Upload test folder
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = patientItem.Mrn, Name = patientItem.Name }
            };
            await _proKnow.Uploads.UploadAsync(workspaceItem.Id, uploadPath, overrides);

            // Verify files were uploaded
            await patientItem.RefreshAsync();
            var entitySummaries = patientItem.FindEntities(t => true);
            Assert.AreEqual(1, entitySummaries.Count);
            var entityItem = await entitySummaries[0].GetAsync() as ImageSetItem;
            Assert.AreEqual(5, entityItem.Data.Images.Count);
        }

        [TestMethod]
        public async Task UploadAsyncTest_WorkspaceItem()
        {
            int testNumber = 3;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Upload test folder
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = patientItem.Mrn, Name = patientItem.Name }
            };
            await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath, overrides);

            // Verify files were uploaded
            await patientItem.RefreshAsync();
            var entitySummaries = patientItem.FindEntities(t => true);
            Assert.AreEqual(1, entitySummaries.Count);
            var entityItem = await entitySummaries[0].GetAsync() as ImageSetItem;
            Assert.AreEqual(5, entityItem.Data.Images.Count);
        }

        [TestMethod]
        public async Task UploadAsyncTest_WorkspaceId_MultiplePaths()
        {
            int testNumber = 4;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient without data
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Upload test folder and test file
            var uploadPath1 = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var uploadPath2 = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RS.dcm");
            var uploadPaths = new List<string>() { uploadPath1, uploadPath2 };
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = patientItem.Mrn, Name = patientItem.Name }
            };
            await _proKnow.Uploads.UploadAsync(workspaceItem.Id, uploadPaths, overrides);

            // Verify test folder and test file were uploaded
            await patientItem.RefreshAsync();
            var entitySummaries = patientItem.FindEntities(t => true);
            Assert.AreEqual(2, entitySummaries.Count);
            var imageSetSummary = patientItem.FindEntities(t => t.Type == "image_set")[0];
            var entityItem = await imageSetSummary.GetAsync() as ImageSetItem;
            Assert.AreEqual(5, entityItem.Data.Images.Count);
        }

        [TestMethod]
        public async Task UploadAsyncTest_WorkspaceItem_MultiplePaths()
        {
            int testNumber = 5;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient without data
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Upload test folder and test file
            var uploadPath1 = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var uploadPath2 = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RS.dcm");
            var uploadPaths = new List<string>() { uploadPath1, uploadPath2 };
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = patientItem.Mrn, Name = patientItem.Name }
            };
            await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPaths, overrides);

            // Verify test folder and test file were uploaded
            await patientItem.RefreshAsync();
            var entitySummaries = patientItem.FindEntities(t => true);
            Assert.AreEqual(2, entitySummaries.Count);
            var imageSetSummary = patientItem.FindEntities(t => t.Type == "image_set")[0];
            var entityItem = await imageSetSummary.GetAsync() as ImageSetItem;
            Assert.AreEqual(5, entityItem.Data.Images.Count);
        }

        [TestMethod]
        public async Task UploadAsyncTest_UploadBatch()
        {
            int testNumber = 6;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload first test folder
            var uploadPath1 = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew");
            await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath1);

            // Upload second test folder
            var uploadPath2 = Path.Combine(TestSettings.TestDataRootDirectory, "Sro");
            var uploadBatch2 = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath2);

            // Verify the returned upload batch for the second test folder
            Assert.AreEqual(1, uploadBatch2.Patients.Count);
            Assert.AreEqual("0stCQd22vqX3RkoxNM0s332kJ", uploadBatch2.Patients[0].Mrn);
            Assert.AreEqual(2, uploadBatch2.Patients[0].Entities.Count);
            var ct = uploadBatch2.Patients[0].Entities.First(x => x.Modality == "CT");
            Assert.AreEqual("1.2.246.352.221.563569281719761951014104635106765053066", ct.Uid);
            var mr = uploadBatch2.Patients[0].Entities.First(x => x.Modality == "MR");
            Assert.AreEqual("1.2.246.352.221.470938394496317011513892701464452657827", mr.Uid);
            Assert.AreEqual(1, uploadBatch2.Patients[0].Sros.Count);
            Assert.AreEqual("1.2.246.352.221.52738008096457865345287404867971417272", uploadBatch2.Patients[0].Sros[0].Uid);
        }

        [Ignore] // Skip since this takes a minute to run
        [TestMethod]
        public async Task UploadAsyncTest_MultipleCalls()
        {
            int testNumber = 7;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Read a DICOM file to use as a template
            var templatePath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            var dicomFile = DicomFile.Open(templatePath, FileReadOption.ReadAll);
            var dicomDataset = dicomFile.Dataset;

            // Create a temporary file to hold data for each upload
            var tempPath = Path.GetTempFileName();

            // Upload more than 200 unique DICOM objects (maximum batch size of upload results returned by ProKnow)
            for (var i = 0; i < 205; i++)
            {
                dicomDataset.AddOrUpdate<string>(DicomTag.SOPInstanceUID, DicomUID.Generate().UID);
                dicomFile.Save(tempPath);
                await _proKnow.Uploads.UploadAsync(workspaceItem.Id, tempPath);
            }

            // Verify that all files were uploaded and processed, i.e., that query parameters were properly applied to page 
            // through upload results returned by ProKnow
            var patientSummaries = await _proKnow.Patients.QueryAsync(workspaceItem.Id);
            Assert.AreEqual(1, patientSummaries.Count);
            var patientItem = await patientSummaries[0].GetAsync();
            var entitySummaries = patientItem.FindEntities(t => true);
            Assert.AreEqual(205, entitySummaries.Count);

            // Delete temporary file
            File.Delete(tempPath);
        }

        [TestMethod]
        public async Task UploadAsyncTest_UploadBatch_DuplicateFiles()
        {
            int testNumber = 8;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload a file
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "DuplicateObjects", "RD.dcm");
            var uploadBatch1 = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath);

            // Verify "completed" status
            Assert.AreEqual(1, uploadBatch1.Patients.Count);
            Assert.AreEqual(1, uploadBatch1.Patients[0].Entities.Count);
            Assert.AreEqual("completed", uploadBatch1.GetStatus(uploadPath));

            // Upload the first file again (same path)
            var uploadBatch2 = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath);

            // Verify "completed" status, since the content wasn't uploaded again and the upload status of the previous upload (ID) was returned
            Assert.AreEqual(1, uploadBatch2.Patients.Count);
            Assert.AreEqual(1, uploadBatch2.Patients[0].Entities.Count);
            Assert.AreEqual("completed", uploadBatch2.GetStatus(uploadPath));
        }

        [TestMethod]
        public async Task UploadAsyncTest_UploadBatch_DuplicateObjects()
        {
            int testNumber = 9;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload a file
            var uploadPath1 = Path.Combine(TestSettings.TestDataRootDirectory, "DuplicateObjects", "RD.dcm");
            var uploadBatch1 = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath1);

            // Verify "completed" status
            Assert.AreEqual(1, uploadBatch1.Patients.Count);
            Assert.AreEqual(1, uploadBatch1.Patients[0].Entities.Count);
            Assert.AreEqual("completed", uploadBatch1.GetStatus(uploadPath1));

            // Upload another file (with a different path) that contains the same object
            var uploadPath2 = Path.Combine(TestSettings.TestDataRootDirectory, "DuplicateObjects", "RD - Copy.dcm");
            var uploadBatch2 = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath2);

            // Verify "completed" status, since the content wasn't uploaded again and the upload status of the previous upload (ID) was returned
            Assert.AreEqual(1, uploadBatch2.Patients.Count);
            Assert.AreEqual(1, uploadBatch2.Patients[0].Entities.Count);
            Assert.AreEqual("completed", uploadBatch2.GetStatus(uploadPath2));
        }
    }
}
