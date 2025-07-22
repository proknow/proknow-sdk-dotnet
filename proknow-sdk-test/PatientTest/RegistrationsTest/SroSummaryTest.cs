using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Registrations.Test
{
    [TestClass]
    public class SroSummaryTest
    {
        private static readonly string _testClassName = nameof(SroSummaryTest);
        private static readonly string _downloadFolderRoot = Path.Combine(Path.GetTempPath(), _testClassName);

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Cleanup from previous test stoppage or failure, if necessary
            await ClassCleanup();

            // Create download folder root
            Directory.CreateDirectory(_downloadFolderRoot);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);

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
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Sro");

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

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            int testNumber = 2;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, "Sro");
            var sroSummaries = patientItem.FindSros(s => true);
            Assert.AreEqual(1, sroSummaries.Count);

            // Delete the SRO
            await sroSummaries[0].DeleteAsync();
            await patientItem.RefreshAsync();

            // Verify that the SRO was deleted
            sroSummaries = patientItem.FindSros(s => true);
            Assert.AreEqual(0, sroSummaries.Count);
        }
    }
}
