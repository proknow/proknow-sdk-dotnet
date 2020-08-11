using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Patient.Entities;
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
        private static readonly string _downloadFolderRoot = Path.Combine(Path.GetTempPath(), _testClassName);

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Cleanup from previous test stoppage or failure, if necessary
            await ClassCleanup();

            // Create download folder root
            Directory.CreateDirectory(_downloadFolderRoot);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);

            // Delete download folder
            if (Directory.Exists(_downloadFolderRoot))
            {
                Directory.Delete(_downloadFolderRoot, true);
            }
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            int testNumber = 1;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Verify that the workspace exists
            Assert.IsNotNull(await _proKnow.Workspaces.FindAsync(w => w.Name == workspaceItem.Name));

            // Delete the workspace using the Requestor
            await _proKnow.Requestor.DeleteAsync($"/workspaces/{workspaceItem.Id}");

            // Verify that the workspace no longer exists
            var workspaceItems = await _proKnow.Workspaces.QueryAsync();
            Assert.IsFalse(workspaceItems.Any(w => w.Name == workspaceItem.Name));
        }

        [TestMethod]
        public async Task DeleteAsyncTest_HttpError()
        {
            try
            {
                await _proKnow.Requestor.DeleteAsync($"/invalid/route");
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
            int testNumber = 3;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Get the workspaces using the Requestor
            string json = await _proKnow.Requestor.GetAsync($"/workspaces");
            var workspaceItems = JsonSerializer.Deserialize<IList<WorkspaceItem>>(json);

            // Verify that the created workspace was one of the workspaces returned
            Assert.IsTrue(workspaceItems.Any(w => w.Name == workspaceItem.Name));
        }

        [TestMethod]
        public async Task GetAsyncTest_HttpError()
        {
            try
            {
                await _proKnow.Requestor.GetAsync($"/invalid/route");
                Assert.Fail();
            }
            catch (ProKnowHttpException ex)
            {
                Assert.IsTrue(ex.Message.Contains("HttpError"));
            }
        }

        [TestMethod]
        public async Task GetBinaryAsyncTest()
        {
            int testNumber = 5;

            // Create a workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient with an image set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "CT"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "image_set");
            var imageSetItem = await entitySummaries[0].GetAsync() as ImageSetItem;

            // Get the data for the first image
            var image = imageSetItem.Data.Images.First(i => i.Uid == "1.3.6.1.4.1.22213.2.26558.2.61");
            var headerKeyValuePairs = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("ProKnow-Key", imageSetItem.Key) };
            var bytes = await _proKnow.Requestor.GetBinaryAsync($"/imagesets/{imageSetItem.Id}/images/{image.Tag}", headerKeyValuePairs);

            // Verify the data
            Assert.AreEqual(512 * 512 * 2, bytes.Length);
            Assert.AreEqual(32, bytes[401]);
            Assert.AreEqual(0, bytes[402]);
            Assert.AreEqual(41, bytes[403]);
            Assert.AreEqual(0, bytes[404]);
            Assert.AreEqual(46, bytes[405]);
            Assert.AreEqual(0, bytes[406]);
            Assert.AreEqual(47, bytes[407]);
            Assert.AreEqual(00, bytes[408]);
            Assert.AreEqual(48, bytes[409]);
            Assert.AreEqual(0, bytes[410]);
        }

        [TestMethod]
        public async Task GetBinaryAsyncTest_HttpError()
        {
            try
            {
                await _proKnow.Requestor.GetBinaryAsync($"/invalid/route");
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
            int testNumber = 7;

            // Create a workspace using the Requestor
            var workspaceName = $"SDK-{_testClassName}-{testNumber}";
            var workspaceItem = new WorkspaceItem { Slug = workspaceName.ToLower(), Name = workspaceName, Protected = false };
            var jsonSerializerOptions = new JsonSerializerOptions { IgnoreNullValues = true };
            var json = JsonSerializer.Serialize(workspaceItem, jsonSerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PostAsync("/workspaces", null, content);

            // Verify that the workspace exists
            var workspaceItems = await _proKnow.Workspaces.QueryAsync();
            Assert.IsTrue(workspaceItems.Any(w => w.Name == workspaceName));
        }

        [TestMethod]
        public async Task PostAsyncTest_HttpError()
        {
            try
            {
                await _proKnow.Requestor.PostAsync($"/invalid/route");
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
            int testNumber = 9;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Make and save workspace changes using the Requestor
            var workspaceName2 = $"SDK-{_testClassName}-{testNumber}-A";
            var properties = new Dictionary<string, object>
            {
                { "slug", workspaceName2.ToLower() },
                { "name", workspaceName2 },
                { "protected", true }
            };
            var content = new StringContent(JsonSerializer.Serialize(properties), Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PutAsync($"/workspaces/{workspaceItem.Id}", null, content);

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
                await _proKnow.Requestor.PutAsync($"/invalid/route");
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
            int testNumber = 11;

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
                    var outputDocumentPath = Path.Combine(_downloadFolderRoot, testNumber.ToString(), "test.pdf");
                    var route = $"/workspaces/{workspaceItem.Id}/patients/{patientItem.Id}/documents/{documentSummary.Id}/{documentSummary.Name}";
                    await _proKnow.Requestor.StreamAsync(route, outputDocumentPath);

                    // Make sure created document and streamed document sizes are the same
                    Assert.AreEqual(documentSummary.Size, new FileInfo(outputDocumentPath).Length);
                    break;
                }
            }
        }

        [TestMethod]
        public async Task StreamAsyncTest_ExistingDirectory()
        {
            int testNumber = 12;

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
                    var outputDocumentPath = Path.Combine(_downloadFolderRoot, testNumber.ToString());
                    try
                    {
                        // Attempt to stream the document to an existing folder using the Requestor object
                        Directory.CreateDirectory(outputDocumentPath);
                        await _proKnow.Requestor.StreamAsync(route, outputDocumentPath);
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
                await _proKnow.Requestor.StreamAsync("/invalid/route", "./path.dcm");
                Assert.Fail();
            }
            catch (ProKnowHttpException ex)
            {
                Assert.IsTrue(ex.Message.Contains("HttpError"));
            }
        }
    }
}
