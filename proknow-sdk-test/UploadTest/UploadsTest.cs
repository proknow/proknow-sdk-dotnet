using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ProKnow.Patient;
using ProKnow.Test;

namespace ProKnow.Upload.Test
{
    [TestClass]
    public class UploadsTest
    {
        [TestMethod]
        public async Task UploadFileAsyncTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var uploads = new Uploads(proKnow);
            var workspaceItem = await proKnow.Workspaces.FindAsync(t => t.Name == TestSettings.TestWorkspaceName);

            // Create a test patient
            var patientIdAndName = "UploadsTest_UploadFileAsyncTest";
            await proKnow.Patients.CreateAsync(TestSettings.TestWorkspaceName, patientIdAndName, patientIdAndName, "2020-02-02", "F");

            // Upload a file with overrides
            var file = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RS.dcm");
            var overrides = new UploadFileOverrides {
                Patient = new PatientMetadata { Name = patientIdAndName, Mrn = patientIdAndName, BirthDate = "2020-02-02", Sex = "F" }};
            var result = await uploads.UploadFileAsync(workspaceItem.Id, file, overrides);

            // Make sure the file was uploaded
            var patientSummary = await proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Mrn == patientIdAndName);
            var patientItem = await patientSummary.GetAsync();
            while (true)
            {
                var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
                if (entitySummaries.Count > 0)
                {
                    Assert.AreEqual(entitySummaries[0].Uid, "1.2.840.10008.5.1.4.1.1.481.3.1535997926");
                    break;
                }
            }

            // Delete the test patient
            await proKnow.Patients.DeleteAsync(workspaceItem.Id, patientItem.Id);
        }
    }
}
