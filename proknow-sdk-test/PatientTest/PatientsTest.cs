using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Patient.Test
{
    [TestClass]
    public class PatientsTest
    {
        private static readonly string _testClassName = nameof(PatientsTest);
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
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);
        }

        [TestCleanup]
        public void Cleanup() 
        { 
            Environment.SetEnvironmentVariable("PATIENTS_PAGE_SIZE", null); 
        }

        [TestMethod]
        public async Task CreateAsyncTest()
        {
            int testNumber = 1;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a new patient
            var patientItem = await _proKnow.Patients.CreateAsync(workspaceItem.Id, "Mrn", "Name", "1976-07-04", "F");

            // Verify the creation
            Assert.AreEqual(workspaceItem.Id, patientItem.WorkspaceId);
            Assert.IsFalse(String.IsNullOrEmpty(patientItem.Id));
            Assert.AreEqual("Mrn", patientItem.Mrn);
            Assert.AreEqual($"Name", patientItem.Name);
            Assert.AreEqual("1976-07-04", patientItem.BirthDate);
            Assert.AreEqual("F", patientItem.Sex);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            int testNumber = 2;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Delete it
            await _proKnow.Patients.DeleteAsync(workspaceItem.Id, patientItem.Id);

            // Verify the deletion
            var patientSummaries = await _proKnow.Patients.LookupAsync(workspaceItem.Id, new List<string> { patientItem.Mrn });
            Assert.IsNull(patientSummaries[0]);
        }

        [TestMethod]
        public async Task FindAsyncTest()
        {
            int testNumber = 3;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Find the created patient
            var patientSummary = await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Id == patientItem.Id);

            // Verify the returned patient
            Assert.AreEqual(patientItem.Name, patientSummary.Name);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            int testNumber = 4;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Get the created patient
            var createdPatientItem = await _proKnow.Patients.GetAsync(workspaceItem.Id, patientItem.Id);

            // Verify the return patient
            Assert.AreEqual(patientItem.Id, createdPatientItem.Id);
        }

        [TestMethod]
        public async Task LookupAsyncTest()
        {
            int testNumber = 5;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Lookup patients by MRN (invalid and valid)
            var patientSummaries = await _proKnow.Patients.LookupAsync(workspaceItem.Id, new string[] { "invalidMrn", patientItem.Mrn });

            // Verify the returned patient summaries
            Assert.IsTrue(patientSummaries.Count == 2);
            Assert.IsNull(patientSummaries[0]);
            Assert.AreEqual(patientItem.Name, patientSummaries[1].Name);
        }

        [TestMethod]
        public async Task QueryAsyncTest_NoSearchString()
        {
            int testNumber = 6;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Query for patients
            var patientSummaries = await _proKnow.Patients.QueryAsync(workspaceItem.Id);

            // Verify the returned patient summaries
            Assert.IsTrue(patientSummaries.Count == 1);
            Assert.AreEqual(patientItem.Name, patientSummaries[0].Name);
        }

        [TestMethod]
        public async Task QueryAsyncTest_NonMatchingSearchString()
        {
            int testNumber = 7;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Query with a non-matching search string
            var patientSummaries = await _proKnow.Patients.QueryAsync(workspaceItem.Id, "foobar");

            // Verify that no patient summaries were returned
            Assert.IsTrue(patientSummaries.Count == 0);
        }

        [TestMethod]
        public async Task QueryAsyncTest_MatchingSearchString()
        {
            int testNumber = 8;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Verify with matching MRN
            var patientSummaries = await _proKnow.Patients.QueryAsync(workspaceItem.Id, patientItem.Mrn);
            Assert.IsTrue(patientSummaries.Count == 1);

            // Verify with matching name
            patientSummaries = await _proKnow.Patients.QueryAsync(workspaceItem.Id, patientItem.Name);
            Assert.IsTrue(patientSummaries.Count == 1);
        }

        [TestMethod]
        public async Task QueryAsyncTest_Paging()
        {
            int testNumber = 9;

            Environment.SetEnvironmentVariable("PATIENTS_PAGE_SIZE", "5");

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create enough patients to invoke paging (over 5 (page size))
            int numPatients = 12;
            var patientItems = await TestHelper.CreateMultiPatientAsync(_testClassName, testNumber, numPatients);

            // Query for the patients
            var patientSummaries = await _proKnow.Patients.QueryAsync(workspaceItem.Id);

            // Sort by ID for comparison
            var patientItemsList = new List<PatientItem>(patientItems);
            var patientSummariesList = new List<PatientSummary>(patientSummaries);
            patientItemsList.Sort((x, y) => x.Id.CompareTo(y.Id));
            patientSummariesList.Sort((x, y) => x.Id.CompareTo(y.Id));

            // Verify the return patients
            Assert.IsTrue(patientSummaries.Count == numPatients);
            for (var i = 0; i < numPatients; i++)
            {
                Assert.AreEqual(patientItemsList[i].Id, patientSummariesList[i].Id);
                Assert.AreEqual(patientItemsList[i].Name, patientSummariesList[i].Name);
            }
        }
    }
}
