using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using ProKnow.Upload;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Patient.Test
{
    [TestClass]
    public class PatientsTest
    {
        private static string _patientMrnAndName = "SDK-PatientsTest";
        private static ProKnowApi _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = _proKnow.Uploads;
        private static string _workspaceId;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
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
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspace
            await _proKnow.Workspaces.DeleteAsync(_workspaceId);
        }

        [TestMethod]
        public async Task CreateAsyncTest()
        {
            // Create a new patient
            var patientItem = await _proKnow.Patients.CreateAsync(_workspaceId, $"{_patientMrnAndName}-Mrn-2",
                $"{_patientMrnAndName}-Name-2", "1976-07-04", "F");

            // Verify the creation
            Assert.AreEqual(_workspaceId, patientItem.WorkspaceId);
            Assert.IsFalse(String.IsNullOrEmpty(patientItem.Id));
            Assert.AreEqual($"{_patientMrnAndName}-Mrn-2", patientItem.Mrn);
            Assert.AreEqual($"{_patientMrnAndName}-Name-2", patientItem.Name);
            Assert.AreEqual("1976-07-04", patientItem.BirthDate);
            Assert.AreEqual("F", patientItem.Sex);

            // Cleanup
            await _proKnow.Patients.DeleteAsync(_workspaceId, patientItem.Id);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            // Create a new patient
            var patientItem = await _proKnow.Patients.CreateAsync(_workspaceId, $"{_patientMrnAndName}-Mrn-3",
                $"{_patientMrnAndName}-Name-3", "1960-01-01", "M");

            // Verify the creation
            var patientSummary = await _proKnow.Patients.FindAsync(_workspaceId, p => p.Name == $"{_patientMrnAndName}-Name-3");
            Assert.IsNotNull(patientSummary);

            // Delete it
            await _proKnow.Patients.DeleteAsync(_workspaceId, patientItem.Id);

            // Verify the deletion
            patientSummary = await _proKnow.Patients.FindAsync(_workspaceId, p => p.Name == $"{_patientMrnAndName}-Name-3");
            Assert.IsNull(patientSummary);
        }

        [TestMethod]
        public async Task FindAsyncTest()
        {
            var patientSummary = await _proKnow.Patients.FindAsync(_workspaceId, p => p.Name == _patientMrnAndName);
            Assert.AreEqual(patientSummary.Name, _patientMrnAndName);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            var patientSummaries = await _proKnow.Patients.QueryAsync(_workspaceId);
            var patientId = patientSummaries.First(p => p.Name == _patientMrnAndName).Id;
            var patientItem = await _proKnow.Patients.GetAsync(_workspaceId, patientId);
            Assert.AreEqual(patientItem.Id, patientId);
        }

        [TestMethod]
        public async Task LookupAsyncTest()
        {
            var myPatientSummaries = await _proKnow.Patients.LookupAsync(_workspaceId, 
                new string[] { "invalidMrn", _patientMrnAndName });
            Assert.IsTrue(myPatientSummaries.Count == 2);
            Assert.IsNull(myPatientSummaries[0]);
            Assert.AreEqual(myPatientSummaries[1].Name, _patientMrnAndName);
        }

        [TestMethod]
        public async Task QueryAsyncTest_NoSearchString()
        {
            var patientSummaries = await _proKnow.Patients.QueryAsync(_workspaceId);
            Assert.IsTrue(patientSummaries.Count == 1);
        }

        [TestMethod]
        public async Task QueryAsyncTest_NonMatchingSearchString()
        {
            var patientSummaries = await _proKnow.Patients.QueryAsync(_workspaceId, "foobar");
            Assert.IsTrue(patientSummaries.Count == 0);
        }

        [TestMethod]
        public async Task QueryAsyncTest_MatchingSearchString()
        {
            // Change patient name so it doesn't match MRN
            var patientSummaries = await _proKnow.Patients.QueryAsync(_workspaceId);
            var patientItem = await patientSummaries.First().GetAsync();
            patientItem.Name = $"{patientItem.Name}-Name";
            await patientItem.SaveAsync();

            // Verify with matching MRN
            patientSummaries = await _proKnow.Patients.QueryAsync(_workspaceId, patientItem.Mrn);
            Assert.IsTrue(patientSummaries.Count == 1);

            // Verify with matching name
            patientSummaries = await _proKnow.Patients.QueryAsync(_workspaceId, patientItem.Name);
            Assert.IsTrue(patientSummaries.Count == 1);

            // Restore patient name to original
            patientItem.Name = _patientMrnAndName;
            await patientItem.SaveAsync();
        }
    }
}
