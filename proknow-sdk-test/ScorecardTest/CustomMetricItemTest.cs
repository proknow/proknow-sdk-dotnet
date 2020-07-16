using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Threading.Tasks;

namespace ProKnow.Scorecard.Test
{
    [TestClass]
    public class CustomMetricItemTest
    {
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static readonly string _baseName = "SDK-CustomMetricItemTest";

        [TestInitialize]
        public async Task ClassInitialize()
        {
            // Delete existing custom metrics, if necessary
            await TestHelper.DeleteCustomMetricsAsync(_baseName);
        }

        [TestCleanup]
        public async Task ClassCleanup()
        {
            // Delete custom metrics created for this test
            await TestHelper.DeleteCustomMetricsAsync(_baseName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            // Create a custom metric
            var customMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_baseName}-1", "dose", "number");

            // Verify it was created
            Assert.IsNotNull(await _proKnow.CustomMetrics.FindAsync(t => t.Name == $"{_baseName}-1"));

            // Delete it
            await customMetricItem.DeleteAsync();

            // Verify it was deleted
            Assert.IsNull(await _proKnow.CustomMetrics.FindAsync(t => t.Name == $"{_baseName}-1"));
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            // Create a custom metric
            var customMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_baseName}-2", "plan", "number");

            // Change the name and context and save the changes
            customMetricItem.Name = $"{_baseName}-2a";
            customMetricItem.Context = "structure_set";
            await customMetricItem.SaveAsync();

            // Verify the changes were saved
            Assert.IsNull(await _proKnow.CustomMetrics.FindAsync(t => t.Name == $"{_baseName}-2"));
            var savedCustomMetricItem = await _proKnow.CustomMetrics.FindAsync(t => t.Name == $"{_baseName}-2a");
            Assert.AreEqual("structure_set", savedCustomMetricItem.Context);
        }
    }
}
