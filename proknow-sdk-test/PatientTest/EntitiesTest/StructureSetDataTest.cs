using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ProKnow.Patient;
using ProKnow.Patient.Entities;
using ProKnow.Test;
using ProKnow.Upload;

namespace ProKnow.Patients.Entities.Test
{
    [TestClass]
    public class StructureSetDataTest
    {
        private static string _patientMrnAndName = "SDK-StructureSetDataTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = _proKnow.Uploads;
        private static WorkspaceItem _workspaceItem;
        private static string _uploadPath;
        private static PatientItem _patientItem;
        private static StructureSetData _structureSetData;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);

            // Create a test workspace
            _workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName);

            // Create a test patient
            var patientSummary = await TestHelper.CreatePatientAsync(_patientMrnAndName);

            // Upload test file
            _uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RS.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceItem.Id, _uploadPath, overrides);

            // Wait until uploaded test file has processed
            while (true)
            {
                _patientItem = await patientSummary.GetAsync();
                var entitySummaries = _patientItem.FindEntities(t => t.Type == "structure_set");
                if (entitySummaries.Count > 0 && entitySummaries[0].Status == "completed")
                {
                    var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;
                    _structureSetData = structureSetItem.Data;
                    break;
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
        public void RoisTest()
        {
            Assert.AreEqual(3, _structureSetData.Rois.Length);
            var expectedNames = new string[] { "BODY", "PAROTID_LT", "PTV" }.ToHashSet();
            var actualNames = _structureSetData.Rois.Select(r => r.Name).ToHashSet();
            Assert.IsTrue(expectedNames.SetEquals(actualNames));
        }
    }
}
