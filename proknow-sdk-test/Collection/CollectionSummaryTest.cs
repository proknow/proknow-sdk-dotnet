using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Collection.Test
{
    [TestClass]
    public class CollectionSummaryTest
    {
        private static readonly string _testClassName = nameof(CollectionSummaryTest);
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
        public async Task GetAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);

            // Create a test collection
            var collectionItem = await _proKnow.Collections.CreateAsync($"{_testClassName}-{testNumber}-Name", $"{_testClassName}-{testNumber}-Description",
                "workspace", new List<string>() { workspaceItem.Id });
            var collectionSummaries = await _proKnow.Collections.QueryAsync(workspaceItem.Id);
            var collectionSummary = collectionSummaries[0];

            // Get the collection from the collection summary
            var collectionItem2 = await collectionSummary.GetAsync();

            // Verify the returned collection
            Assert.AreEqual(collectionItem.Id, collectionItem2.Id);
            Assert.AreEqual(collectionItem.Name, collectionItem2.Name);
            Assert.AreEqual(collectionItem.Description, collectionItem2.Description);
            Assert.AreEqual(collectionItem.Type, collectionItem2.Type);
            Assert.AreEqual(1, collectionItem2.WorkspaceIds.Count);
            Assert.AreEqual(workspaceItem.Id, collectionItem2.WorkspaceIds[0]);
        }
    }
}
