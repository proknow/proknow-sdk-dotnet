using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.IO;
using ProKnow.Audit;
using System;
using ProKnow.Patient;
using ProKnow.Exceptions;
using ProKnow.Test;
using System.Text.Json;
using ProKnow.User;

namespace ProKnow.Audit.Test
{
    [TestClass]
    public class AuditTest
    {
        private static readonly string _testClassName = nameof(AuditTest);
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static WorkspaceItem _workspaceItemTwo;
        private static PatientItem _patientTwo;

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Cleanup from previous test stoppage or failure, if necessary
            await ClassCleanup();
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);
        }

        [TestMethod]
        [ExpectedException(typeof(ProKnowException), "Must call Query first")]
        public async Task NextBeforeQueryTest()
        {
            AuditPage page = new AuditPage();
            await page.Next();           
        }

        [TestMethod]
        public async Task QueryTest()
        {
            var testNumber = 123456;
            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"));

            ++testNumber;
            _workspaceItemTwo = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);
            _patientTwo = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"));

            var json = await _proKnow.Requestor.GetAsync($"/user");
            var userItem = JsonSerializer.Deserialize<UserItem>(json);

            FilterParameters filterParams = new FilterParameters();
            filterParams.WorkspaceId = _workspaceItemTwo.Id;
            filterParams.PageSize = 1;

            var auditPage = await _proKnow.Audit.Query(filterParams);

            Assert.AreEqual(auditPage.Total, (uint)4);

            var patientItem = auditPage.Items[0];

            Assert.AreEqual(patientItem.Classification, "HTTP");
            Assert.AreEqual(patientItem.Method, "GET");
            Assert.AreEqual(patientItem.PatientId, $"{_patientTwo.Id}");
            Assert.AreEqual(patientItem.PatientMrn, "123457-Mrn");
            Assert.AreEqual(patientItem.PatientName, "123457-Name");
            Assert.AreEqual(patientItem.ResourceId, $"{_patientTwo.Id}");
            Assert.AreEqual(patientItem.ResourceName, "123457-Name");
            Assert.AreEqual(patientItem.StatusCode, "200");
            Assert.AreEqual(patientItem.Uri, $"/workspaces/{_workspaceItemTwo.Id}/patients/{_patientTwo.Id}");
            Assert.AreEqual(patientItem.UserName, userItem.Name);
            Assert.AreEqual(patientItem.WorkspaceId, $"{_workspaceItemTwo.Id}");
            Assert.AreEqual(patientItem.WorkspaceName, $"{_workspaceItemTwo.Name}");

            await auditPage.Next();
            auditPage = await auditPage.Next();

            Assert.AreEqual(auditPage.Total, (uint)4);

            var nextPatientItem = auditPage.Items[0];

            Assert.AreEqual(nextPatientItem.Classification, "HTTP");
            Assert.AreEqual(nextPatientItem.Method, "POST");
            Assert.AreEqual(nextPatientItem.PatientId, $"{_patientTwo.Id}");
            Assert.AreEqual(nextPatientItem.PatientMrn, "123457-Mrn");
            Assert.AreEqual(nextPatientItem.PatientName, "123457-Name");
            Assert.AreEqual(nextPatientItem.ResourceId, $"{_patientTwo.Id}");
            Assert.AreEqual(nextPatientItem.ResourceName, "123457-Name");
            Assert.AreEqual(nextPatientItem.StatusCode, "200");
            Assert.AreEqual(nextPatientItem.Uri, $"/workspaces/{_workspaceItemTwo.Id}/patients");
            Assert.AreEqual(nextPatientItem.UserName, userItem.Name);
            Assert.AreEqual(nextPatientItem.WorkspaceId, $"{_workspaceItemTwo.Id}");
            Assert.AreEqual(nextPatientItem.WorkspaceName, $"{_workspaceItemTwo.Name}");
        }

        [TestMethod]
        public async Task FilterParametersTest()
        {
            FilterParameters filterParams = new FilterParameters();
            filterParams.Types = new string[] { "patient_created" };
            filterParams.PageSize = 1;
            filterParams.StartTime = DateTime.Now.AddDays(-1); 
            filterParams.EndTime = DateTime.Now;
            filterParams.UserName ="Admin";
            filterParams.PatientName = "2-Name";
            filterParams.Classification = "HTTP";
            filterParams.Methods = new string[] { "POST" };
            filterParams.URI = $"/workspaces/1234/patients";
            filterParams.UserAgent = "1234";
            filterParams.IpAddress = "127.0.0.1";
            filterParams.StatusCodes = new string[] { "200" };
            filterParams.WorkspaceId = "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee";
            filterParams.ResourceId = "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee";

            var receivedAuditLogItem = await _proKnow.Audit.Query(filterParams);
            Assert.IsNotNull(receivedAuditLogItem);
        }
    }
}
