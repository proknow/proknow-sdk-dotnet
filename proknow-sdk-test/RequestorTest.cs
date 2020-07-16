using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Exceptions;
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
        private static readonly string _testClassName = nameof(RequestorTest);
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static Requestor _requestor;

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Delete test workspaces, if necessary
            await TestHelper.DeleteWorkspacesAsync(_testClassName);

            // Create requestor
            using StreamReader sr = new StreamReader(TestSettings.CredentialsFile);
            var proKnowCredentials = JsonSerializer.Deserialize<ProKnowCredentials>(sr.ReadToEnd());
            _requestor = new Requestor(TestSettings.BaseUrl, proKnowCredentials.Id, proKnowCredentials.Secret);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);

            // Delete temporary folder
            var outputDocumentPath = Path.Combine(Path.GetTempPath(), _testClassName);
            Directory.Delete(outputDocumentPath, true);
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
        public async Task DeleteAsyncTest_HttpError()
        {
            try
            {
                await _requestor.DeleteAsync($"/invalid/route");
                Assert.Fail();
            }
            catch (ProKnowHttpException ex)
            {
                Assert.IsTrue(ex.Message.Contains("HttpError"));
            }
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
        public async Task GetAsyncTest_HttpError()
        {
            try
            {
                await _requestor.GetAsync($"/invalid/route");
                Assert.Fail();
            }
            catch (ProKnowHttpException ex)
            {
                Assert.IsTrue(ex.Message.Contains("HttpError"));
            }
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
        public async Task PostAsyncTest_HttpError()
        {
            try
            {
                await _requestor.PostAsync($"/invalid/route");
                Assert.Fail();
            }
            catch (ProKnowHttpException ex)
            {
                Assert.IsTrue(ex.Message.Contains("HttpError"));
            }
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
            var properties = new Dictionary<string, object>
            {
                { "slug", workspaceName2.ToLower() },
                { "name", workspaceName2 },
                { "protected", true }
            };
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
        public async Task PutAsyncTest_HttpError()
        {
            try
            {
                await _requestor.PutAsync($"/invalid/route");
                Assert.Fail();
            }
            catch (ProKnowHttpException ex)
            {
                Assert.IsTrue(ex.Message.Contains("HttpError"));
            }
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
            await _proKnow.Patients.Documents.CreateAsync(workspaceItem.Id, patientItem.Id, inputDocumentPath, "test.pdf");

            // Wait until the document processing has completed
            while (true)
            {
                // Query patient documents so we can get the document ID
                var documentSummaries = await _proKnow.Patients.Documents.QueryAsync(workspaceItem.Id, patientItem.Id);
                var documentSummary = documentSummaries.FirstOrDefault(d => d.Name == "test.pdf");
                if (documentSummary != null)
                {
                    // Stream the document to a new file using the Requestor object
                    var outputDocumentPath = Path.Combine(Path.GetTempPath(), _testClassName, testNumber.ToString(), "test.pdf");
                    var documentName = Path.GetFileName(outputDocumentPath);
                    var route = $"/workspaces/{workspaceItem.Id}/patients/{patientItem.Id}/documents/{documentSummary.Id}/{documentSummary.Name}";
                    await _requestor.StreamAsync(route, outputDocumentPath);

                    // Make sure created document and streamed document sizes are the same
                    Assert.AreEqual(documentSummary.Size, new FileInfo(outputDocumentPath).Length);
                    break;
                }
            }
        }

        [TestMethod]
        public async Task StreamAsyncTest_ExistingDirectory()
        {
            int testNumber = 6;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber);

            // Create a document using the Documents object
            var inputDocumentPath = Path.Combine(TestSettings.TestDataRootDirectory, "dummy.pdf");
            await _proKnow.Patients.Documents.CreateAsync(workspaceItem.Id, patientItem.Id, inputDocumentPath, "test.pdf");

            // Wait until the document processing has completed
            while (true)
            {
                // Query patient documents so we can get the document ID
                var documentSummaries = await _proKnow.Patients.Documents.QueryAsync(workspaceItem.Id, patientItem.Id);
                var documentSummary = documentSummaries.FirstOrDefault(d => d.Name == "test.pdf");
                if (documentSummary != null)
                {
                    var route = $"/workspaces/{workspaceItem.Id}/patients/{patientItem.Id}/documents/{documentSummary.Id}/{documentSummary.Name}";
                    var outputDocumentPath = Path.Combine(Path.GetTempPath(), _testClassName, testNumber.ToString());
                    try
                    {
                        // Attempt to stream the document to an existing folder using the Requestor object
                        Directory.CreateDirectory(outputDocumentPath);
                        await _requestor.StreamAsync(route, outputDocumentPath);
                        Assert.Fail();
                    }
                    catch (ProKnowException ex)
                    {
                        Assert.AreEqual($"Cannot stream '{route}' to '{outputDocumentPath}'.  It is a path to an existing directory.", ex.Message);
                        break;
                    }
                }
            }
        }

        [TestMethod]
        public async Task StreamAsyncTest_HttpError()
        {
            try
            {
                await _requestor.StreamAsync("/invalid/route", "./path.dcm");
                Assert.Fail();
            }
            catch (ProKnowHttpException ex)
            {
                Assert.IsTrue(ex.Message.Contains("HttpError"));
            }
        }
    }
}
