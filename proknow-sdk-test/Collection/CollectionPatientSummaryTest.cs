using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Collection.Test
{
    [TestClass]
    public class CollectionPatientSummaryTest
    {
        private static string _testClassName = nameof(CollectionPatientSummaryTest);
        private static ProKnowApi _proKnow = TestSettings.ProKnow;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test collections, if necessary
            await TestHelper.DeleteCollectionsAsync(_testClassName);

            // Delete test workspaces, if necessary
            await TestHelper.DeleteWorkspacesAsync(_testClassName);
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
        public async Task GetAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a structure set
            var patientSummary = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummary = patientSummary.FindEntities(e => e.Type == "structure_set")[0];

            // Create a test collection
            var collectionItem = await _proKnow.Collections.CreateAsync($"SDK-{_testClassName}-{testNumber}-Name",
                $"SDK-{_testClassName}-{testNumber}-Description", "workspace", new List<string>() { workspaceItem.Id });

            // Add the patient and structure set to the collection
            var savedItems = new List<CollectionPatientsAddSchema>() { new CollectionPatientsAddSchema(patientSummary.Id, entitySummary.Id) };
            await collectionItem.Patients.AddAsync(workspaceItem.Id, savedItems);

            // Query the items in the collection to get a collection patient summary
            var collectionPatientSummaries = await collectionItem.Patients.QueryAsync();
            Assert.AreEqual(1, collectionPatientSummaries.Count);

            // Get the full patient item
            var patientItem = await collectionPatientSummaries[0].GetAsync();

            // Verify the item returned
            Assert.AreEqual(patientSummary.Id, patientItem.Id);
            Assert.AreEqual(patientSummary.Mrn, patientItem.Mrn);
            Assert.AreEqual(patientSummary.Name, patientItem.Name);
            Assert.IsNotNull(patientItem.FindEntities(e => e.Type == "structure_set"));
        }
    }
}
