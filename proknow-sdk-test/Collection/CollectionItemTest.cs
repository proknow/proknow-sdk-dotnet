using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Collection.Test
{
    [TestClass]
    public class CollectionItemTest
    {
        private static readonly string _testClassName = nameof(CollectionItemTest);
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
            // Delete test collections
            await TestHelper.DeleteCollectionsAsync(_testClassName);

            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"));

            // Create a collection
            var collectionItem = await _proKnow.Collections.CreateAsync($"{_testClassName}-{testNumber}-Name", $"{_testClassName}-{testNumber}-Description",
               "workspace", new List<string>() { workspaceItem.Id });

            // Delete the collection
            await collectionItem.DeleteAsync();

            // Verify the collection was deleted
            var collectionSummaries = await _proKnow.Collections.QueryAsync(workspaceItem.Id);
            Assert.AreEqual(0, collectionSummaries.Count);
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            var testNumber = 2;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"));

            // Create a collection
            var collectionItem = await _proKnow.Collections.CreateAsync($"{_testClassName}-{testNumber}-Name", $"{_testClassName}-{testNumber}-Description",
               "workspace", new List<string>() { workspaceItem.Id });

            // Modify the collection and save the changes
            collectionItem.Name = $"{_testClassName}-{testNumber}-Name-2";
            collectionItem.Description = $"{_testClassName}-{testNumber}-Description-2";
            await collectionItem.SaveAsync();

            // Verify the collection changes were saved
            var collectionSummaries = await _proKnow.Collections.QueryAsync(workspaceItem.Id);
            Assert.AreEqual(1, collectionSummaries.Count);
            Assert.AreEqual(collectionItem.Id, collectionSummaries[0].Id);
            Assert.AreEqual(collectionItem.Name, collectionSummaries[0].Name);
            Assert.AreEqual(collectionItem.Description, collectionSummaries[0].Description);
        }
    }
}
