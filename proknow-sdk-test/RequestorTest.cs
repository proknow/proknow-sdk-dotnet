using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProKnow.Test
{
    [TestClass]
    public class RequestorTest
    {
        private static string _testClassName = nameof(RequestorTest);
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Requestor _requestor;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspaces, if necessary
            await TestHelper.DeleteWorkspacesAsync(_testClassName);

            // Create requestor
            using (StreamReader sr = new StreamReader(TestSettings.CredentialsFile))
            {
                var proKnowCredentials = JsonSerializer.Deserialize<ProKnowCredentials>(sr.ReadToEnd());
                _requestor = new Requestor(TestSettings.BaseUrl, proKnowCredentials.Id, proKnowCredentials.Secret);
            }
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            int testNumber = 1;

            // Create a workspace using the Workspaces object
            var workspaceName = $"SDK-{_testClassName}-{testNumber}";
            var workspaceItem = await _proKnow.Workspaces.CreateAsync(workspaceName.ToLower(), workspaceName, false);

            // Verify that the workspace exists
            Assert.IsNotNull(await _proKnow.Workspaces.FindAsync(w => w.Name == workspaceName));

            // Delete the workspace using the Requestor
            await _requestor.DeleteAsync($"/workspaces/{workspaceItem.Id}");

            // Verify that the workspace no longer exists
            var workspaceItems = await _proKnow.Workspaces.QueryAsync();
            Assert.IsFalse(workspaceItems.Any(w => w.Name == workspaceName));
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            int testNumber = 2;

            // Create a workspace using the Workspaces object
            var workspaceName = $"SDK-{_testClassName}-{testNumber}";
            var workspaceItem = await _proKnow.Workspaces.CreateAsync(workspaceName.ToLower(), workspaceName, false);

            // Get the workspaces using the Requestor
            string json = await _requestor.GetAsync($"/workspaces");
            var workspaceItems = JsonSerializer.Deserialize<IList<WorkspaceItem>>(json);

            // Verify that the created workspace was one of the workspaces returned
            Assert.IsNotNull(await _proKnow.Workspaces.FindAsync(w => w.Name == workspaceName));
        }

        [TestMethod]
        public async Task PostAsyncTest()
        {
            int testNumber = 3;

            // Create a workspace using the Requestor
            var workspaceName = $"SDK-{_testClassName}-{testNumber}";
            var workspaceItem = new WorkspaceItem { Slug = workspaceName.ToLower(), Name = workspaceName, Protected = false };
            var jsonSerializerOptions = new JsonSerializerOptions { IgnoreNullValues = true };
            var json = JsonSerializer.Serialize(workspaceItem, jsonSerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _requestor.PostAsync("/workspaces", null, content);

            // Verify that the workspace exists
            var workspaceItems = await _proKnow.Workspaces.QueryAsync();
            Assert.IsTrue(workspaceItems.Any(w => w.Name == workspaceName));
        }

        [TestMethod]
        public async Task PutAsyncTest()
        {
            int testNumber = 4;

            // Create a workspace using the Workspaces object
            var workspaceName = $"SDK-{_testClassName}-{testNumber}";
            var workspaceItem = await _proKnow.Workspaces.CreateAsync(workspaceName.ToLower(), workspaceName, false);

            // Make and save workspace changes using the Requestor
            var workspaceName2 = $"SDK-{_testClassName}-{testNumber}-A";
            var properties = new Dictionary<string, object>();
            properties.Add("slug", workspaceName2.ToLower());
            properties.Add("name", workspaceName2);
            properties.Add("protected", true);
            var content = new StringContent(JsonSerializer.Serialize(properties), Encoding.UTF8, "application/json");
            await _requestor.PutAsync($"/workspaces/{workspaceItem.Id}", null, content);

            // Retrieve the workspace
            workspaceItem = await _proKnow.Workspaces.FindAsync(w => w.Id == workspaceItem.Id);

            // Verify that workspace changes were saved
            Assert.AreEqual(properties["slug"], workspaceItem.Slug);
            Assert.AreEqual(properties["name"], workspaceItem.Name);
            Assert.AreEqual(properties["protected"], workspaceItem.Protected);
        }

        [TestMethod]
        public async Task StreamAsyncTest()
        {
            int testNumber = 5;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Create a document using the Documents object
            var inputDocumentPath = Path.Combine(TestSettings.TestDataRootDirectory, "dummy.pdf");
            await _proKnow.Patients.Documents.CreateAsync(workspaceItem.Id, patientItem.Id, inputDocumentPath, "StreamAsyncTest.pdf");

            // Wait until the document processing has completed
            while (true)
            {
                // Query patient documents so we can get the document ID
                var documentSummaries = await _proKnow.Patients.Documents.QueryAsync(workspaceItem.Id, patientItem.Id);
                var documentSummary = documentSummaries.FirstOrDefault(d => d.Name == "StreamAsyncTest.pdf");
                if (documentSummary != null)
                {
                    // Stream the document to a new file using the Requestor object
                    var outputDocumentPath = Path.Combine(Path.GetTempPath(), _testClassName, "StreamAsyncTest.pdf");
                    var documentName = Path.GetFileName(outputDocumentPath);
                    var route = $"/workspaces/{workspaceItem.Id}/patients/{patientItem.Id}/documents/{documentSummary.Id}/{documentName}";
                    await _requestor.StreamAsync(route, outputDocumentPath);

                    // Make sure created document and streamed document sizes are the same
                    Assert.AreEqual(documentSummary.Size, new FileInfo(outputDocumentPath).Length);
                    break;
                }
            }
        }
    }
}
