using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

using ProKnow.Test;

namespace ProKnow.Patients.Test
{
    [TestClass]
    public class PatientItemTest
    {
        [TestMethod]
        public async Task FindEntitiesTest_Predicate_1st_Level()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, p => p.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var imageSetEntities = patientItem.FindEntities(e => e.Type == "image_set");
            Assert.AreEqual(imageSetEntities.Count, 1);
        }

        [TestMethod]
        public async Task FindEntitiesTest_Predicate_2nd_Level()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, p => p.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var structureSetEntities = patientItem.FindEntities(e => e.Type == "structure_set");
            Assert.AreEqual(structureSetEntities.Count, 1);
        }

        [TestMethod]
        public async Task FindEntitiesTest_Predicate_3rd_Level()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, p => p.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var planEntities = patientItem.FindEntities(e => e.Type == "plan");
            Assert.AreEqual(planEntities.Count, 1);
        }

        [TestMethod]
        public async Task FindEntitiesTest_Predicate_4th_Level()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, p => p.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var doseEntities = patientItem.FindEntities(e => e.Type == "dose");
            Assert.AreEqual(doseEntities.Count, 1);
        }
    }
}
