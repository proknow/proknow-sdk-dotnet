using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Patient.Entities;
using ProKnow.Patient;
using ProKnow.Test;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Collection.Test
{
    [TestClass]
    public class CollectionPatientsTest
    {
        private static readonly string _testClassName = nameof(CollectionPatientsTest);
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
        public async Task QueryAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a structure set
            var patientSummary = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"));
            var entitySummary = patientSummary.FindEntities(e => e.Type == "structure_set")[0];

            // Create a test collection
            var collectionItem = await _proKnow.Collections.CreateAsync($"SDK-{_testClassName}-{testNumber}-Name",
                $"SDK-{_testClassName}-{testNumber}-Description", "workspace", new List<string>() { workspaceItem.Id });

            // Add the patient and structure set to the collection
            var savedItems = new List<CollectionPatientsAddSchema>() { new CollectionPatientsAddSchema(patientSummary.Id, entitySummary.Id) };
            await collectionItem.Patients.AddAsync(workspaceItem.Id, savedItems);

            // Query the items in the collection
            var collectionPatientSummaries = await collectionItem.Patients.QueryAsync();

            // Verify the items returned
            Assert.AreEqual(1, collectionPatientSummaries.Count);
            Assert.AreEqual(workspaceItem.Id, collectionPatientSummaries[0].Workspace.Id);
            Assert.AreEqual(workspaceItem.Slug, collectionPatientSummaries[0].Workspace.Slug);
            Assert.AreEqual(workspaceItem.Name, collectionPatientSummaries[0].Workspace.Name);
            Assert.AreEqual(patientSummary.Id, collectionPatientSummaries[0].Patient.Id);
            Assert.AreEqual(patientSummary.Mrn, collectionPatientSummaries[0].Patient.Mrn);
            Assert.AreEqual(patientSummary.Name, collectionPatientSummaries[0].Patient.Name);
            Assert.AreEqual(entitySummary.Id, collectionPatientSummaries[0].Entity.Id);
            Assert.AreEqual(entitySummary.Type, collectionPatientSummaries[0].Entity.Type);
        }

        [TestMethod]
        public async Task RemoveAsyncTest()
        {
            var testNumber = 2;

            // Create two test workspaces
            var workspaceItem1 = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);
            var workspaceItem2 = await TestHelper.CreateWorkspaceAsync(_testClassName, 1000 + testNumber);

            // To each workspace, add a patient with a structure set
            var patientSummary1 = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"));
            var entitySummary1 = patientSummary1.FindEntities(e => e.Type == "structure_set")[0];
            var patientSummary2 = await TestHelper.CreatePatientAsync(_testClassName, 1000 + testNumber, Path.Combine("Becker^Matthew", "RS.dcm"));
            var entitySummary2 = patientSummary2.FindEntities(e => e.Type == "structure_set")[0];

            // Create a test collection
            var collectionItem = await _proKnow.Collections.CreateAsync($"SDK-{_testClassName}-{testNumber}-Name",
                $"SDK-{_testClassName}-{testNumber}-Description", "organization", new List<string>() { workspaceItem1.Id, workspaceItem2.Id });

            // Add the patients and structures set to the collection
            var savedItems1 = new List<CollectionPatientsAddSchema>() { new CollectionPatientsAddSchema(patientSummary1.Id, entitySummary1.Id) };
            await collectionItem.Patients.AddAsync(workspaceItem1.Id, savedItems1);
            var savedItems2 = new List<CollectionPatientsAddSchema>() { new CollectionPatientsAddSchema(patientSummary2.Id, entitySummary2.Id) };
            await collectionItem.Patients.AddAsync(workspaceItem2.Id, savedItems2);

            // Query the items in the collection
            var collectionPatientSummaries = await collectionItem.Patients.QueryAsync();

            // Verify both patients and structure sets are returned
            Assert.AreEqual(2, collectionPatientSummaries.Count);
            Assert.IsTrue(collectionPatientSummaries.Any(p => p.Workspace.Id == workspaceItem1.Id &&
                p.Patient.Id == patientSummary1.Id && p.Entity.Id == entitySummary1.Id));
            Assert.IsTrue(collectionPatientSummaries.Any(p => p.Workspace.Id == workspaceItem2.Id &&
                p.Patient.Id == patientSummary2.Id && p.Entity.Id == entitySummary2.Id));

            // Remove the first patient and structure set
            await collectionItem.Patients.RemoveAsync(workspaceItem1.Id, new List<string>() { patientSummary1.Id });

            // Query the items in the collection again
            collectionPatientSummaries = await collectionItem.Patients.QueryAsync();

            // Verify that only the second patient and structure set are returned
            Assert.AreEqual(1, collectionPatientSummaries.Count);
            Assert.IsTrue(collectionPatientSummaries.Any(p => p.Workspace.Id == workspaceItem2.Id &&
                p.Patient.Id == patientSummary2.Id && p.Entity.Id == entitySummary2.Id));
        }

        [TestMethod]
        public async Task QueryAsyncTest_Paging()
        {
            var testNumber = 3;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test collection
            var collectionItem = await _proKnow.Collections.CreateAsync($"SDK-{_testClassName}-{testNumber}-Name",
                $"SDK-{_testClassName}-{testNumber}-Description", "workspace", new List<string>() { workspaceItem.Id });

            // Create enough test patients to invoke paging (over 200)
            var numPatients = 210;
            var patientItems = await TestHelper.CreateMultiPatientAsync(_testClassName, testNumber, numPatients);

            var patientCollectionTasks = new List<Task>();
            for (var i = 0; i < numPatients; i++)
            {
                // Add the patient and structure set to the collection
                var savedPatientItems = new List<CollectionPatientsAddSchema>() { new CollectionPatientsAddSchema(patientItems[i].Id) };
                patientCollectionTasks.Add(collectionItem.Patients.AddAsync(workspaceItem.Id, savedPatientItems));
            }
            await Task.WhenAll(patientCollectionTasks);

            // Query the items in the collection
            var collectionPatientSummaries = await collectionItem.Patients.QueryAsync();

            // Sort by ID for comparison
            var patientItemsList = new List<PatientItem>(patientItems);
            var collectionPatientSummariesList = new List<CollectionPatientSummary>(collectionPatientSummaries);
            patientItemsList.Sort((x, y) => x.Id.CompareTo(y.Id));
            collectionPatientSummariesList.Sort((x, y) => x.Patient.Id.CompareTo(y.Patient.Id));

            // Verify the items returned
            Assert.AreEqual(numPatients, collectionPatientSummaries.Count);
            for (var i = 0; i < numPatients; i++)
            {
                Assert.AreEqual(patientItemsList[i].Id, collectionPatientSummariesList[i].Patient.Id);
                Assert.AreEqual(patientItemsList[i].Name, collectionPatientSummariesList[i].Patient.Name);
            }
        }
    }
}
