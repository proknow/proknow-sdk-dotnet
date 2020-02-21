using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProKnow.Test;
using ProKnow.Upload;

namespace ProKnow.Patient.Test
{
    [TestClass]
    public class PatientsTest
    {
        private static string _patientMrnAndName = "SDK-PatientsTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = new Uploads(_proKnow);
        private static WorkspaceItem _workspaceItem;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);

            // Create a test workspace
            _workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName);

            // Create a test patient
            var patientSummary = await TestHelper.CreatePatientAsync(_patientMrnAndName);

            // Upload test files
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceItem.Id, uploadPath, overrides);

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
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);
        }

        [TestMethod]
        public async Task FindAsyncTest()
        {
            var patientSummary = await _proKnow.Patients.FindAsync(_workspaceItem.Id, p => p.Name == _patientMrnAndName);
            Assert.AreEqual(patientSummary.Name, _patientMrnAndName);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            var patientSummaries = await _proKnow.Patients.QueryAsync(_workspaceItem.Id);
            var patientId = patientSummaries.First(p => p.Name == _patientMrnAndName).Id;
            var patientItem = await _proKnow.Patients.GetAsync(_workspaceItem.Id, patientId);
            Assert.AreEqual(patientItem.Id, patientId);
        }

        [TestMethod]
        public async Task LookupAsyncTest()
        {
            var myPatientSummaries = await _proKnow.Patients.LookupAsync(_workspaceItem.Id, new string[] { "invalidMrn", _patientMrnAndName });
            Assert.IsTrue(myPatientSummaries.Count == 2);
            Assert.IsNull(myPatientSummaries[0]);
            Assert.AreEqual(myPatientSummaries[1].Name, _patientMrnAndName);
        }

        [TestMethod]
        public async Task QueryAsyncTest()
        {
            var patientSummaries = await _proKnow.Patients.QueryAsync(_workspaceItem.Id);
            Assert.IsTrue(patientSummaries.Count == 1);
        }
    }
}
