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
        private static string _patientMrnAndName = "SDK-PatientItemTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = _proKnow.Uploads;
        private static CustomMetricItem _enumCustomMetricItem;
        private static CustomMetricItem _numberCustomMetricItem;
        private static CustomMetricItem _stringCustomMetricItem;
        private static string _workspaceId;
        private static PatientItem _patientItem;
        private static Dictionary<string, object> _metadata;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete existing custom metrics, if necessary
            await TestHelper.DeleteCustomMetricsAsync(_patientMrnAndName);

            // Create custom metrics for testing
            _enumCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_patientMrnAndName}-enum", "patient", "enum",
                new string[] { "one", "two", "three" });
            _numberCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_patientMrnAndName}-number", "patient", "number");
            _stringCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_patientMrnAndName}-string", "patient", "string");

            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName);
            _workspaceId = workspaceItem.Id;

            // Create a test patient
            var patientSummary = await TestHelper.CreatePatientAsync(_patientMrnAndName);

            // Upload test files
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceId, uploadPath, overrides);

            // Wait until uploaded test files have processed
            while (true)
            {
                _patientItem = await patientSummary.GetAsync();
                var entitySummaries = _patientItem.FindEntities(e => true);
                if (entitySummaries.Count() >= 4)
                {
                    var statuses = entitySummaries.Select(e => e.Status).Distinct();
                    if (statuses.Count() == 1 && statuses.First() == "completed")
                    {
                        break;
                    }
                }
            }

            // Add custom metric values
            _metadata = new Dictionary<string, object>();
            _metadata.Add(_enumCustomMetricItem.Name, "one");
            _metadata.Add(_numberCustomMetricItem.Name, 1);
            _metadata.Add(_stringCustomMetricItem.Name, "I");
            await _patientItem.SetMetadataAsync(_metadata);
            await _patientItem.SaveAsync();

            //todo--There is some timing issue that causes only 2 of the 3 custom metrics to get saved to the test patient (possibly conflict with another test)
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspace
            await _proKnow.Workspaces.DeleteAsync(_workspaceId);

            // Delete custom metrics
            await TestHelper.DeleteCustomMetricsAsync(_patientMrnAndName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            // Delete this patient
            await _patientItem.DeleteAsync();

            // Verify that the patient was deleted
            while (true)
            {
                var patientSummaries = await _proKnow.Patients.LookupAsync(_workspaceId, new List<string>() { _patientItem.Mrn });
                if (patientSummaries[0] == null)
                {
                    break;
                }
            }

            // Restore the patient for other tests

            // Upload test files
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            var uploadBatch = await _uploads.UploadAsync(_workspaceId, uploadPath, overrides);

            // Wait until uploaded test files have processed
            while (true)
            {
                _patientItem = await _proKnow.Patients.GetAsync(_workspaceId, uploadBatch.Patients.First().Id);
                var entitySummaries = _patientItem.FindEntities(e => true);
                if (entitySummaries.Count() >= 4)
                {
                    var statuses = entitySummaries.Select(e => e.Status).Distinct();
                    if (statuses.Count() == 1 && statuses.First() == "completed")
                    {
                        break;
                    }
                }
            }

            // Add custom metric values
            await _patientItem.SetMetadataAsync(_metadata);
            await _patientItem.SaveAsync();
        }

        [TestMethod]
        public void FindEntitiesTest_Predicate_1st_Level()
        {
           var imageSetEntities = _patientItem.FindEntities(e => e.Type == "image_set");
            Assert.AreEqual(imageSetEntities.Count, 1);
        }

        [TestMethod]
        public void FindEntitiesTest_Predicate_2nd_Level()
        {
            var structureSetEntities = _patientItem.FindEntities(e => e.Type == "structure_set");
            Assert.AreEqual(structureSetEntities.Count, 1);
        }

        [TestMethod]
        public void FindEntitiesTest_Predicate_3rd_Level()
        {
            var planEntities = _patientItem.FindEntities(e => e.Type == "plan");
            Assert.AreEqual(planEntities.Count, 1);
        }

        [TestMethod]
        public void FindEntitiesTest_Predicate_4th_Level()
        {
            var doseEntities = _patientItem.FindEntities(e => e.Type == "dose");
            Assert.AreEqual(doseEntities.Count, 1);
        }

        [TestMethod]
        public async Task GetMetadataAsyncTest()
        {
            var metadata = await _patientItem.GetMetadataAsync();
            Assert.AreEqual(3, metadata.Keys.Count);
            Assert.IsTrue(metadata.ContainsKey(_enumCustomMetricItem.Name));
            Assert.AreEqual("one", metadata[_enumCustomMetricItem.Name]);
            Assert.IsTrue(metadata.ContainsKey(_numberCustomMetricItem.Name));
            Assert.AreEqual(1, metadata[_numberCustomMetricItem.Name]);
            Assert.IsTrue(metadata.ContainsKey(_stringCustomMetricItem.Name));
            Assert.AreEqual("I", metadata[_stringCustomMetricItem.Name]);
        }

        //todo--RefreshAsyncTest

        //todo--SaveAsyncTest

        [TestMethod]
        public async Task SetMetadataAsyncTest()
        {
            // Verify the metadata set in the class initializer
            var patientSummary = await _proKnow.Patients.FindAsync(_workspaceId, p => p.Id == _patientItem.Id);
            var patientItem = await patientSummary.GetAsync();
            Assert.AreEqual(3, patientItem.Metadata.Keys.Count);
            Assert.IsTrue(patientItem.Metadata.ContainsKey(_enumCustomMetricItem.Id));
            Assert.AreEqual("one", patientItem.Metadata[_enumCustomMetricItem.Id]);
            Assert.IsTrue(patientItem.Metadata.ContainsKey(_numberCustomMetricItem.Id));
            Assert.AreEqual(1, patientItem.Metadata[_numberCustomMetricItem.Id]);
            Assert.IsTrue(patientItem.Metadata.ContainsKey(_stringCustomMetricItem.Id));
            Assert.AreEqual("I", patientItem.Metadata[_stringCustomMetricItem.Id]);
        }

        [TestMethod]
        public async Task UploadAsyncTest_SingleFile()
        {
            // Upload test file
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            var uploadBatch = await _patientItem.UploadAsync(uploadPath, overrides);

            // Wait until uploaded test file has processed
            while (true)
            {
                await _patientItem.RefreshAsync();
                var entitySummaries = _patientItem.FindEntities(t => t.Type == "plan");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    // Make sure the uploaded data is the same
                    Assert.AreEqual(entitySummaries[0].Uid, "2.16.840.1.114337.1.1.1535997926.0");

                    // Cleanup (in case there are other tests using the same patient)
                    await entitySummaries[0].DeleteAsync();

                    break;
                }
            }
        }

        [TestMethod]
        public async Task UploadAsyncTest_SingleFolder()
        {
            // Upload test folder
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            var uploadBatch = await _patientItem.UploadAsync(uploadPath, overrides);
            var uploadedFiles = Directory.GetFiles(uploadPath);

            // Wait until uploaded test file has processed
            while (true)
            {
                await _patientItem.RefreshAsync();
                var entitySummaries = _patientItem.FindEntities(t => t.Type == "image_set");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    // Make sure the uploaded data is the same
                    var entityItem = await entitySummaries[0].GetAsync() as ImageSetItem;
                    if (entityItem.Data.Images.Count == uploadedFiles.Length)

                        // Cleanup (in case there are other tests using the same image set)
                        await entityItem.DeleteAsync();

                    break;
                }
            }
        }

        //todo--UploadAsyncTest_MultipleFilesAndOrFolders
    }
}
