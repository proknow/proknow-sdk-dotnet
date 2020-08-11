using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Collection.Test
{
    [TestClass]
    public class CollectionsTest
    {
        private static readonly string _testClassName = nameof(CollectionsTest);
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
            // Delete test collection
            await TestHelper.DeleteCollectionsAsync(_testClassName);

            // Delete test workspace
            await TestHelper.DeleteWorkspacesAsync(_testClassName);
        }

        [TestMethod]
        public async Task CreateAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);

            // Create a collection
            var collectionItem = await _proKnow.Collections.CreateAsync($"{_testClassName}-{testNumber}-Name", $"{_testClassName}-{testNumber}-Description",
                "workspace", new List<string>() { workspaceItem.Id });

            // Verify that the collection was properly created
            Assert.IsFalse(String.IsNullOrEmpty(collectionItem.Id));
            Assert.AreEqual($"{_testClassName}-{testNumber}-Name", collectionItem.Name);
            Assert.AreEqual($"{_testClassName}-{testNumber}-Description", collectionItem.Description);
            Assert.AreEqual("workspace", collectionItem.Type);
            Assert.AreEqual(1, collectionItem.WorkspaceIds.Count);
            Assert.AreEqual(workspaceItem.Id, collectionItem.WorkspaceIds[0]);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            var testNumber = 2;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);

            // Create a collection
            var collectionItem = await _proKnow.Collections.CreateAsync($"{_testClassName}-{testNumber}-Name", $"{_testClassName}-{testNumber}-Description",
                "workspace", new List<string>() { workspaceItem.Id });

            // Delete the collection
            await _proKnow.Collections.DeleteAsync(collectionItem.Id);

            // Verify the deletion
            var collections = await _proKnow.Collections.QueryAsync(workspaceItem.Id);
            Assert.AreEqual(0, collections.Count);
        }

        [TestMethod]
        public async Task FindAsyncTest()
        {
            var testNumber = 3;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);

            // Create a collection
            var collectionItem = await _proKnow.Collections.CreateAsync($"{_testClassName}-{testNumber}-Name", $"{_testClassName}-{testNumber}-Description",
                "workspace", new List<string>() { workspaceItem.Id });

            // Find the collection
            var collectionSummary = await _proKnow.Collections.FindAsync(workspaceItem.Id, p => p.Name == $"{_testClassName}-{testNumber}-Name");

            // Verify the collection found
            Assert.AreEqual(collectionItem.Id, collectionSummary.Id);
            Assert.AreEqual(collectionItem.Name, collectionSummary.Name);
            Assert.AreEqual(collectionItem.Description, collectionSummary.Description);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            var testNumber = 4;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);

            // Create a collection
            var collectionItem = await _proKnow.Collections.CreateAsync($"{_testClassName}-{testNumber}-Name", $"{_testClassName}-{testNumber}-Description",
                "workspace", new List<string>() { workspaceItem.Id });

            // Get the collection
            var collectionItem2 = await _proKnow.Collections.GetAsync(collectionItem.Id);

            // Verify the returned collection
            Assert.AreEqual(collectionItem.Id, collectionItem2.Id);
            Assert.AreEqual(collectionItem.Name, collectionItem2.Name);
            Assert.AreEqual(collectionItem.Description, collectionItem2.Description);
            Assert.AreEqual(collectionItem.Type, collectionItem2.Type);
            Assert.AreEqual(1, collectionItem2.WorkspaceIds.Count);
            Assert.AreEqual(workspaceItem.Id, collectionItem2.WorkspaceIds[0]);
        }

        [TestMethod]
        public async Task QueryAsyncTest()
        {
            var testNumber = 5;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);

            // Create a collection
            var collectionItem = await _proKnow.Collections.CreateAsync($"{_testClassName}-{testNumber}-Name", $"{_testClassName}-{testNumber}-Description",
                "workspace", new List<string>() { workspaceItem.Id });

            // Query for collections
            var collectionSummaries = await _proKnow.Collections.QueryAsync(workspaceItem.Id);

            // Verify the created collection was returned
            Assert.AreEqual(1, collectionSummaries.Count);
            Assert.AreEqual(collectionItem.Id, collectionSummaries[0].Id);
            Assert.AreEqual(collectionItem.Name, collectionSummaries[0].Name);
            Assert.AreEqual(collectionItem.Description, collectionSummaries[0].Description);
        }
    }
}
