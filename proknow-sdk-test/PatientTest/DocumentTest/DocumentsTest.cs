﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Patient.Document.Test
{
    [TestClass]
    public class DocumentsTest
    {
        private static readonly string _testClassName = nameof(DocumentsTest);
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static readonly string _testDocumentPath = Path.Combine(TestSettings.TestDataRootDirectory, "dummy.pdf");
        private static readonly string _testDocumentPath2 = Path.Combine(TestSettings.TestDataRootDirectory, "sample.mp4");
        private static readonly string _outputFolderPath = Path.Combine(Path.GetTempPath(), _testClassName);

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
            // Delete output folder
            if (Directory.Exists(_outputFolderPath))
            {
                Directory.Delete(_outputFolderPath, recursive: true);
            }

            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);
        }

        [TestMethod]
        public async Task CreateAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, 
                Path.Combine("Becker^Matthew", "RD.dcm"));

            // Create the document
            var testDocumentName = $"{_testClassName}-{testNumber}-{Path.GetFileName(_testDocumentPath)}";
            await _proKnow.Patients.Documents.CreateAsync(workspaceItem.Id, patientItem.Id, _testDocumentPath,
                testDocumentName);

            // Wait until the document processing has completed
            while (true)
            {
                // Query patient documents so we can get the document ID
                var documentSummaries = await _proKnow.Patients.Documents.QueryAsync(workspaceItem.Id, patientItem.Id);
                var documentSummary = documentSummaries.FirstOrDefault(d => d.Name == testDocumentName);
                if (documentSummary != null)
                {
                    // Stream document to a new file
                    var outputDocumentPath = Path.Combine(Path.GetTempPath(), _testClassName, testDocumentName);
                    await _proKnow.Patients.Documents.StreamAsync(workspaceItem.Id, patientItem.Id, documentSummary.Id,
                        documentSummary.Name, outputDocumentPath);

                    // Make sure created document and streamed document sizes are the same
                    Assert.AreEqual(documentSummary.Size, new FileInfo(outputDocumentPath).Length);

                    return;
                }
            }
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            var testNumber = 2;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber,
                Path.Combine("Becker^Matthew", "RD.dcm"));

            // Create the document
            var testDocumentName = $"{_testClassName}-{testNumber}-{Path.GetFileName(_testDocumentPath)}";
            await _proKnow.Patients.Documents.CreateAsync(workspaceItem.Id, patientItem.Id, _testDocumentPath,
                testDocumentName);

            // Wait, if necessary, until document processing has completed
            while (true)
            {
                // Query patient documents so we can get the document ID
                var documentSummaries = await _proKnow.Patients.Documents.QueryAsync(workspaceItem.Id, patientItem.Id);
                var documentSummary = documentSummaries.FirstOrDefault(d => d.Name == testDocumentName);
                if (documentSummary != null)
                {
                    // Delete the document
                    await _proKnow.Patients.Documents.DeleteAsync(workspaceItem.Id, patientItem.Id, documentSummary.Id);

                    // Wait, if necessary, until document deletion has completed
                    while (true)
                    {
                        documentSummaries = await _proKnow.Patients.Documents.QueryAsync(workspaceItem.Id, patientItem.Id);
                        documentSummary = documentSummaries.FirstOrDefault(d => d.Name == testDocumentName);
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
            var testNumber = 3;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber,
                Path.Combine("Becker^Matthew", "RD.dcm"));

            // Create the document
            var testDocumentName = $"{_testClassName}-{testNumber}-{Path.GetFileName(_testDocumentPath)}";
            await _proKnow.Patients.Documents.CreateAsync(workspaceItem.Id, patientItem.Id, _testDocumentPath,
                testDocumentName);

            // Wait, if necessary, until document processing has completed
            while (true)
            {
                // Verify the document added is in the query results
                var documentSummaries = await _proKnow.Patients.Documents.QueryAsync(workspaceItem.Id, patientItem.Id);
                var documentSummary = documentSummaries.FirstOrDefault(d => d.Name == testDocumentName);
                if (documentSummary != null)
                {
                    return;
                }
            }
        }

        [TestMethod]
        public async Task StreamAsyncTest()
        {
            var testNumber = 4;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"));

            // Create the documents
            var testDocumentName = $"{_testClassName}-{testNumber}-{Path.GetFileName(_testDocumentPath)}";
            await _proKnow.Patients.Documents.CreateAsync(workspaceItem.Id, patientItem.Id, _testDocumentPath,
                testDocumentName);
            var testDocumentName2 = $"{_testClassName}-{testNumber}-{Path.GetFileName(_testDocumentPath2)}";
            await _proKnow.Patients.Documents.CreateAsync(workspaceItem.Id, patientItem.Id, _testDocumentPath2,
                testDocumentName2);

            // Wait, if necessary, until document processing has completed
            while (true)
            {
                // Query patient documents so we can get the document IDs
                var documentSummaries = await _proKnow.Patients.Documents.QueryAsync(workspaceItem.Id, patientItem.Id);
                var documentSummary = documentSummaries.FirstOrDefault(d => d.Name == testDocumentName);
                var documentSummary2 = documentSummaries.FirstOrDefault(d => d.Name == testDocumentName2);
                if (documentSummary != null && documentSummary2 != null)
                {
                    // Stream documents to new files
                    var outputDocumentPath = Path.Combine(_outputFolderPath, testDocumentName);
                    await _proKnow.Patients.Documents.StreamAsync(workspaceItem.Id, patientItem.Id, documentSummary.Id,
                        documentSummary.Name, outputDocumentPath);
                    var outputDocumentPath2 = Path.Combine(_outputFolderPath, testDocumentName2);
                    await _proKnow.Patients.Documents.StreamAsync(workspaceItem.Id, patientItem.Id, documentSummary2.Id,
                        documentSummary2.Name, outputDocumentPath2);

                    // Make sure created documentsand streamed document sizes are the same
                    Assert.AreEqual(documentSummary.Size, new FileInfo(outputDocumentPath).Length);
                    Assert.AreEqual(documentSummary2.Size, new FileInfo(outputDocumentPath2).Length);

                    return;
                }
            }
        }

        [TestMethod]
        public async Task UpdateAsyncTest()
        {
            var testNumber = 5;

            // Create a test workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"));

            // Create the document
            var testDocumentName = $"{_testClassName}-{testNumber}-{Path.GetFileName(_testDocumentPath)}";
            await _proKnow.Patients.Documents.CreateAsync(workspaceItem.Id, patientItem.Id, _testDocumentPath,
                testDocumentName);

            // Wait until the document processing has completed
            while (true)
            {
                // Query patient documents so we can get the document ID
                var documentSummaries = await _proKnow.Patients.Documents.QueryAsync(workspaceItem.Id, patientItem.Id);
                var documentSummary = documentSummaries.FirstOrDefault(d => d.Name == testDocumentName);
                if (documentSummary != null)
                {
                    // Update the document
                    await _proKnow.Patients.Documents.UpdateAsync(workspaceItem.Id, patientItem.Id, documentSummary.Id,
                        $"updated-{testDocumentName}", "category");

                    // Keep querying patient documents until update is finished
                    documentSummaries = await _proKnow.Patients.Documents.QueryAsync(workspaceItem.Id, patientItem.Id);
                    documentSummary = documentSummaries.FirstOrDefault(d => d.Name == $"updated-{testDocumentName}");
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
