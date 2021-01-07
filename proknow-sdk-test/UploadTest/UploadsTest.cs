using Dicom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Patient;
using ProKnow.Patient.Entities;
using ProKnow.Test;
using System;
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
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath, overrides);

            // Wait for processing
            await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);

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
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath, overrides);

            // Wait for processing
            await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);

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
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath, overrides);

            // Wait for processing
            await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);

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
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPaths, overrides);

            // Wait for processing
            await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);

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
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPaths, overrides);

            // Wait for processing
            await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);

            // Verify test folder and test file were uploaded
            await patientItem.RefreshAsync();
            var entitySummaries = patientItem.FindEntities(t => true);
            Assert.AreEqual(2, entitySummaries.Count);
            var imageSetSummary = patientItem.FindEntities(t => t.Type == "image_set")[0];
            var entityItem = await imageSetSummary.GetAsync() as ImageSetItem;
            Assert.AreEqual(5, entityItem.Data.Images.Count);
        }

        [TestMethod]
        public async Task UploadAsyncTest_ReturnedResults()
        {
            int testNumber = 6;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload test folder
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro");
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath);

            // Verify the returned upload results
            Assert.AreEqual(3, uploadResults.Count);
            var reg = uploadResults.First(t => t.Path == Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "reg.dcm"));
            var ct = uploadResults.First(t => t.Path == Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "ct.dcm"));
            var mr = uploadResults.First(t => t.Path == Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "mr.dcm"));
            Assert.AreEqual("uploading", reg.Status);
            Assert.AreEqual("uploading", ct.Status);
            Assert.AreEqual("uploading", mr.Status);
        }

        [TestMethod]
        public async Task GetUploadProcessingResultsAsyncTest()
        {
            int testNumber = 7;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload test folder and get upload results
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Sro");
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath);
            var regUploadResult = uploadResults.First(t => t.Path == Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "reg.dcm"));
            var ctUploadResult = uploadResults.First(t => t.Path == Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "ct.dcm"));
            var mrUploadResult = uploadResults.First(t => t.Path == Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "mr.dcm"));

            // Wait for processing and get upload processing results
            var uploadProcessingResults = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);

            // Verify the returned upload processing results
            Assert.AreEqual(3, uploadProcessingResults.Results.Count);

            var regProcessingResult = uploadProcessingResults.Results.First(t => t.Id == regUploadResult.Id);
            Assert.AreEqual(regUploadResult.Path, regProcessingResult.Path);
            Assert.AreEqual("completed", regProcessingResult.Status);
            Assert.AreEqual("0stCQd22vqX3RkoxNM0s332kJ", regProcessingResult.Patient.Mrn);
            Assert.AreEqual("CTMRregforPROknow", regProcessingResult.Patient.Name);
            Assert.AreEqual("1.2.246.352.221.533224884342370113612258208604561177759", regProcessingResult.Study.Uid);
            Assert.IsTrue(String.IsNullOrEmpty(regProcessingResult.Study.Name));
            Assert.AreEqual("1.2.246.352.221.52738008096457865345287404867971417272", regProcessingResult.Sro.Uid);

            var ctProcessingResult = uploadProcessingResults.Results.First(t => t.Id == ctUploadResult.Id);
            Assert.AreEqual(ctUploadResult.Path, ctProcessingResult.Path);
            Assert.AreEqual("completed", ctProcessingResult.Status);
            Assert.AreEqual("0stCQd22vqX3RkoxNM0s332kJ", ctProcessingResult.Patient.Mrn);
            Assert.AreEqual("CTMRregforPROknow", ctProcessingResult.Patient.Name);
            Assert.AreEqual("1.2.246.352.221.533224884342370113612258208604561177759", ctProcessingResult.Study.Uid);
            Assert.IsTrue(String.IsNullOrEmpty(ctProcessingResult.Study.Name));
            Assert.AreEqual("1.2.246.352.221.563569281719761951014104635106765053066", ctProcessingResult.Entity.Uid);
            Assert.AreEqual("image_set", ctProcessingResult.Entity.Type);
            Assert.AreEqual("CT", ctProcessingResult.Entity.Modality);
            Assert.IsTrue(String.IsNullOrEmpty(ctProcessingResult.Entity.Description));

            var mrProcessingResult = uploadProcessingResults.Results.First(t => t.Id == mrUploadResult.Id);
            Assert.AreEqual(mrUploadResult.Path, mrProcessingResult.Path);
            Assert.AreEqual("completed", mrProcessingResult.Status);
            Assert.AreEqual("0stCQd22vqX3RkoxNM0s332kJ", mrProcessingResult.Patient.Mrn);
            Assert.AreEqual("CTMRregforPROknow", mrProcessingResult.Patient.Name);
            Assert.AreEqual("1.2.246.352.221.4769506379372726428577641853787688121", mrProcessingResult.Study.Uid);
            Assert.IsTrue(String.IsNullOrEmpty(mrProcessingResult.Study.Name));
            Assert.AreEqual("1.2.246.352.221.470938394496317011513892701464452657827", mrProcessingResult.Entity.Uid);
            Assert.AreEqual("image_set", mrProcessingResult.Entity.Type);
            Assert.AreEqual("MR", mrProcessingResult.Entity.Modality);
            Assert.IsTrue(String.IsNullOrEmpty(mrProcessingResult.Entity.Description));

            Assert.IsFalse(uploadProcessingResults.WereRetryDelaysExhausted);
            Assert.AreEqual(30000, uploadProcessingResults.TotalRetryDelayInMsec);
        }

        [TestMethod]
        public async Task GetUploadProcessingResultsAsyncTest_MultipleUploads()
        {
            int testNumber = 8;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload first test folder
            var uploadPath1 = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew");
            var uploadResults1 = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath1);

            // Upload second test folder
            var uploadPath2 = Path.Combine(TestSettings.TestDataRootDirectory, "Sro");
            var uploadResults2 = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath2);

            // Get upload processing results for second folder
            var uploadProcessingResults2 = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults2);

            // Verify that upload processing results were returned for the second test folder
            Assert.AreEqual(3, uploadProcessingResults2.Results.Count);
            Assert.IsTrue(uploadProcessingResults2.Results.Any(t => t.Path == Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "reg.dcm")));
            Assert.IsTrue(uploadProcessingResults2.Results.Any(t => t.Path == Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "ct.dcm")));
            Assert.IsTrue(uploadProcessingResults2.Results.Any(t => t.Path == Path.Combine(TestSettings.TestDataRootDirectory, "Sro", "mr.dcm")));

            Assert.IsFalse(uploadProcessingResults2.WereRetryDelaysExhausted);
            Assert.AreEqual(30000, uploadProcessingResults2.TotalRetryDelayInMsec);
        }

        [TestMethod]
        public async Task GetUploadProcessingResultsAsyncTest_Paging()
        {
            int testNumber = 9;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Read a DICOM file to use as a template
            var templatePath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            var dicomFile = DicomFile.Open(templatePath, FileReadOption.ReadAll);
            var dicomDataset = dicomFile.Dataset;

            // Upload more than 200 unique DICOM objects (maximum batch size of upload results returned by ProKnow)
            var allUploadResults = new List<UploadResult>();
            for (var i = 0; i < 205; i++)
            {
                dicomDataset.AddOrUpdate<string>(DicomTag.SOPInstanceUID, DicomUID.Generate().UID);
                var tempPath = Path.GetTempFileName();
                dicomFile.Save(tempPath);
                var thisUploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, tempPath);
                allUploadResults.AddRange(thisUploadResults);
                File.Delete(tempPath);
            }

            // Wait for processing and get processing results
            var uploadProcessingResults = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, allUploadResults);

            // Verify processing results were successfully retrieved for all uploaded files, i.e., that query parameters were
            // properly applied to page through upload results returned by ProKnow
            Assert.AreEqual(205, uploadProcessingResults.Results.Count);
            Assert.AreEqual(0, uploadProcessingResults.Results.Count(t => t.Status != "completed"));

            Assert.IsFalse(uploadProcessingResults.WereRetryDelaysExhausted);
            Assert.AreEqual(30000, uploadProcessingResults.TotalRetryDelayInMsec);
        }

        [TestMethod]
        public async Task GetUploadProcessingResultsAsyncTest_ExhaustRetries()
        {
            int testNumber = 10;

            // Create a ProKnowApi with one small retry delay
            var proKnow = new ProKnowApi(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            proKnow.Uploads.RetryDelays = new int[] { 1 }.ToList();

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Read a DICOM file to use as a template
            var templatePath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            var dicomFile = DicomFile.Open(templatePath, FileReadOption.ReadAll);
            var dicomDataset = dicomFile.Dataset;

            // Upload enough DICOM objects so that some reach terminal status and others don't
            var allUploadResults = new List<UploadResult>();
            for (var i = 0; i < 20; i++)
            {
                dicomDataset.AddOrUpdate<string>(DicomTag.SOPInstanceUID, DicomUID.Generate().UID);
                var tempPath = Path.GetTempFileName();
                dicomFile.Save(tempPath);
                var thisUploadResults = await proKnow.Uploads.UploadAsync(workspaceItem, tempPath);
                allUploadResults.AddRange(thisUploadResults);
                File.Delete(tempPath);
            }

            // Wait for processing and get processing results
            var uploadProcessingResults = await proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, allUploadResults);

            // Verify processing results were successfully retrieved for all uploaded files, i.e., that query parameters were
            // properly applied to page through upload results returned by ProKnow
            Assert.AreEqual(20, uploadProcessingResults.Results.Count);
            Assert.IsTrue(uploadProcessingResults.Results.Any(t => t.Status == "completed"));
            Assert.IsTrue(uploadProcessingResults.Results.Any(t => t.Status == "processing"));

            Assert.IsTrue(uploadProcessingResults.WereRetryDelaysExhausted);
            Assert.AreEqual(1, uploadProcessingResults.TotalRetryDelayInMsec);
        }

        [TestMethod]
        public async Task UploadAsyncTest_DuplicateFiles()
        {
            int testNumber = 11;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload a file and wait for it to be processed
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "DuplicateObjects", "RD.dcm");
            var uploadResults1 = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath);
            await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults1);

            // Upload the first file again (same path)
            var uploadResults2 = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath);

            // Verify that the upload result shows that it has already been uploaded
            Assert.AreEqual(1, uploadResults2.Count);
            Assert.AreEqual("completed", uploadResults2[0].Status);

            // Wait for processing anyway and get the upload processing results
            var uploadProcessingResults2 = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults2);

            // Verify the returned upload processing results
            Assert.AreEqual(1, uploadProcessingResults2.Results.Count);
            Assert.AreEqual(uploadResults2[0].Id, uploadProcessingResults2.Results[0].Id);
            Assert.AreEqual(uploadResults2[0].Path, uploadProcessingResults2.Results[0].Path);
            Assert.AreEqual("completed", uploadProcessingResults2.Results[0].Status);
            Assert.AreEqual("HNC-0522c0009", uploadProcessingResults2.Results[0].Patient.Mrn);
            Assert.AreEqual("Becker^Matthew", uploadProcessingResults2.Results[0].Patient.Name);
            Assert.AreEqual("1.3.6.1.4.1.22213.2.26558", uploadProcessingResults2.Results[0].Study.Uid);
            Assert.AreEqual("522", uploadProcessingResults2.Results[0].Study.Name);
            Assert.AreEqual("2.16.840.1.114337.1.1.1535997926.2", uploadProcessingResults2.Results[0].Entity.Uid);
            Assert.AreEqual("dose", uploadProcessingResults2.Results[0].Entity.Type);
            Assert.AreEqual("RTDOSE", uploadProcessingResults2.Results[0].Entity.Modality);
            Assert.IsTrue(String.IsNullOrEmpty(uploadProcessingResults2.Results[0].Entity.Description));

            Assert.IsFalse(uploadProcessingResults2.WereRetryDelaysExhausted);
            Assert.AreEqual(30000, uploadProcessingResults2.TotalRetryDelayInMsec);
        }

        [TestMethod]
        public async Task UploadAsyncTest_DuplicateObjects()
        {
            int testNumber = 12;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload a file and wait for it to be processed
            var uploadPath1 = Path.Combine(TestSettings.TestDataRootDirectory, "DuplicateObjects", "RD.dcm");
            var uploadResults1 = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath1);
            await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults1);

            // Upload another file (with a different path) that contains the same object
            var uploadPath2 = Path.Combine(TestSettings.TestDataRootDirectory, "DuplicateObjects", "RD - Copy.dcm");
            var uploadResults2 = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath2);

            // Verify that the upload result shows that it has already been uploaded
            Assert.AreEqual(1, uploadResults2.Count);
            Assert.AreEqual("completed", uploadResults2[0].Status);

            // Wait for processing anyway and get the upload processing results
            var uploadProcessingResults2 = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults2);

            // Verify the returned upload processing results, including that the new rather than original file name is returned
            Assert.AreEqual(1, uploadProcessingResults2.Results.Count);
            Assert.AreEqual(uploadResults2[0].Id, uploadProcessingResults2.Results[0].Id);
            Assert.AreEqual(uploadPath2, uploadProcessingResults2.Results[0].Path);
            Assert.AreEqual("completed", uploadProcessingResults2.Results[0].Status);
            Assert.AreEqual("HNC-0522c0009", uploadProcessingResults2.Results[0].Patient.Mrn);
            Assert.AreEqual("Becker^Matthew", uploadProcessingResults2.Results[0].Patient.Name);
            Assert.AreEqual("1.3.6.1.4.1.22213.2.26558", uploadProcessingResults2.Results[0].Study.Uid);
            Assert.AreEqual("522", uploadProcessingResults2.Results[0].Study.Name);
            Assert.AreEqual("2.16.840.1.114337.1.1.1535997926.2", uploadProcessingResults2.Results[0].Entity.Uid);
            Assert.AreEqual("dose", uploadProcessingResults2.Results[0].Entity.Type);
            Assert.AreEqual("RTDOSE", uploadProcessingResults2.Results[0].Entity.Modality);
            Assert.IsTrue(String.IsNullOrEmpty(uploadProcessingResults2.Results[0].Entity.Description));

            Assert.IsFalse(uploadProcessingResults2.WereRetryDelaysExhausted);
            Assert.AreEqual(30000, uploadProcessingResults2.TotalRetryDelayInMsec);
        }

        [TestMethod]
        public async Task UploadAsyncTest_LongProcessingTimes()
        {
            int testNumber = 13;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Read a DICOM file to use as a template
            var templatePath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            var dicomFile = DicomFile.Open(templatePath, FileReadOption.ReadAll);
            var dicomDataset = dicomFile.Dataset;

            // Upload more than 200 unique DICOM objects (maximum batch size of upload results returned by ProKnow)
            var allUploadResults = new List<UploadResult>();
            for (var i = 0; i < 205; i++)
            {
                dicomDataset.AddOrUpdate<string>(DicomTag.SOPInstanceUID, DicomUID.Generate().UID);
                var tempPath = Path.GetTempFileName();
                dicomFile.Save(tempPath);
                allUploadResults.AddRange(await _proKnow.Uploads.UploadAsync(workspaceItem, tempPath));
                File.Delete(tempPath);
            }

            // Upload another DICOM object that will take a long time to process
            var path = Path.Combine(TestSettings.TestDataRootDirectory, "StructureSet", "RS.Large.dcm");
            allUploadResults.AddRange(await _proKnow.Uploads.UploadAsync(workspaceItem, path));

            // Upload one more DICOM object that won't take a long time to process
            path = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            allUploadResults.AddRange(await _proKnow.Uploads.UploadAsync(workspaceItem, path));

            // Wait for processing and get processing results
            var uploadProcessingResults = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, allUploadResults);

            // Verify processing results were successfully retrieved for all uploaded files, i.e., that query parameters were
            // properly applied to page through upload results returned by ProKnow
            Assert.AreEqual(207, uploadProcessingResults.Results.Count);
            Assert.AreEqual(0, uploadProcessingResults.Results.Count(t => t.Status != "completed"));

            Assert.IsFalse(uploadProcessingResults.WereRetryDelaysExhausted);
            Assert.AreEqual(30000, uploadProcessingResults.TotalRetryDelayInMsec);
        }

        [TestMethod]
        public async Task UploadAsyncTest_LargeFile()
        {
            int testNumber = 14;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Upload a large file and wait for it to be processed
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Dose", "RD.Large.dcm");
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath);
            var uploadProcessingResults = await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);

            // Verify the returned upload processing results
            Assert.AreEqual(1, uploadProcessingResults.Results.Count);
            Assert.AreEqual(uploadResults[0].Id, uploadProcessingResults.Results[0].Id);
            Assert.AreEqual(uploadResults[0].Path, uploadProcessingResults.Results[0].Path);
            Assert.AreEqual("completed", uploadProcessingResults.Results[0].Status);
            Assert.AreEqual("OFHMKeeLRs58TLb5MxR3CvqVq", uploadProcessingResults.Results[0].Patient.Mrn);
            Assert.AreEqual("pZNqUhhwdfVnxjXu", uploadProcessingResults.Results[0].Patient.Name);
            Assert.AreEqual("1.2.246.352.221.48272622262322454668259763899214074244", uploadProcessingResults.Results[0].Study.Uid);
            Assert.IsTrue(String.IsNullOrEmpty(uploadProcessingResults.Results[0].Study.Name));
            Assert.AreEqual("1.2.246.352.221.50664457847203972258548924729170517675", uploadProcessingResults.Results[0].Entity.Uid);
            Assert.AreEqual("dose", uploadProcessingResults.Results[0].Entity.Type);
            Assert.AreEqual("RTDOSE", uploadProcessingResults.Results[0].Entity.Modality);
            Assert.IsTrue(String.IsNullOrEmpty(uploadProcessingResults.Results[0].Entity.Description));

            Assert.IsFalse(uploadProcessingResults.WereRetryDelaysExhausted);
            Assert.AreEqual(30000, uploadProcessingResults.TotalRetryDelayInMsec);
        }
    }
}
