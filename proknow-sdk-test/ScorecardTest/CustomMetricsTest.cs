using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Scorecard.Test
{
    [TestClass]
    public class CustomMetricsTest
    {
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static readonly string _testClassName = nameof(CustomMetricsTest);

        [TestInitialize]
        public async Task ClassInitialize()
        {
            // Cleanup from previous test stoppage or failure, if necessary
            await ClassCleanup();
        }

        [TestCleanup]
        public async Task ClassCleanup()
        {
            // Delete custom metrics
            await TestHelper.DeleteCustomMetricsAsync(_testClassName);
        }

        [TestMethod]
        public async Task CreateAsyncTest()
        {
            var testNumber = 1;

            // Create custom metrics
            var enumCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-enum", "patient", "enum",
                new string[] { "one", "two", "three" });
            var numberCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-number", "dose", "number");
            var stringCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-string", "plan", "string");

            // Verify creation of the enum custom metric
            Assert.AreEqual($"{_testClassName}-{testNumber}-enum", enumCustomMetricItem.Name);
            Assert.AreEqual("patient", enumCustomMetricItem.Context);
            Assert.IsNotNull(enumCustomMetricItem.Type.Enum);
            Assert.AreEqual(3, enumCustomMetricItem.Type.Enum.Values.Length);
            Assert.IsNull(enumCustomMetricItem.Type.Number);
            Assert.IsNull(enumCustomMetricItem.Type.String);

            // Verify creation of the number custom metric
            Assert.AreEqual($"{_testClassName}-{testNumber}-number", numberCustomMetricItem.Name);
            Assert.AreEqual("dose", numberCustomMetricItem.Context);
            Assert.IsNull(numberCustomMetricItem.Type.Enum);
            Assert.IsNotNull(numberCustomMetricItem.Type.Number);
            Assert.IsNull(numberCustomMetricItem.Type.String);

            // Verify creation of the string custom metric
            Assert.AreEqual($"{_testClassName}-{testNumber}-string", stringCustomMetricItem.Name);
            Assert.AreEqual("plan", stringCustomMetricItem.Context);
            Assert.IsNull(stringCustomMetricItem.Type.Enum);
            Assert.IsNull(stringCustomMetricItem.Type.Number);
            Assert.IsNotNull(stringCustomMetricItem.Type.String);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            var testNumber = 2;

            // Create a custom metric
            var numberCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-number", "dose", "number");

            // Delete it
            await _proKnow.CustomMetrics.DeleteAsync(numberCustomMetricItem.Id);

            // Verify that the custom metric was deleted
            var customMetrics = await _proKnow.CustomMetrics.QueryAsync();
            Assert.IsNull(customMetrics.FirstOrDefault(c => c.Id == numberCustomMetricItem.Id));
        }

        [TestMethod]
        public async Task FindAsyncTest()
        {
            var testNumber = 3;

            // Create custom metrics
            await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-enum", "patient", "enum",
                new string[] { "one", "two", "three" });
            var numberCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-number", "dose", "number");
            await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-string", "plan", "string");

            // Find a custom metric
            var customMetric = await _proKnow.CustomMetrics.FindAsync(m => m.Name == numberCustomMetricItem.Name);

            // Verify that the custom metric was found
            Assert.AreEqual(numberCustomMetricItem.Id, customMetric.Id);
        }

        [TestMethod]
        public async Task QueryAsyncTest()
        {
            var testNumber = 4;

            // Create custom metrics
            var expectedEnumCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-enum", "patient", "enum",
                new string[] { "one", "two", "three" });
            var expectedNumberCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-number", "dose", "number");
            var expectedStringCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-string", "plan", "string");

            // Query for custom metrics
            var customMetrics = await _proKnow.CustomMetrics.QueryAsync();

            // Verify that the enum custom metric was returned
            var actualEnumCustomMetricItem = customMetrics.First(c => c.Name == $"{_testClassName}-{testNumber}-enum");
            Assert.AreEqual(expectedEnumCustomMetricItem.Id, actualEnumCustomMetricItem.Id);
            Assert.AreEqual(expectedEnumCustomMetricItem.Name, actualEnumCustomMetricItem.Name);
            Assert.AreEqual(expectedEnumCustomMetricItem.Context, actualEnumCustomMetricItem.Context);
            Assert.AreEqual(3, actualEnumCustomMetricItem.Type.Enum.Values.Count());
            Assert.AreEqual(expectedEnumCustomMetricItem.Type.Enum.Values[0], actualEnumCustomMetricItem.Type.Enum.Values[0]);
            Assert.AreEqual(expectedEnumCustomMetricItem.Type.Enum.Values[1], actualEnumCustomMetricItem.Type.Enum.Values[1]);
            Assert.AreEqual(expectedEnumCustomMetricItem.Type.Enum.Values[2], actualEnumCustomMetricItem.Type.Enum.Values[2]);
            Assert.IsNull(actualEnumCustomMetricItem.Type.Number);
            Assert.IsNull(actualEnumCustomMetricItem.Type.String);

            // Verify that the number custom metric was returned
            var actualNumberCustomMetricItem = customMetrics.First(c => c.Name == $"{_testClassName}-{testNumber}-number");
            Assert.AreEqual(expectedNumberCustomMetricItem.Id, actualNumberCustomMetricItem.Id);
            Assert.AreEqual(expectedNumberCustomMetricItem.Name, actualNumberCustomMetricItem.Name);
            Assert.AreEqual(expectedNumberCustomMetricItem.Context, actualNumberCustomMetricItem.Context);
            Assert.IsNull(actualNumberCustomMetricItem.Type.Enum);
            Assert.IsNotNull(actualNumberCustomMetricItem.Type.Number);
            Assert.IsNull(actualNumberCustomMetricItem.Type.String);

            // Verify that the string custom metric was returned
            var actualStringCustomMetricItem = customMetrics.First(c => c.Name == $"{_testClassName}-{testNumber}-string");
            Assert.AreEqual(expectedStringCustomMetricItem.Id, actualStringCustomMetricItem.Id);
            Assert.AreEqual(expectedStringCustomMetricItem.Name, actualStringCustomMetricItem.Name);
            Assert.AreEqual(expectedStringCustomMetricItem.Context, actualStringCustomMetricItem.Context);
            Assert.IsNull(actualStringCustomMetricItem.Type.Enum);
            Assert.IsNull(actualStringCustomMetricItem.Type.Number);
            Assert.IsNotNull(actualStringCustomMetricItem.Type.String);
        }

        [TestMethod]
        public async Task ResolveAsyncTest_Id()
        {
            var testNumber = 5;

            // Create a custom metric
            var stringCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-string", "plan", "string");

            // Resolve the custom metric by providing ID
            var customMetric = await _proKnow.CustomMetrics.ResolveAsync(stringCustomMetricItem.Id);

            // Verify the returned custom metric
            Assert.AreEqual(stringCustomMetricItem.Name, customMetric.Name);
            Assert.AreEqual(stringCustomMetricItem.Context, customMetric.Context);
            Assert.IsNull(customMetric.Type.Enum);
            Assert.IsNull(customMetric.Type.Number);
            Assert.IsNotNull(customMetric.Type.String);
        }

        [TestMethod]
        public async Task ResolveAsyncTest_Name()
        {
            var testNumber = 6;

            // Create a custom metric
            var stringCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-string", "plan", "string");

            // Resolve the custom metric by providing name
            var customMetric = await _proKnow.CustomMetrics.ResolveAsync(stringCustomMetricItem.Name);

            // Verify the returned custom metric
            Assert.AreEqual(stringCustomMetricItem.Id, customMetric.Id);
            Assert.AreEqual(stringCustomMetricItem.Context, customMetric.Context);
            Assert.IsNull(customMetric.Type.Enum);
            Assert.IsNull(customMetric.Type.Number);
            Assert.IsNotNull(customMetric.Type.String);
        }

        [TestMethod]
        public async Task ResolveByIdAsyncTest()
        {
            var testNumber = 7;

            // Create a custom metric
            var stringCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-string", "plan", "string");

            // Resolve the custom metric by ID
            var customMetric = await _proKnow.CustomMetrics.ResolveByIdAsync(stringCustomMetricItem.Id);

            // Verify the returned custom metric
            Assert.AreEqual(stringCustomMetricItem.Name, customMetric.Name);
            Assert.AreEqual(stringCustomMetricItem.Context, customMetric.Context);
            Assert.IsNull(customMetric.Type.Enum);
            Assert.IsNull(customMetric.Type.Number);
            Assert.IsNotNull(customMetric.Type.String);
        }

        [TestMethod]
        public async Task ResolveByNameAsyncTest()
        {
            var testNumber = 8;

            // Create a custom metric
            var stringCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_testClassName}-{testNumber}-string", "plan", "string");

            // Resolve the custom metric by name
            var customMetric = await _proKnow.CustomMetrics.ResolveByNameAsync(stringCustomMetricItem.Name);

            // Verify the returned custom metric
            Assert.AreEqual(stringCustomMetricItem.Id, customMetric.Id);
            Assert.AreEqual(stringCustomMetricItem.Context, customMetric.Context);
            Assert.IsNull(customMetric.Type.Enum);
            Assert.IsNull(customMetric.Type.Number);
            Assert.IsNotNull(customMetric.Type.String);
        }
    }
}
