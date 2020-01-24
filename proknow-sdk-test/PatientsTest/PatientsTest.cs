using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Test
{
    [TestClass]
    public class PatientsTest
    {
        [TestMethod]
        public async Task QueryAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummaries = await proKnow.Patients.QueryAsync(workspace.Id);
            Assert.IsTrue(patientSummaries.Count > 0);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummaries = await proKnow.Patients.QueryAsync(workspace.Id);
            var patientId = patientSummaries.First(p => p.Name == TestSettings.TestPatientName).Id;
            var patientItem = await proKnow.Patients.GetAsync(workspace.Id, patientId);
            Assert.AreEqual(patientItem.Id, patientId);
        }
    }
}
