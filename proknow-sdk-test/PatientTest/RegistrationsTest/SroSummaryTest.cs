using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Registrations.Test
{
    [TestClass]
    public class SroSummaryTest
    {
        private static string _patientMrnAndName = "SDK-SroSummaryTest";
        private static ProKnowApi _proKnow = TestSettings.ProKnow;
        private static string _downloadFolderRoot = Path.Combine(Path.GetTempPath(), _patientMrnAndName);

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
        public async Task GetAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_patientMrnAndName, testNumber, "Sro", 3);

            // Get the full representation of the image sets and SRO
            var ctImageSetItem = await patientItem.FindEntities(e => e.Modality == "CT")[0].GetAsync();
            var mrImageSetItem = await patientItem.FindEntities(e => e.Modality == "MR")[0].GetAsync();
            var sroItem = await patientItem.FindSros(s => true)[0].GetAsync();

            // Verify the contents
            Assert.AreEqual(workspaceItem.Id, sroItem.WorkspaceId);
            Assert.AreEqual(patientItem.Id, sroItem.PatientId);
            Assert.AreEqual(ctImageSetItem.StudyId, sroItem.StudyId);
            Assert.IsNotNull(sroItem.Id);
            Assert.AreEqual("1.2.246.352.221.52738008096457865345287404867971417272", sroItem.Uid);
            Assert.AreEqual("2018-02-19 09:06:05", sroItem.Name);
            Assert.AreEqual("rigid", sroItem.Type);
            Assert.AreEqual(mrImageSetItem.FrameOfReferenceUid, sroItem.Source.FrameOfReferenceUid);
            Assert.AreEqual(mrImageSetItem.Id, sroItem.Source.ImageSetId);
            Assert.AreEqual(ctImageSetItem.FrameOfReferenceUid, sroItem.Target.FrameOfReferenceUid);
            Assert.AreEqual(ctImageSetItem.Id, sroItem.Target.ImageSetId);
        }
    }
}
