using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
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
            var imageSetEntities = patientItem.FindEntities(e => e.Data["type"].ToString() == "image_set");
            Assert.AreEqual(imageSetEntities.Count, 1);
        }

        [TestMethod]
        public async Task FindEntitiesTest_Predicate_2nd_Level()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, p => p.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var structureSetEntities = patientItem.FindEntities(e => e.Data["type"].ToString() == "structure_set");
            Assert.AreEqual(structureSetEntities.Count, 1);
        }

        [TestMethod]
        public async Task FindEntitiesTest_Predicate_3rd_Level()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, p => p.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var planEntities = patientItem.FindEntities(e => e.Data["type"].ToString() == "plan");
            Assert.AreEqual(planEntities.Count, 1);
        }

        [TestMethod]
        public async Task FindEntitiesTest_Predicate_4th_Level()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
            var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, p => p.Name == TestSettings.TestPatientName);
            var patientItem = await patientSummary.GetAsync();
            var doseEntities = patientItem.FindEntities(e => e.Data["type"].ToString() == "dose");
            Assert.AreEqual(doseEntities.Count, 1);
        }

        //todo--these tests fail because Equals fails when Data[] is JsonElement and property is string, e.g.

        //[TestMethod]
        //public async Task FindEntitiesTest_Properties_1st_Level()
        //{
        //    var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
        //    var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
        //    var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, p => p.Name == TestSettings.TestPatientName);
        //    var patientItem = await patientSummary.GetAsync();
        //    var imageSetEntities = patientItem.FindEntities(null, new KeyValuePair<string, object>("type", "image_set"));
        //    Assert.AreEqual(imageSetEntities.Count, 1);
        //}

        //[TestMethod]
        //public async Task FindEntitiesTest_Properties_2nd_Level()
        //{
        //    var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
        //    var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
        //    var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, p => p.Name == TestSettings.TestPatientName);
        //    var patientItem = await patientSummary.GetAsync();
        //    var structureSetEntities = patientItem.FindEntities(null, new KeyValuePair<string, object>("type", "structure_set"));
        //    Assert.AreEqual(structureSetEntities.Count, 1);
        //}

        //[TestMethod]
        //public async Task FindEntitiesTest_Properties_3rd_Level()
        //{
        //    var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
        //    var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
        //    var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, p => p.Name == TestSettings.TestPatientName);
        //    var patientItem = await patientSummary.GetAsync();
        //    var planEntities = patientItem.FindEntities(null, new KeyValuePair<string, object>("type", "plan"));
        //    Assert.AreEqual(planEntities.Count, 1);
        //}

        //[TestMethod]
        //public async Task FindEntitiesTest_Properties_4th_Level()
        //{
        //    var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
        //    var workspace = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);
        //    var patientSummary = await proKnow.Patients.FindAsync(workspace.Id, p => p.Name == TestSettings.TestPatientName);
        //    var patientItem = await patientSummary.GetAsync();
        //    var doseEntities = patientItem.FindEntities(null, new KeyValuePair<string, object>("type", "dose"));
        //    Assert.AreEqual(doseEntities.Count, 1);
        //}
    }
}
