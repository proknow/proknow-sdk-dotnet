using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Threading.Tasks;

using ProKnow.Test;

namespace ProKnow.Patients.Entities.Test
{
    [TestClass]
    public class EntitySummaryTest
    {
        [TestMethod]
        public async Task ImageSet_GetAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, t => t.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var imageSetSummary = patientItem.Studies[0].Entities[0]; //todo--use FindEntities
            var imageSetItem = await imageSetSummary.GetAsync();
            Assert.AreEqual(imageSetItem.WorkspaceId, workspace.Id);
            Assert.AreEqual(imageSetItem.PatientId, patientItem.Id);
            Assert.AreEqual(imageSetItem.Id, imageSetSummary.Id);
        }

        [TestMethod]
        public async Task StructureSet_GetAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, t => t.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var structureSetSummary = patientItem.Studies[0].Entities[0].Entities[0]; //todo--use FindEntities
            var structureSetItem = await structureSetSummary.GetAsync();
            Assert.AreEqual(structureSetItem.WorkspaceId, workspace.Id);
            Assert.AreEqual(structureSetItem.PatientId, patientItem.Id);
            Assert.AreEqual(structureSetItem.Id, structureSetSummary.Id);
        }

        [TestMethod]
        public async Task Plan_GetAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, t => t.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var planSummary = patientItem.Studies[0].Entities[0].Entities[0].Entities[0]; //todo--use FindEntities
            var planItem = await planSummary.GetAsync();
            Assert.AreEqual(planItem.WorkspaceId, workspace.Id);
            Assert.AreEqual(planItem.PatientId, patientItem.Id);
            Assert.AreEqual(planItem.Id, planSummary.Id);
        }

        [TestMethod]
        public async Task Dose_GetAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, t => t.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var doseSummary = patientItem.Studies[0].Entities[0].Entities[0].Entities[0].Entities[0]; //todo--use FindEntities
            var doseItem = await doseSummary.GetAsync();
            Assert.AreEqual(doseItem.WorkspaceId, workspace.Id);
            Assert.AreEqual(doseItem.PatientId, patientItem.Id);
            Assert.AreEqual(doseItem.Id, doseSummary.Id);
        }
    }
}
