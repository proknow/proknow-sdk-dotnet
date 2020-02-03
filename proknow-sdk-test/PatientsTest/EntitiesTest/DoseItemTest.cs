using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ProKnow.Test;

namespace ProKnow.Patients.Entities.Test
{
    [TestClass]
    public class DoseItemTest
    {
        [TestMethod]
        public async Task DownloadAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, t => t.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var entitySummary = patientItem.FindEntities(t => t.Type == "dose").First();
            var doseItem = await entitySummary.GetAsync();
            string folder = Path.Combine(Path.GetTempPath(), "DoseItemTest_DownloadAsyncTest");
            string file = await doseItem.Download(folder);
            Directory.Delete(folder, true);
        }
    }
}
