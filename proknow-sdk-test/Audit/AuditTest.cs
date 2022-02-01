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
        private static WorkspaceItem _workspaceItemOne;
        private static PatientItem _patientOne;
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

        [Ignore("This test fails checking auditPage.Total because ProKnow probably already has some audit log entries prior to running the tests and the tests are being run in parallel, causing more audit log entries to be generated.")]
        [TestMethod]
        public async Task QueryTest()
        {
            var testNumber = 123456;
            // Create a test workspace
            _workspaceItemOne = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            _patientOne = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"));
            await TestHelper.DeleteWorkspacesAsync(_workspaceItemOne.Name);

            var json = await _proKnow.Requestor.GetAsync($"/user");
            var userItem = JsonSerializer.Deserialize<UserItem>(json);

            FilterParameters filterParams = new FilterParameters();
            filterParams.PageSize = 1;
            filterParams.Types = new string[] { "patient_read", "workspace_deleted" };
            filterParams.WorkspaceId = _workspaceItemOne.Id;
            var auditPage = await _proKnow.Audit.Query(filterParams);

            Assert.AreEqual(auditPage.Total, (uint)2);

            var patientItem = auditPage.Items[0];

            Assert.AreEqual(patientItem.Classification, "HTTP");
            Assert.AreEqual(patientItem.Method, "DELETE");
            Assert.AreEqual(patientItem.PatientId, null);
            Assert.AreEqual(patientItem.PatientMrn, null);
            Assert.AreEqual(patientItem.PatientName, null);
            Assert.AreEqual(patientItem.ResourceId, $"{_workspaceItemOne.Id}");
            Assert.AreEqual(patientItem.ResourceName, _workspaceItemOne.Name);
            Assert.AreEqual(patientItem.StatusCode, "200");
            Assert.AreEqual(patientItem.Uri, $"/workspaces/{_workspaceItemOne.Id}");
            Assert.AreEqual(patientItem.UserName, userItem.Name);
            Assert.AreEqual(patientItem.WorkspaceId, $"{_workspaceItemOne.Id}");
            Assert.AreEqual(patientItem.WorkspaceName, null);

            var audit2Page = await auditPage.Next();

            Assert.AreEqual(audit2Page.Total, (uint)2);

            var nextPatientItem = audit2Page.Items[0];

            Assert.AreEqual(nextPatientItem.Classification, "HTTP");
            Assert.AreEqual(nextPatientItem.Method, "GET");
            Assert.AreEqual(nextPatientItem.PatientId, $"{_patientOne.Id}");
            Assert.AreEqual(nextPatientItem.PatientMrn, "123456-Mrn");
            Assert.AreEqual(nextPatientItem.PatientName, "123456-Name");
            Assert.AreEqual(nextPatientItem.ResourceId, $"{_patientOne.Id}");
            Assert.AreEqual(nextPatientItem.ResourceName, "123456-Name");
            Assert.AreEqual(nextPatientItem.StatusCode, "200");
            Assert.AreEqual(nextPatientItem.Uri, $"/workspaces/{_workspaceItemOne.Id}/patients/{_patientOne.Id}");
            Assert.AreEqual(nextPatientItem.UserName, userItem.Name);
            Assert.AreEqual(nextPatientItem.WorkspaceId, $"{_workspaceItemOne.Id}");
            Assert.AreEqual(nextPatientItem.WorkspaceName, $"{_workspaceItemOne.Name}");

            //Call Next() on auditPage to verify it still retrieves Page 2 data
            var audit2PageAgain = await auditPage.Next();
            Assert.AreEqual(audit2PageAgain.Total, (uint)2);

            var lastPatientItem = audit2PageAgain.Items[0];

            Assert.AreEqual(lastPatientItem.Classification, "HTTP");
            Assert.AreEqual(lastPatientItem.Method, "GET");
            Assert.AreEqual(lastPatientItem.PatientId, $"{_patientOne.Id}");
            Assert.AreEqual(lastPatientItem.PatientMrn, "123456-Mrn");
            Assert.AreEqual(lastPatientItem.PatientName, "123456-Name");
            Assert.AreEqual(lastPatientItem.ResourceId, $"{_patientOne.Id}");
            Assert.AreEqual(lastPatientItem.ResourceName, "123456-Name");
            Assert.AreEqual(lastPatientItem.StatusCode, "200");
            Assert.AreEqual(lastPatientItem.Uri, $"/workspaces/{_workspaceItemOne.Id}/patients/{_patientOne.Id}");
            Assert.AreEqual(lastPatientItem.UserName, userItem.Name);
            Assert.AreEqual(lastPatientItem.WorkspaceId, $"{_workspaceItemOne.Id}");
            Assert.AreEqual(lastPatientItem.WorkspaceName, $"{_workspaceItemOne.Name}");
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
