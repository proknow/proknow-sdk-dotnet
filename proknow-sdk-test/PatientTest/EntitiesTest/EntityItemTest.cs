using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using ProKnow.Upload;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class EntityItemTest
    {
        private static readonly string _patientMrnAndName = "SDK-EntityItemTest";
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static readonly Uploads _uploads = _proKnow.Uploads;
        private static string _workspaceId;
        private static PatientSummary _patientSummary;

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName);
            _workspaceId = workspaceItem.Id;

            // Create a test patient
            _patientSummary = await TestHelper.CreatePatientAsync(_patientMrnAndName);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspace
            await _proKnow.Workspaces.DeleteAsync(_workspaceId);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            // Upload test file
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RP.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceId, uploadPath, overrides);

            // Wait until uploaded test file has processed
            PlanItem planItem = null;
            while (true)
            {
                var patientItem = await _patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "plan");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    planItem = await entitySummaries[0].GetAsync() as PlanItem;
                    break;
                }
            }

            // Delete entity
            await planItem.DeleteAsync();

            // Verify it was deleted
            while (true)
            {
                var patientItem = await _patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "plan");
                if (entitySummaries.Count == 0)
                {
                    break;
                }
            }
        }

        [TestMethod]
        public async Task GetMetadataAsyncTest()
        {
            // Upload test file
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceId, uploadPath, overrides);

            // Wait until uploaded test file has processed
            DoseItem doseItem = null;
            while (true)
            {
                var patientItem = await _patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "dose");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    doseItem = await entitySummaries[0].GetAsync() as DoseItem;
                    break;
                }
            }
            
            // Create custom metric for testing
            var customMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_patientMrnAndName}-1", "dose", "number");

            // Set test metadata
            doseItem.Metadata.Add(customMetricItem.Id, 3.141);

            // Get metadata
            var metadata = await doseItem.GetMetadataAsync();

            // Verify metadata
            Assert.AreEqual(1, metadata.Keys.Count);
            Assert.AreEqual(3.141, metadata[customMetricItem.Name]);

            // Cleanup
            await doseItem.DeleteAsync();
            await _proKnow.CustomMetrics.DeleteAsync(customMetricItem.Id);
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            // Upload test file
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RS.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceId, uploadPath, overrides);

            // Wait until uploaded test file has processed
            EntitySummary entitySummary = null;
            StructureSetItem structureSetItem = null;
            while (true)
            {
                var patientItem = await _patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "structure_set");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    entitySummary = entitySummaries[0];
                    structureSetItem = await entitySummary.GetAsync() as StructureSetItem;
                    break;
                }
            }

            // Create custom metric for testing
            var customMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_patientMrnAndName}-2", "structure_set", "number");

            // Set description and metadata
            structureSetItem.Description = _patientMrnAndName;
            structureSetItem.Metadata.Add(customMetricItem.Id, 1);

            // Save entity changes
            await structureSetItem.SaveAsync();

            // Refresh entity
            structureSetItem = await entitySummary.GetAsync() as StructureSetItem;

            // Verify changes were saved
            Assert.AreEqual(_patientMrnAndName, structureSetItem.Description);
            Assert.AreEqual(1, structureSetItem.Metadata.Keys.Count);
            Assert.AreEqual(1, structureSetItem.Metadata[customMetricItem.Id]);

            // Cleanup
            await structureSetItem.DeleteAsync();
            await _proKnow.CustomMetrics.DeleteAsync(customMetricItem.Id);
        }

        [TestMethod]
        public async Task SetMetadataAsyncTest()
        {
            // Upload test files
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "CT");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceId, uploadPath, overrides);
            var uploadedFiles = Directory.GetFiles(uploadPath);

            // Wait until uploaded test files have processed
            ImageSetItem imageSetItem = null;
            while (true)
            {
                var patientItem = await _patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(t => t.Type == "image_set");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    imageSetItem = await entitySummaries[0].GetAsync() as ImageSetItem;
                    if (imageSetItem.Data.Images.Count == uploadedFiles.Length)
                    {
                        break;
                    }
                }
            }

            // Create custom metric for testing
            var customMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_patientMrnAndName}-3", "image_set", 
                "enum", new string[] { "one", "two", "three" });

            // Set metadata
            var metadata = new Dictionary<string, object>() { { customMetricItem.Name, "two" } };
            await imageSetItem.SetMetadataAsync(metadata);

            // Verify metadata was set
            Assert.AreEqual(1, imageSetItem.Metadata.Keys.Count);
            Assert.AreEqual("two", imageSetItem.Metadata[customMetricItem.Id]);

            // Cleanup
            await imageSetItem.DeleteAsync();
            await _proKnow.CustomMetrics.DeleteAsync(customMetricItem.Id);
        }
    }
}
