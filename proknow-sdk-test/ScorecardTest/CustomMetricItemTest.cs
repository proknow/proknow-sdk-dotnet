using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Threading.Tasks;

namespace ProKnow.Scorecard.Test
{
    [TestClass]
    public class CustomMetricItemTest
    {
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static readonly string _testClassName = nameof(CustomMetricItemTest);

        [TestInitialize]
        public async Task ClassInitialize()
        {
            // Cleanup from previous test stoppage or failure, if necessary
            await ClassCleanup();
        }

        [TestCleanup]
        public async Task ClassCleanup()
        {
            // Delete custom metrics created for this test
            await TestHelper.DeleteCustomMetricsAsync(_testClassName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            var testNumber = 1;

            // Create a custom metric
            var customMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}", "dose", "number");

            // Verify it was created
            Assert.IsNotNull(await _proKnow.CustomMetrics.FindAsync(t => t.Name == $"{_testClassName}-{testNumber}"));

            // Delete it
            await customMetricItem.DeleteAsync();

            // Verify it was deleted
            Assert.IsNull(await _proKnow.CustomMetrics.FindAsync(t => t.Name == $"{_testClassName}-{testNumber}"));
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            var testNumber = 2;

            // Create a custom metric
            var customMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}", "plan", "number");

            // Change the name and context and save the changes
            customMetricItem.Name = $"{_testClassName}-{testNumber}a";
            customMetricItem.Context = "structure_set";
            await customMetricItem.SaveAsync();

            // Verify the changes were saved
            Assert.IsNull(await _proKnow.CustomMetrics.FindAsync(t => t.Name == $"{_testClassName}-{testNumber}"));
            var savedCustomMetricItem = await _proKnow.CustomMetrics.FindAsync(t => t.Name == $"{_testClassName}-{testNumber}a");
            Assert.AreEqual("structure_set", savedCustomMetricItem.Context);
        }
    }
}
