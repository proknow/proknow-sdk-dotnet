﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class DoseItemTest
    {
        private static readonly string _testClassName = nameof(DoseItemTest);
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
        public async Task DownloadAsyncTest_Directory()
        {
            var testNumber = 1;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "dose");
            var doseItem = await entitySummaries[0].GetAsync() as DoseItem;

            // Download the entity to an existing directory using the default filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            Directory.CreateDirectory(downloadFolder);
            string expectedDownloadPath = Path.Combine(downloadFolder, $"RD.{doseItem.Uid}.dcm");
            string actualDownloadPath = await doseItem.DownloadAsync(downloadFolder);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_ExistingFileWithExistingParent()
        {
            var testNumber = 2;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "dose");
            var doseItem = await entitySummaries[0].GetAsync() as DoseItem;

            // Download the entity to an existing filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            Directory.CreateDirectory(downloadFolder);
            string expectedDownloadPath = Path.Combine(downloadFolder, "RD.dcm");
            File.WriteAllText(expectedDownloadPath, "This is an existing file!");
            string actualDownloadPath = await doseItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_NewFileWithExistingParent()
        {
            var testNumber = 3;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "dose");
            var doseItem = await entitySummaries[0].GetAsync() as DoseItem;

            // Download the entity to an existing directory using a specified filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            string expectedDownloadPath = Path.Combine(downloadFolder, "RD.dcm");
            string actualDownloadPath = await doseItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }

        [TestMethod]
        public async Task DownloadAsyncTest_NewFileWithNonexistingParent()
        {
            var testNumber = 4;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "dose");
            var doseItem = await entitySummaries[0].GetAsync() as DoseItem;

            // Download the entity to an nonexisting directory using a specified filename
            string downloadFolder = Path.Combine(_downloadFolderRoot, testNumber.ToString());
            string expectedDownloadPath = Path.Combine(downloadFolder, "grandparent", "parent", "RD.dcm");
            string actualDownloadPath = await doseItem.DownloadAsync(expectedDownloadPath);

            // Make sure it was downloaded to the expected path
            Assert.AreEqual(expectedDownloadPath, actualDownloadPath);

            // Compare it to the uploaded one
            var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, "Becker^Matthew", "RD.dcm");
            Assert.IsTrue(TestHelper.FileEquals(uploadPath, actualDownloadPath));
        }

        [TestMethod]
        public async Task GetSliceDataAsyncTest()
        {
            var testNumber = 5;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a dose
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "dose");
            var doseItem = await entitySummaries[0].GetAsync() as DoseItem;

            // Get the data for this first dose slice
            var voxelData = await doseItem.GetSliceDataAsync(0);

            // Verify the data
            Assert.AreEqual(doseItem.Data.ResolutionX * doseItem.Data.ResolutionZ, voxelData.Length);
            var intercept = doseItem.Data.PixelIntercept;
            var slope = doseItem.Data.PixelSlope;
            var tolerance = 0.5 * slope; // half of a pixel
            Assert.AreEqual(5.0873445503321e-06 * uint.Parse("00009901", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * voxelData[83], tolerance);
            Assert.AreEqual(5.0873445503321e-06 * uint.Parse("00009901", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * voxelData[84], tolerance);
            Assert.AreEqual(5.0873445503321e-06 * uint.Parse("00008801", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * voxelData[85], tolerance);
            Assert.AreEqual(5.0873445503321e-06 * uint.Parse("00007b00", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * voxelData[86], tolerance);
            Assert.AreEqual(5.0873445503321e-06 * uint.Parse("0000b601", System.Globalization.NumberStyles.AllowHexSpecifier), intercept + slope * voxelData[87], tolerance);
        }
    }
}
