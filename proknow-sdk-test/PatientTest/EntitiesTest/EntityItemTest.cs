using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.Test
{
    [TestClass]
    public class EntityItemTest
    {
        private static readonly string _testClassName = nameof(EntityItemTest);
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

            // Delete custom metrics
            await TestHelper.DeleteCustomMetricsAsync(_testClassName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            var testNumber = 1;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a plan
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RP.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "plan");
            var planItem = await entitySummaries[0].GetAsync() as PlanItem;

            // Delete entity
            await planItem.DeleteAsync();

            // Verify it was deleted
            await patientItem.RefreshAsync();
            entitySummaries = patientItem.FindEntities(t => t.Type == "plan");
            Assert.AreEqual(0, entitySummaries.Count);
        }

        [TestMethod]
        public async Task GetMetadataAsyncTest()
        {
            var testNumber = 2;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a dose
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RD.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "dose");
            var doseItem = await entitySummaries[0].GetAsync() as DoseItem;

            // Create custom metric for testing
            var customMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}", "dose", "number");

            // Set test metadata
            doseItem.Metadata.Add(customMetricItem.Id, 3.141);

            // Get metadata
            var metadata = await doseItem.GetMetadataAsync();

            // Verify metadata
            Assert.AreEqual(1, metadata.Keys.Count);
            Assert.AreEqual(3.141, metadata[customMetricItem.Name]);
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            var testNumber = 3;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with a structure set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "RS.dcm"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "structure_set");
            var structureSetItem = await entitySummaries[0].GetAsync() as StructureSetItem;

            // Create custom metric for testing
            var customMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}", "structure_set", "number");

            // Set description and metadata
            structureSetItem.Description = _testClassName;
            structureSetItem.Metadata.Add(customMetricItem.Id, 1);

            // Save entity changes
            await structureSetItem.SaveAsync();

            // Refresh entity
            await structureSetItem.RefreshAsync();

            // Verify changes were saved
            Assert.AreEqual(_testClassName, structureSetItem.Description);
            Assert.AreEqual(1, structureSetItem.Metadata.Keys.Count);
            Assert.AreEqual(1, structureSetItem.Metadata[customMetricItem.Id]);
        }

        [TestMethod]
        public async Task SetMetadataAsyncTest()
        {
            var testNumber = 4;

            // Create a test workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a test patient with an image set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "CT"), 1);
            var entitySummaries = patientItem.FindEntities(e => e.Type == "image_set");
            var imageSetItem = await entitySummaries[0].GetAsync() as ImageSetItem;

            // Create custom metric for testing
            var customMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}", "image_set", 
                "enum", new string[] { "one", "two", "three" });

            // Set metadata
            var metadata = new Dictionary<string, object>() { { customMetricItem.Name, "two" } };
            await imageSetItem.SetMetadataAsync(metadata);

            // Verify metadata was set
            Assert.AreEqual(1, imageSetItem.Metadata.Keys.Count);
            Assert.AreEqual("two", imageSetItem.Metadata[customMetricItem.Id]);
        }
    }
}
