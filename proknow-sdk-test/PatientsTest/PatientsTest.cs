using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace ProKnow.Test
{
    [TestClass]
    public class PatientsTest
    {
        [TestMethod]
        public async Task QueryTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patients = await proKnow.Patients.QueryAsync(workspace.Id);
            Assert.IsTrue(patients.Count > 0);
        }
    }
}
