using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.IO;
using ProKnow.Logs;
using System;

namespace ProKnow.Test.LogTest
{
    [TestClass]
    public class AuditTest
    {
        private static readonly string _testClassName = nameof(AuditTest);
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;

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
        public async Task NextBeforeQueryTest()
        {
            try
            {
                var receivedAuditLogItem = await _proKnow.Audit.Next();
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Must call Query first");
            }
        }

        [TestMethod]
        public async Task QueryTest()
        {
            var testNumber = 1;
            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var newPatient = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"));

            ++testNumber;
            var secondworkspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);
            var secondPatient = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"));

            FilterParameters filterParams = new FilterParameters();
            filterParams.Types = new string[] { "patient_created" };
            filterParams.PageSize = 1;

            var receivedAuditLogItem = await _proKnow.Audit.Query(filterParams);

            Assert.AreEqual(receivedAuditLogItem.Total, (uint)2);

            var patientItem = receivedAuditLogItem.Items[0];

            Assert.AreEqual(patientItem.Classification, "HTTP");
            Assert.AreEqual(patientItem.Method, "POST");
            Assert.AreEqual(patientItem.PatientId, $"{secondPatient.Id}");
            Assert.AreEqual(patientItem.PatientMrn, "2-Mrn");
            Assert.AreEqual(patientItem.PatientName, "2-Name");
            Assert.AreEqual(patientItem.ResourceId, $"{secondPatient.Id}");
            Assert.AreEqual(patientItem.ResourceName, "2-Name");
            Assert.AreEqual(patientItem.StatusCode, "200");
            Assert.AreEqual(patientItem.Uri, $"/workspaces/{secondworkspaceItem.Id}/patients");
            Assert.AreEqual(patientItem.UserName, "Admin");
            Assert.AreEqual(patientItem.WorkspaceId, $"{secondworkspaceItem.Id}");
            Assert.AreEqual(patientItem.WorkspaceName, $"{secondworkspaceItem.Name}");

            receivedAuditLogItem = await _proKnow.Audit.Next();

            Assert.AreEqual(receivedAuditLogItem.Total, (uint)2);

            var nextPatientItem = receivedAuditLogItem.Items[0];

            Assert.AreEqual(nextPatientItem.Classification, "HTTP");
            Assert.AreEqual(nextPatientItem.Method, "POST");
            Assert.AreEqual(nextPatientItem.PatientId, $"{newPatient.Id}");
            Assert.AreEqual(nextPatientItem.PatientMrn, "1-Mrn");
            Assert.AreEqual(nextPatientItem.PatientName, "1-Name");
            Assert.AreEqual(nextPatientItem.ResourceId, $"{newPatient.Id}");
            Assert.AreEqual(nextPatientItem.ResourceName, "1-Name");
            Assert.AreEqual(nextPatientItem.StatusCode, "200");
            Assert.AreEqual(nextPatientItem.Uri, $"/workspaces/{workspaceItem.Id}/patients");
            Assert.AreEqual(nextPatientItem.UserName, "Admin");
            Assert.AreEqual(nextPatientItem.WorkspaceId, $"{workspaceItem.Id}");
            Assert.AreEqual(nextPatientItem.WorkspaceName, $"{workspaceItem.Name}");
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

            try
            {
                var receivedAuditLogItem = await _proKnow.Audit.Query(filterParams);
                Assert.IsNotNull(receivedAuditLogItem);
            }
            catch (Exception e)
            {
                Assert.Fail("Bad Audit.Query filter parameter");
            }
        }
    }
}
