using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var entitySummary = patientItem.FindEntities(e => e.Type == "image_set")[0];
            var imageSetItem = await entitySummary.GetAsync();
            Assert.AreEqual(imageSetItem.WorkspaceId, workspace.Id);
            Assert.AreEqual(imageSetItem.PatientId, patientItem.Id);
            Assert.AreEqual(imageSetItem.Id, entitySummary.Id);
        }

        [TestMethod]
        public async Task StructureSet_GetAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, t => t.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var entitySummary = patientItem.FindEntities(e => e.Type == "structure_set")[0];
            var structureSetItem = await entitySummary.GetAsync();
            Assert.AreEqual(structureSetItem.WorkspaceId, workspace.Id);
            Assert.AreEqual(structureSetItem.PatientId, patientItem.Id);
            Assert.AreEqual(structureSetItem.Id, entitySummary.Id);
        }

        [TestMethod]
        public async Task Plan_GetAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, t => t.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var entitySummary = patientItem.FindEntities(e => e.Type == "plan")[0];
            var planItem = await entitySummary.GetAsync();
            Assert.AreEqual(planItem.WorkspaceId, workspace.Id);
            Assert.AreEqual(planItem.PatientId, patientItem.Id);
            Assert.AreEqual(planItem.Id, entitySummary.Id);
        }

        [TestMethod]
        public async Task Dose_GetAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, t => t.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var entitySummary = patientItem.FindEntities(e => e.Type == "dose")[0];
            var doseItem = await entitySummary.GetAsync();
            Assert.AreEqual(doseItem.WorkspaceId, workspace.Id);
            Assert.AreEqual(doseItem.PatientId, patientItem.Id);
            Assert.AreEqual(doseItem.Id, entitySummary.Id);
        }
    }
}
