using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProKnow.Test;
using ProKnow.Upload;

namespace ProKnow.Patient.Test
{
    [TestClass]
    public class DocumentsTest
    {
        private static string _patientMrnAndName = "SDK-DocumentsTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = new Uploads(_proKnow);
        private static string _workspaceId;
        private static string _patientId;

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
            _patientId = patientSummary.Id;

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
            var documentPath = Path.Combine(TestSettings.TestDataRootDirectory, "dummy.pdf");
            await _proKnow.Patients.Documents.CreateAsync(_workspaceId, _patientId, documentPath);
            //todo--stream and compare
        }
    }
}
