using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProKnow.Test;
using ProKnow.Upload;

namespace ProKnow.Patient.Test
{
    [TestClass]
    public class DocumentsTest
    {
        private static string _patientMrnAndName = "SDK-DocumentsTest";
        private static ProKnow _proKnow = TestSettings.ProKnow;
        private static Uploads _uploads = _proKnow.Uploads;
        private static Documents _documents = _proKnow.Patients.Documents;
        private static string _workspaceId;
        private static string _patientId;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspace, if necessary
            await TestHelper.DeleteWorkspaceAsync(_patientMrnAndName);

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_patientMrnAndName);
            _workspaceId = workspaceItem.Id;

            // Create a test patient
            var patientSummary = await TestHelper.CreatePatientAsync(_patientMrnAndName);
            _patientId = patientSummary.Id;

            // Upload test files
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew");
            var overrides = new UploadFileOverrides
            {
                Patient = new PatientCreateSchema { Name = _patientMrnAndName, Mrn = _patientMrnAndName }
            };
            await _uploads.UploadAsync(_workspaceId, uploadPath, overrides);

            // Wait until uploaded test files have processed
            while (true)
            {
                var patientItem = await patientSummary.GetAsync();
                var entitySummaries = patientItem.FindEntities(e => e.PatientId == patientSummary.Id);
                if (entitySummaries.Count() >= 4)
                {
                    var statuses = entitySummaries.Select(e => e.Status).Distinct();
                    if (statuses.Count() == 1 && statuses.First() == "completed")
                    {
                        break;
                    }
                }
            }
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspace
            await _proKnow.Workspaces.DeleteAsync(_workspaceId);
        }

        [TestMethod]
        public async Task CreateAsyncTest()
        {
            // Create the document
            var inputDocumentPath = Path.Combine(TestSettings.TestDataRootDirectory, "dummy.pdf");
            await _documents.CreateAsync(_workspaceId, _patientId, inputDocumentPath, "CreateAsyncTest.pdf");

            // Wait until the document processing has completed
            while (true)
            {
                // Query patient documents so we can get the document ID
                var documentSummaries = await _documents.QueryAsync(_workspaceId, _patientId);
                var documentSummary = documentSummaries.FirstOrDefault(d => d.Name == "CreateAsyncTest.pdf");
                if (documentSummary != null)
                {
                    // Stream document to a new file
                    var outputDocumentPath = Path.Combine(Path.GetTempPath(), _patientMrnAndName, "CreateAsyncTest.pdf");
                    await _documents.StreamAsync(_workspaceId, _patientId, documentSummary.Id, "CreateAsyncTest.pdf",
                        outputDocumentPath);

                    // Make sure created document and streamed document sizes are the same
                    Assert.AreEqual(documentSummary.Size, new FileInfo(outputDocumentPath).Length);

                    // Delete the streamed document
                    File.Delete(outputDocumentPath);
                    return;
                }
            }
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            // Create a document
            var documentPath = Path.Combine(TestSettings.TestDataRootDirectory, "dummy.pdf");
            await _proKnow.Patients.Documents.CreateAsync(_workspaceId, _patientId, documentPath, "DeleteAsyncTest.pdf");

            // Wait, if necessary, until document processing has completed
            while (true)
            {
                // Query patient documents so we can get the document ID
                var documentSummaries = await _documents.QueryAsync(_workspaceId, _patientId);
                var documentSummary = documentSummaries.FirstOrDefault(d => d.Name == "DeleteAsyncTest.pdf");
                if (documentSummary != null)
                {
                    // Delete the document
                    await _documents.DeleteAsync(_workspaceId, _patientId, documentSummary.Id);

                    // Wait, if necessary, until document deletion has completed
                    while (true)
                    {
                        documentSummaries = await _documents.QueryAsync(_workspaceId, _patientId);
                        documentSummary = documentSummaries.FirstOrDefault(d => d.Name == "DeleteAsyncTest.pdf");
                        if (documentSummary == null)
                        {
                            return;
                        }
                    }
                }
            }
        }

        [TestMethod]
        public async Task QueryAsyncTest()
        {
            // Create a document
            var documentPath = Path.Combine(TestSettings.TestDataRootDirectory, "dummy.pdf");
            await _proKnow.Patients.Documents.CreateAsync(_workspaceId, _patientId, documentPath, "QueryAsyncTest.pdf");

            // Wait, if necessary, until document processing has completed
            while (true)
            {
                // Verify the document added is in the query results
                var documentSummaries = await _proKnow.Patients.Documents.QueryAsync(_workspaceId, _patientId);
                var documentSummary = documentSummaries.FirstOrDefault(d => d.Name == "QueryAsyncTest.pdf");
                if (documentSummary != null)
                {
                    return;
                }
            }
        }

        [TestMethod]
        public async Task StreamAsyncTest()
        {
            // Create a document
            var documentPath = Path.Combine(TestSettings.TestDataRootDirectory, "dummy.pdf");
            await _proKnow.Patients.Documents.CreateAsync(_workspaceId, _patientId, documentPath, "StreamAsyncTest.pdf");

            // Wait, if necessary, until document processing has completed
            while (true)
            {
                // Query patient documents so we can get the document ID
                var documentSummaries = await _documents.QueryAsync(_workspaceId, _patientId);
                var documentSummary = documentSummaries.FirstOrDefault(d => d.Name == "StreamAsyncTest.pdf");
                if (documentSummary != null)
                {
                    // Stream document to a new file
                    var outputDocumentPath = Path.Combine(Path.GetTempPath(), _patientMrnAndName, "StreamAsyncTest.pdf");
                    await _documents.StreamAsync(_workspaceId, _patientId, documentSummary.Id, "StreamAsyncTest.pdf",
                        outputDocumentPath);

                    // Make sure created document and streamed document sizes are the same
                    Assert.AreEqual(documentSummary.Size, new FileInfo(outputDocumentPath).Length);

                    // Delete the streamed document
                    File.Delete(outputDocumentPath);
                    return;
                }
            }
        }

        [TestMethod]
        public async Task UpdateAsyncTest()
        {
            // Create the document
            var originalDocumentPath = Path.Combine(TestSettings.TestDataRootDirectory, "dummy.pdf");
            await _documents.CreateAsync(_workspaceId, _patientId, originalDocumentPath, "UpdateAsyncTest.pdf");

            // Wait until the document processing has completed
            while (true)
            {
                // Query patient documents so we can get the document ID
                var documentSummaries = await _documents.QueryAsync(_workspaceId, _patientId);
                var documentSummary = documentSummaries.FirstOrDefault(d => d.Name == "UpdateAsyncTest.pdf");
                if (documentSummary != null)
                {
                    // Update the document
                    await _documents.UpdateAsync(_workspaceId, _patientId, documentSummary.Id, "UpdateAsyncTest2.pdf", "category");

                    // Keep querying patient documents until update is finished
                    documentSummaries = await _documents.QueryAsync(_workspaceId, _patientId);
                    documentSummary = documentSummaries.FirstOrDefault(d => d.Name == "UpdateAsyncTest2.pdf");
                    if (documentSummary != null)
                    {
                        Assert.AreEqual("category", documentSummary.Category);
                        return;
                    }
                }
            }
        }
    }
}
