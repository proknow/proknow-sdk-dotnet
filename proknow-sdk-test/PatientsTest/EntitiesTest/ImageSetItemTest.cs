using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ProKnow.Test;

namespace ProKnow.Patients.Entities.Test
{
    [TestClass]
    public class ImageSetItemTest
    {
        [TestMethod]
        public async Task DownloadAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, t => t.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var imageSetSummary = patientItem.FindEntities(t => t.Type == "image_set").First();
            var imageSetItem = await imageSetSummary.GetAsync();
            string rootFolder = Path.Combine(Path.GetTempPath(), "ImageSetItemTest_DownloadAsyncTest");
            string imageSetFolder = await imageSetItem.Download(rootFolder);
            Directory.Delete(rootFolder, true);
        }
    }
}
