using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Test
{
    [TestClass]
    public class PatientsTest
    {
        [TestMethod]
        public async Task FindAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, p => p.Name == TestSettings.TestPatientName);
            Assert.AreEqual(patientSummary.Name, TestSettings.TestPatientName);
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

        [TestMethod]
        public async Task LookupAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var allPatientSummaries = await proKnow.Patients.QueryAsync(workspace.Id);
            var patientMrn = allPatientSummaries.First(p => p.Name == TestSettings.TestPatientName).Mrn;
            var myPatientSummaries = await proKnow.Patients.LookupAsync(workspace.Id, new string[] { "invalidMrn", patientMrn });
            Assert.IsTrue(myPatientSummaries.Count == 2);
            Assert.IsNull(myPatientSummaries[0]);
            Assert.AreEqual(myPatientSummaries[1].Name, TestSettings.TestPatientName);
        }

        [TestMethod]
        public async Task QueryAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummaries = await proKnow.Patients.QueryAsync(workspace.Id);
            Assert.IsTrue(patientSummaries.Count > 0);
        }
    }
}
