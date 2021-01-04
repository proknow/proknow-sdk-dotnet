using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Patient.Entities;
using ProKnow.Test;
using ProKnow.Upload;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Patient.Test
{
    [TestClass]
    public class PatientItemTest
    {
        private static readonly string _testClassName = nameof(PatientItemTest);
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

            // Delete custom metrics
            await TestHelper.DeleteCustomMetricsAsync(_testClassName);
        }

        [TestMethod]
        public async Task CreateStructureSetAsyncTest()
        {
            int testNumber = 1;

            // Create a workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient with only images
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "CT"));

            // Get the image set ID
            var imageSetSummary = patientItem.FindEntities(e => e.Type == "image_set")[0];

            // Create a structure set
            var structureSetSummary = await patientItem.CreateStructureSetAsync("Peaches", imageSetSummary.Id);

            // Verify that the structure set was created
            Assert.AreEqual("Peaches", (await structureSetSummary.GetAsync() as StructureSetItem).Data.Name);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            int testNumber = 2;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Verify patient is found
            Assert.IsNotNull(await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Mrn == patientItem.Mrn));

            // Delete the patient
            await patientItem.DeleteAsync();

            // Verify that the patient was deleted
            Assert.IsNull(await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Mrn == patientItem.Mrn));
        }

        [TestMethod]
        public async Task FindEntitiesTest()
        {
            int testNumber = 3;

            // Create a workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew");

            // Verify that entities are found at each of the four levels
            Assert.AreEqual(1, patientItem.FindEntities(e => e.Type == "image_set").Count);
            Assert.AreEqual(1, patientItem.FindEntities(e => e.Type == "structure_set").Count);
            Assert.AreEqual(1, patientItem.FindEntities(e => e.Type == "plan").Count);
            Assert.AreEqual(1, patientItem.FindEntities(e => e.Type == "dose").Count);
        }

        [TestMethod]
        public async Task FindSrosTest()
        {
            int testNumber = 4;

            // Create a workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Sro");

            // Verify that the SRO is found
            Assert.AreEqual(1, patientItem.FindSros(s => true).Count);
        }

        [TestMethod]
        public async Task GetMetadataAsyncTest()
        {
            int testNumber = 5;

            // Create custom metrics
            var enumCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-enum", "patient", "enum",
                new string[] { "one", "two", "three" });
            var numberCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-number", "patient", "number");
            var stringCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-string", "patient", "string");

            // Create a workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient with metadata
            var metadata = new Dictionary<string, object>
            {
                { enumCustomMetricItem.Name, "one" },
                { numberCustomMetricItem.Name, 1 },
                { stringCustomMetricItem.Name, "I" }
            };
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, null, null, null, metadata);

            // Get the resolved metadata
            var resolvedMetadata = await patientItem.GetMetadataAsync();

            // Verify the resolved metadata
            Assert.AreEqual(3, resolvedMetadata.Keys.Count);
            Assert.IsTrue(resolvedMetadata.ContainsKey(enumCustomMetricItem.Name));
            Assert.AreEqual("one", resolvedMetadata[enumCustomMetricItem.Name]);
            Assert.IsTrue(resolvedMetadata.ContainsKey(numberCustomMetricItem.Name));
            Assert.AreEqual(1, resolvedMetadata[numberCustomMetricItem.Name]);
            Assert.IsTrue(resolvedMetadata.ContainsKey(stringCustomMetricItem.Name));
            Assert.AreEqual("I", resolvedMetadata[stringCustomMetricItem.Name]);
        }

        [TestMethod]
        public async Task RefreshAsyncTest()
        {
            int testNumber = 6;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient with only images
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "CT"));
            Assert.AreEqual(1, patientItem.FindEntities(e => true).Count);

            // Upload a structure set for the same patient
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, Path.Combine("Becker^Matthew", "RS.dcm"));
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = patientItem.Mrn, Name = patientItem.Name }
            };
            var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath, overrides);
            await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);

            // Refresh the patient and verify that it now includes the structure set
            await patientItem.RefreshAsync();
            var entitySummaries = patientItem.FindEntities(e => true);
            Assert.AreEqual(2, entitySummaries.Count());
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            int testNumber = 7;

            // Create custom metrics
            var enumCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-enum", "patient", "enum",
                new string[] { "one", "two", "three" });
            var numberCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-number", "patient", "number");
            var stringCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-string", "patient", "string");

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient without metadata
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);
            Assert.AreEqual(0, patientItem.Metadata.Keys.Count);

            // Change/set the MRN, name, birth date, sex, and metadata
            patientItem.Mrn = $"{testNumber}-Mrn-A";
            patientItem.Name = $"{testNumber}-Name-A";
            patientItem.BirthDate = "1776-07-04";
            patientItem.Sex = "M";
            var metadata = new Dictionary<string, object>
            {
                { enumCustomMetricItem.Name, "one" },
                { numberCustomMetricItem.Name, 1 },
                { stringCustomMetricItem.Name, "I" }
            };
            await patientItem.SetMetadataAsync(metadata);

            // Save the changes
            await patientItem.SaveAsync();

            // Retrieve the patient and verify that the changes were saved
            var patientSummary = await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Id == patientItem.Id);
            var patientItem2 = await patientSummary.GetAsync();
            Assert.AreEqual($"{testNumber}-Mrn-A", patientItem2.Mrn);
            Assert.AreEqual($"{testNumber}-Name-A", patientItem2.Name);
            Assert.AreEqual("1776-07-04", patientItem2.BirthDate);
            Assert.AreEqual("M", patientItem2.Sex);
            Assert.AreEqual(3, patientItem2.Metadata.Keys.Count);
            Assert.IsTrue(patientItem2.Metadata.ContainsKey(enumCustomMetricItem.Id));
            Assert.AreEqual("one", patientItem2.Metadata[enumCustomMetricItem.Id]);
            Assert.IsTrue(patientItem2.Metadata.ContainsKey(numberCustomMetricItem.Id));
            Assert.AreEqual(1, patientItem2.Metadata[numberCustomMetricItem.Id]);
            Assert.IsTrue(patientItem2.Metadata.ContainsKey(stringCustomMetricItem.Id));
            Assert.AreEqual("I", patientItem2.Metadata[stringCustomMetricItem.Id]);
        }

        [TestMethod]
        public async Task SetMetadataAsyncTest()
        {
            int testNumber = 8;

            // Create custom metrics
            var enumCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-enum", "patient", "enum",
                new string[] { "one", "two", "three" });
            var numberCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-number", "patient", "number");
            var stringCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-string", "patient", "string");

            // Create a workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient without metadata
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);
            Assert.AreEqual(0, patientItem.Metadata.Keys.Count);

            // Set the metadata
            var metadata = new Dictionary<string, object>
            {
                { enumCustomMetricItem.Name, "one" },
                { numberCustomMetricItem.Name, 1 },
                { stringCustomMetricItem.Name, "I" }
            };
            await patientItem.SetMetadataAsync(metadata);

            // Verify the metadata property was set using resolved ProKnow IDs
            Assert.AreEqual(3, patientItem.Metadata.Keys.Count);
            Assert.IsTrue(patientItem.Metadata.ContainsKey(enumCustomMetricItem.Id));
            Assert.AreEqual("one", patientItem.Metadata[enumCustomMetricItem.Id]);
            Assert.IsTrue(patientItem.Metadata.ContainsKey(numberCustomMetricItem.Id));
            Assert.AreEqual(1, patientItem.Metadata[numberCustomMetricItem.Id]);
            Assert.IsTrue(patientItem.Metadata.ContainsKey(stringCustomMetricItem.Id));
            Assert.AreEqual("I", patientItem.Metadata[stringCustomMetricItem.Id]);
        }

        [TestMethod]
        public async Task UploadAsyncTest_FilePath()
        {
            int testNumber = 9;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient without data
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Upload test file
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = patientItem.Mrn, Name = patientItem.Name }
            };
            var uploadResults = await patientItem.UploadAsync(uploadPath, overrides);
            await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);

            // Verify test file was uploaded
            await patientItem.RefreshAsync();
            var entitySummaries = patientItem.FindEntities(t => true);
            Assert.AreEqual(1, entitySummaries.Count);
        }

        [TestMethod]
        public async Task UploadAsyncTest_FolderPath()
        {
            int testNumber = 10;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient without data
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Upload test folder
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = patientItem.Mrn, Name = patientItem.Name }
            };
            var uploadResults = await patientItem.UploadAsync(uploadPath, overrides);
            await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);

            // Verify test folder was uploaded
            await patientItem.RefreshAsync();
            var entitySummaries = patientItem.FindEntities(t => true);
            Assert.AreEqual(1, entitySummaries.Count);
            var entityItem = await entitySummaries[0].GetAsync() as ImageSetItem;
            Assert.AreEqual(5, entityItem.Data.Images.Count);
        }

        [TestMethod]
        public async Task UploadAsyncTest_MultiplePaths()
        {
            int testNumber = 11;

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
            var uploadResults = await patientItem.UploadAsync(uploadPaths, overrides);
            await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);

            // Verify test folder and test file were uploaded
            await patientItem.RefreshAsync();
            var entitySummaries = patientItem.FindEntities(t => true);
            Assert.AreEqual(2, entitySummaries.Count);
            var imageSetSummary = patientItem.FindEntities(t => t.Type == "image_set")[0];
            var entityItem = await imageSetSummary.GetAsync() as ImageSetItem;
            Assert.AreEqual(5, entityItem.Data.Images.Count);
            Assert.AreEqual(1, patientItem.FindEntities(t => t.Type == "structure_set").Count);
        }
    }
}
