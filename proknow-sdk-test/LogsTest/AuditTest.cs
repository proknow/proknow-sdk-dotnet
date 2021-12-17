using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.IO;
using ProKnow.Logs;
using System;
using ProKnow.Patient;
using ProKnow.Exceptions;

namespace ProKnow.Test.LogTest
{
    [TestClass]
    public class AuditTest
    {
        private static readonly string _testClassName = nameof(AuditTest);
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static WorkspaceItem _workspaceItemOne;
        private static WorkspaceItem _workspaceItemTwo;
        private static PatientItem _patientOne;
        private static PatientItem _patientTwo;

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Cleanup from previous test stoppage or failure, if necessary
            await ClassCleanup();

            var testNumber = 123456;
            // Create a test workspace
            _workspaceItemOne = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            _patientOne = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"));

            ++testNumber;
            _workspaceItemTwo = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);
            _patientTwo = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"));
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
            var receivedAuditLogItem = await _proKnow.Audit.Next();           
        }

        [TestMethod]
        public async Task QueryTest()
        {
            FilterParameters filterParams = new FilterParameters();
            filterParams.WorkspaceId = _workspaceItemTwo.Id;
            filterParams.PageSize = 1;

            var receivedAuditLogItem = await _proKnow.Audit.Query(filterParams);

            Assert.AreEqual(receivedAuditLogItem.Total, (uint)4);

            var patientItem = receivedAuditLogItem.Items[0];

            Assert.AreEqual(patientItem.Classification, "HTTP");
            Assert.AreEqual(patientItem.Method, "GET");
            Assert.AreEqual(patientItem.PatientId, $"{_patientTwo.Id}");
            Assert.AreEqual(patientItem.PatientMrn, "123457-Mrn");
            Assert.AreEqual(patientItem.PatientName, "123457-Name");
            Assert.AreEqual(patientItem.ResourceId, $"{_patientTwo.Id}");
            Assert.AreEqual(patientItem.ResourceName, "123457-Name");
            Assert.AreEqual(patientItem.StatusCode, "200");
            Assert.AreEqual(patientItem.Uri, $"/workspaces/{_workspaceItemTwo.Id}/patients/{_patientTwo.Id}");
            Assert.AreEqual(patientItem.UserName, "Admin");
            Assert.AreEqual(patientItem.WorkspaceId, $"{_workspaceItemTwo.Id}");
            Assert.AreEqual(patientItem.WorkspaceName, $"{_workspaceItemTwo.Name}");

            await _proKnow.Audit.Next();
            receivedAuditLogItem = await _proKnow.Audit.Next();

            Assert.AreEqual(receivedAuditLogItem.Total, (uint)4);

            var nextPatientItem = receivedAuditLogItem.Items[0];

            Assert.AreEqual(nextPatientItem.Classification, "HTTP");
            Assert.AreEqual(nextPatientItem.Method, "POST");
            Assert.AreEqual(nextPatientItem.PatientId, $"{_patientTwo.Id}");
            Assert.AreEqual(nextPatientItem.PatientMrn, "123457-Mrn");
            Assert.AreEqual(nextPatientItem.PatientName, "123457-Name");
            Assert.AreEqual(nextPatientItem.ResourceId, $"{_patientTwo.Id}");
            Assert.AreEqual(nextPatientItem.ResourceName, "123457-Name");
            Assert.AreEqual(nextPatientItem.StatusCode, "200");
            Assert.AreEqual(nextPatientItem.Uri, $"/workspaces/{_workspaceItemTwo.Id}/patients");
            Assert.AreEqual(nextPatientItem.UserName, "Admin");
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

            try
            {
                var receivedAuditLogItem = await _proKnow.Audit.Query(filterParams);
                Assert.IsNotNull(receivedAuditLogItem);
            }
            catch (Exception)
            {
                Assert.Fail("Bad Audit.Query filter parameter");
            }
        }
    }
}
