using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Patient;
using ProKnow.Test;
using ProKnow.Upload;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Collection.Test
{
    [TestClass]
    public class CollectionSummaryTest
    {
        private static string _baseName = "SDK-CollectionSummaryTest";
        private static ProKnowApi _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = _proKnow.Uploads;
        private static string _workspaceId;
        private static CollectionItem _collectionItem;
        private static CollectionSummary _collectionSummary;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test collections, if necessary
            await TestHelper.DeleteCollectionsAsync(_baseName);

            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspaceAsync(_baseName);

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_baseName);
            _workspaceId = workspaceItem.Id;

            // Create a test patient
            var patientSummary = await TestHelper.CreatePatientAsync(_baseName);

            // Upload test files
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _baseName, Mrn = _baseName }
            };
            await _uploads.UploadAsync(_workspaceId, uploadPath, overrides);

            // Wait until uploaded test files have processed
            while (true)
            {
                var patientItem = await patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(e => e.PatientId == patientSummary.Id);
                if (entitySummaries.Count() >= 4)
                {
                    var statuses = entitySummaries.Select(e => e.Status).Distinct();
                    if (statuses.Count() == 1 && statuses.First() == "completed")
                    {
                        break;
                    }
                }
            }

            // Create a test collection
            _collectionItem = await _proKnow.Collections.CreateAsync($"{_baseName}-Name", $"{_baseName}-Description",
                "workspace", new List<string>() { _workspaceId });
            var collectionSummaries = await _proKnow.Collections.QueryAsync(_workspaceId);
            _collectionSummary = collectionSummaries[0];
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test collection
            await _proKnow.Collections.DeleteAsync(_collectionItem.Id);

            // Delete test workspace
            await _proKnow.Workspaces.DeleteAsync(_workspaceId);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            var collectionItem = await _collectionSummary.GetAsync();
            Assert.AreEqual(_collectionItem.Id, collectionItem.Id);
            Assert.AreEqual(_collectionItem.Name, collectionItem.Name);
            Assert.AreEqual(_collectionItem.Description, collectionItem.Description);
            Assert.AreEqual(_collectionItem.Type, collectionItem.Type);
            Assert.AreEqual(1, collectionItem.WorkspaceIds.Count);
            Assert.AreEqual(_workspaceId, collectionItem.WorkspaceIds[0]);
        }
    }
}
