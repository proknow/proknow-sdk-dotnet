using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Patient;
using ProKnow.Test;
using ProKnow.Upload;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Collection.Test
{
    [TestClass]
    public class CollectionsTest
    {
        private static string _baseName = "SDK-CollectionsTest";
        private static ProKnowApi _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = _proKnow.Uploads;
        private static string _workspaceId;
        private static CollectionItem _collectionItem;

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
        public void CreateAsyncTest()
        {
            // Verify that the collection was properly created by the test class initialization
            Assert.IsFalse(String.IsNullOrEmpty(_collectionItem.Id));
            Assert.AreEqual($"{_baseName}-Name", _collectionItem.Name);
            Assert.AreEqual($"{_baseName}-Description", _collectionItem.Description);
            Assert.AreEqual("workspace", _collectionItem.Type);
            Assert.AreEqual(1, _collectionItem.WorkspaceIds.Count);
            Assert.AreEqual(_workspaceId, _collectionItem.WorkspaceIds[0]);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            // Delete the collection
            await _proKnow.Collections.DeleteAsync(_collectionItem.Id);

            // Verify the deletion
            var collections = await _proKnow.Collections.QueryAsync(_workspaceId);
            Assert.AreEqual(0, collections.Count);

            // Restore the deleted collection
            _collectionItem = await _proKnow.Collections.CreateAsync($"{_baseName}-Name", $"{_baseName}-Description",
                "workspace", new List<string>() { _workspaceId });
        }

        [TestMethod]
        public async Task FindAsyncTest()
        {
            var collectionSummary = await _proKnow.Collections.FindAsync(_workspaceId, p => p.Name.Contains(_baseName));
            Assert.AreEqual(_collectionItem.Id, collectionSummary.Id);
            Assert.AreEqual(_collectionItem.Name, collectionSummary.Name);
            Assert.AreEqual(_collectionItem.Description, collectionSummary.Description);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            var collectionItem = await _proKnow.Collections.GetAsync(_collectionItem.Id);
            Assert.AreEqual(_collectionItem.Id, collectionItem.Id);
            Assert.AreEqual(_collectionItem.Name, collectionItem.Name);
            Assert.AreEqual(_collectionItem.Description, collectionItem.Description);
            Assert.AreEqual(_collectionItem.Type, collectionItem.Type);
            Assert.AreEqual(1, collectionItem.WorkspaceIds.Count);
            Assert.AreEqual(_workspaceId, collectionItem.WorkspaceIds[0]);
        }

        [TestMethod]
        public async Task QueryAsyncTest()
        {
            var collectionSummaries = await _proKnow.Collections.QueryAsync(_workspaceId);
            Assert.AreEqual(1, collectionSummaries.Count);
            Assert.AreEqual(_collectionItem.Id, collectionSummaries[0].Id);
            Assert.AreEqual(_collectionItem.Name, collectionSummaries[0].Name);
            Assert.AreEqual(_collectionItem.Description, collectionSummaries[0].Description);
        }
    }
}
