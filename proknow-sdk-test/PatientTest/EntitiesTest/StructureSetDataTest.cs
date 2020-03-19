using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProKnow.Test;
using ProKnow.Upload;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class StructureSetDataTest
    {
        private static string _patientMrnAndName = "SDK-StructureSetDataTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = _proKnow.Uploads;
        private static string _workspaceId;
        private static string _uploadPath;
        private static PatientItem _patientItem;
        private static StructureSetData _structureSetData;

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

            // Upload test file
            _uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RS.dcm");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceId, _uploadPath, overrides);

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
            await _proKnow.Workspaces.DeleteAsync(_workspaceId);
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
