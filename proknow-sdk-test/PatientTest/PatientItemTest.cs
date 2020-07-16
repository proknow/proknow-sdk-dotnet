using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Patient.Entities;
using ProKnow.Scorecard;
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
        private static readonly Uploads _uploads = _proKnow.Uploads;
        private static CustomMetricItem _enumCustomMetricItem;
        private static CustomMetricItem _numberCustomMetricItem;
        private static CustomMetricItem _stringCustomMetricItem;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete existing custom metrics, if necessary
            await TestHelper.DeleteCustomMetricsAsync(_testClassName);

            // Create custom metrics for testing
            _enumCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-enum", "patient", "enum",
                new string[] { "one", "two", "three" });
            _numberCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-number", "patient", "number");
            _stringCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-string", "patient", "string");

            // Delete test workspaces, if necessary
            await TestHelper.DeleteWorkspacesAsync(_testClassName);
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
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient with only images
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "CT"), 1);

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
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Becker^Matthew", 4);

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
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Sro", 3);

            // Verify that the SRO is found
            Assert.AreEqual(1, patientItem.FindSros(s => true).Count);
        }

        [TestMethod]
        public async Task GetMetadataAsyncTest()
        {
            int testNumber = 5;

            // Create a workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient with metadata
            var metadata = new Dictionary<string, object>
            {
                { _enumCustomMetricItem.Name, "one" },
                { _numberCustomMetricItem.Name, 1 },
                { _stringCustomMetricItem.Name, "I" }
            };
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, null, 0, null, null, metadata);

            // Get the resolved metadata
            var resolvedMetadata = await patientItem.GetMetadataAsync();

            // Verify the resolved metadata
            Assert.AreEqual(3, resolvedMetadata.Keys.Count);
            Assert.IsTrue(resolvedMetadata.ContainsKey(_enumCustomMetricItem.Name));
            Assert.AreEqual("one", resolvedMetadata[_enumCustomMetricItem.Name]);
            Assert.IsTrue(resolvedMetadata.ContainsKey(_numberCustomMetricItem.Name));
            Assert.AreEqual(1, resolvedMetadata[_numberCustomMetricItem.Name]);
            Assert.IsTrue(resolvedMetadata.ContainsKey(_stringCustomMetricItem.Name));
            Assert.AreEqual("I", resolvedMetadata[_stringCustomMetricItem.Name]);
        }

        [TestMethod]
        public async Task RefreshAsyncTest()
        {
            int testNumber = 6;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient with only images
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "CT"), 1);
            Assert.AreEqual(1, patientItem.FindEntities(e => true).Count);

            // Upload a structure set for the same patient
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, Path.Combine("Becker^Matthew", "RS.dcm"));
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Mrn = patientItem.Mrn, Name = patientItem.Name }
            };
            await _proKnow.Uploads.UploadAsync(workspaceItem.Id, uploadPath, overrides);

            // Refresh the patient and verify that it now includes the structure set
            await patientItem.RefreshAsync();
            var entitySummaries = patientItem.FindEntities(e => true);
            Assert.AreEqual(2, entitySummaries.Count());
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            int testNumber = 7;

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
                { _enumCustomMetricItem.Name, "one" },
                { _numberCustomMetricItem.Name, 1 },
                { _stringCustomMetricItem.Name, "I" }
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
            Assert.IsTrue(patientItem2.Metadata.ContainsKey(_enumCustomMetricItem.Id));
            Assert.AreEqual("one", patientItem2.Metadata[_enumCustomMetricItem.Id]);
            Assert.IsTrue(patientItem2.Metadata.ContainsKey(_numberCustomMetricItem.Id));
            Assert.AreEqual(1, patientItem2.Metadata[_numberCustomMetricItem.Id]);
            Assert.IsTrue(patientItem2.Metadata.ContainsKey(_stringCustomMetricItem.Id));
            Assert.AreEqual("I", patientItem2.Metadata[_stringCustomMetricItem.Id]);
        }

        [TestMethod]
        public async Task SetMetadataAsyncTest()
        {
            int testNumber = 8;

            // Create a workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient without metadata
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);
            Assert.AreEqual(0, patientItem.Metadata.Keys.Count);

            // Set the metadata
            var metadata = new Dictionary<string, object>
            {
                { _enumCustomMetricItem.Name, "one" },
                { _numberCustomMetricItem.Name, 1 },
                { _stringCustomMetricItem.Name, "I" }
            };
            await patientItem.SetMetadataAsync(metadata);

            // Verify the metadata property was set using resolved ProKnow IDs
            Assert.AreEqual(3, patientItem.Metadata.Keys.Count);
            Assert.IsTrue(patientItem.Metadata.ContainsKey(_enumCustomMetricItem.Id));
            Assert.AreEqual("one", patientItem.Metadata[_enumCustomMetricItem.Id]);
            Assert.IsTrue(patientItem.Metadata.ContainsKey(_numberCustomMetricItem.Id));
            Assert.AreEqual(1, patientItem.Metadata[_numberCustomMetricItem.Id]);
            Assert.IsTrue(patientItem.Metadata.ContainsKey(_stringCustomMetricItem.Id));
            Assert.AreEqual("I", patientItem.Metadata[_stringCustomMetricItem.Id]);
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
            var uploadBatch = await patientItem.UploadAsync(uploadPath, overrides);

            // Verify test file was uploaded (may have to wait for file to be processed)
            while (true)
            {
                await patientItem.RefreshAsync();
                var entitySummaries = patientItem.FindEntities(t => true);
                if (entitySummaries.Count == 1)
                {
                    break;
                }
            }
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
            var uploadBatch = await patientItem.UploadAsync(uploadPath, overrides);
            var uploadedFiles = Directory.GetFiles(uploadPath);

            // Verify test folder was uploaded (may have to wait for files to be processed)
            while (true)
            {
                await patientItem.RefreshAsync();
                var entitySummaries = patientItem.FindEntities(t => true);
                if (entitySummaries.Count == 1 && entitySummaries[0].Status == "completed")
                {
                    var entityItem = await entitySummaries[0].GetAsync() as ImageSetItem;
                    if (entityItem.Data.Images.Count == 5)
                    {
                        break;
                    }
                }
            }
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
            var uploadBatch = await patientItem.UploadAsync(uploadPaths, overrides);

            // Verify test folder and test file were uploaded (may have to wait for files to be processed)
            while (true)
            {
                await patientItem.RefreshAsync();
                var entitySummaries = patientItem.FindEntities(t => true);
                if (entitySummaries.Count == 2 && entitySummaries[0].Status == "completed" && entitySummaries[1].Status == "completed")
                {
                    var imageSetSummary = patientItem.FindEntities(t => t.Type == "image_set")[0];
                    var entityItem = await imageSetSummary.GetAsync() as ImageSetItem;
                    if (entityItem.Data.Images.Count == 5)
                    {
                        break;
                    }
                }
            }
        }
    }
}
