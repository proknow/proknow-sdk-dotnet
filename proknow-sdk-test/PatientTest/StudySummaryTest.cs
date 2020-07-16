using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProKnow.Test;

namespace ProKnow.Patient.Test
{
    [TestClass]
    public class StudySummaryTest
    {
        private static readonly string _patientMrnAndName = "SDK-StudySummaryTest";
        private static readonly string _downloadFolderRoot = Path.Combine(Path.GetTempPath(), _patientMrnAndName);

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspacesAsync(_patientMrnAndName);

            // Create download folder root
            if (Directory.Exists(_downloadFolderRoot))
            {
                Directory.Delete(_downloadFolderRoot, true);
            }
            Directory.CreateDirectory(_downloadFolderRoot);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_patientMrnAndName);

            // Delete download folder
            if (Directory.Exists(_downloadFolderRoot))
            {
                Directory.Delete(_downloadFolderRoot, true);
            }
        }

        [TestMethod]
        public async Task Constructor_IncludesSros()
        {
            int testNumber = 1;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, "Sro", 3);
            var studySummary = patientItem.Studies.Where(s => s.Sros.Count() > 0).First();
            var ctImageSetItem = await patientItem.FindEntities(e => e.Modality == "CT")[0].GetAsync();
            var mrImageSetItem = await patientItem.FindEntities(e => e.Modality == "MR")[0].GetAsync();

            // Verify that the study summary includes the SROs
            Assert.AreEqual(1, studySummary.Sros.Count);
            var sroSummary = studySummary.Sros[0];
            Assert.AreEqual(patientItem.WorkspaceId, sroSummary.WorkspaceId);
            Assert.AreEqual(patientItem.Id, sroSummary.PatientId);
            Assert.AreEqual(studySummary.Id, sroSummary.StudyId);
            Assert.IsNotNull(sroSummary.Id);
            Assert.AreEqual("1.2.246.352.221.52738008096457865345287404867971417272", sroSummary.Uid);
            Assert.AreEqual("2018-02-19 09:06:05", sroSummary.Name);
            Assert.AreEqual("rigid", sroSummary.Type);
            Assert.AreEqual(mrImageSetItem.FrameOfReferenceUid, sroSummary.SourceFrameOfReferenceUid);
            Assert.AreEqual(mrImageSetItem.Id, sroSummary.SourceImageSetId);
            Assert.AreEqual(ctImageSetItem.FrameOfReferenceUid, sroSummary.TargetFrameOfReferenceUid);
            Assert.AreEqual(ctImageSetItem.Id, sroSummary.TargetImageSetId);
        }
    }
}
