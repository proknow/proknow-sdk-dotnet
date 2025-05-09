using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Patient.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using static ProKnow.RtvRequestor;
using System.Text.Json;

namespace ProKnow.Test
{
    [TestClass]
    public class RtvRequestorTest
    {
        private static readonly string _testClassName = nameof(RtvRequestorTest);
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
            // Delete test custom metrics
            await TestHelper.DeleteCustomMetricsAsync(_testClassName);

            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);

            // Delete download folder
            if (Directory.Exists(_downloadFolderRoot))
            {
                Directory.Delete(_downloadFolderRoot, true);
            }
        }

        [TestMethod]
        public async Task GetBinaryAsyncTest()
        {
            int testNumber = 7;

            // Create a workspace
            await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a patient with an image set
            var patientItem = await TestHelper.CreatePatientAsync(_testClassName, testNumber, Path.Combine("Becker^Matthew", "CT"));
            var entitySummaries = patientItem.FindEntities(e => e.Type == "image_set");
            var imageSetItem = await entitySummaries[0].GetAsync() as ImageSetItem;

            // Get the data for the first image
            var image = imageSetItem.Data.Images.First(i => i.Uid == "1.3.6.1.4.1.22213.2.26558.2.61");
            var headerKeyValuePairs = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Authorization", "Bearer " + imageSetItem.Data.DicomToken),
                new KeyValuePair<string, string>("Accept-Version", await _proKnow.RtvRequestor.GetApiVersion(ObjectType.ImageSet))
            };
            var bytes = await _proKnow.RtvRequestor.GetBinaryAsync($"/imageset/{imageSetItem.Data.ProcessedId}/image/{image.Tag}", headerKeyValuePairs);

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
        public async Task GetApiVersionTest()
        {
            var imageSetVersions = await _proKnow.RtvRequestor.GetApiVersion(ObjectType.ImageSet);
            var imageVersions = JsonSerializer.Deserialize<JsonElement>(imageSetVersions);
            Assert.IsTrue(imageVersions.GetProperty("ct").GetInt32() >= 0);
            Assert.IsTrue(imageVersions.GetProperty("mr").GetInt32() >= 0);
            Assert.IsTrue(imageVersions.GetProperty("pt").GetInt32() >= 0);

            var structureSetVersion = await _proKnow.RtvRequestor.GetApiVersion(ObjectType.StructureSet);
            Assert.IsTrue(int.Parse(structureSetVersion) >= 0);

            var planVersion = await _proKnow.RtvRequestor.GetApiVersion(ObjectType.Plan);
            Assert.IsTrue(int.Parse(planVersion) >= 0);

            var doseVersion = await _proKnow.RtvRequestor.GetApiVersion(ObjectType.Dose);
            Assert.IsTrue(int.Parse(doseVersion) >= 0);
        }

        public class TestStartup
        {
            public void Configure(IApplicationBuilder app)
            {
                app.Run(async (context) =>
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(context.Request.Headers["User-Agent"]);
                });
            }
        }
    }
}
