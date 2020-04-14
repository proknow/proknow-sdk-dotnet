using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Scorecard.Test
{
    [TestClass]
    public class CustomMetricsTest
    {
        private static ProKnowApi _proKnow = TestSettings.ProKnow;
        private static string _baseName = "SDK-CustomMetricsTest";
        private static CustomMetricItem _enumCustomMetricItem;
        private static CustomMetricItem _numberCustomMetricItem;
        private static CustomMetricItem _stringCustomMetricItem;

        [TestInitialize]
        public async Task ClassInitialize()
        {
            // Delete existing custom metrics, if necessary
            await TestHelper.DeleteCustomMetricsAsync(_baseName);

            // Create custom metrics for testing
            _enumCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_baseName}-enum", "patient", "enum",
                new string[] { "one", "two", "three" });
            _numberCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_baseName}-number", "dose", "number");
            _stringCustomMetricItem = await _proKnow.CustomMetrics.CreateAsync($"{_baseName}-string", "plan", "string");
        }

        [TestCleanup]
        public async Task ClassCleanup()
        {
            // Delete custom metrics created for this test
            await TestHelper.DeleteCustomMetricsAsync(_baseName);
        }

        [TestMethod]
        public void CreateAsyncTest()
        {
            // Verify creation of the enum custom metric in ClassInitialize
            Assert.AreEqual("SDK-CustomMetricsTest-enum", _enumCustomMetricItem.Name);
            Assert.AreEqual("patient", _enumCustomMetricItem.Context);
            Assert.IsNotNull(_enumCustomMetricItem.Type.Enum);
            Assert.AreEqual(3, _enumCustomMetricItem.Type.Enum.Values.Length);
            Assert.IsNull(_enumCustomMetricItem.Type.Number);
            Assert.IsNull(_enumCustomMetricItem.Type.String);

            // Verify creation of the number custom metric in ClassInitialize
            Assert.AreEqual("SDK-CustomMetricsTest-number", _numberCustomMetricItem.Name);
            Assert.AreEqual("dose", _numberCustomMetricItem.Context);
            Assert.IsNull(_numberCustomMetricItem.Type.Enum);
            Assert.IsNotNull(_numberCustomMetricItem.Type.Number);
            Assert.IsNull(_numberCustomMetricItem.Type.String);

            // Verify creation of the string custom metric in ClassInitialize
            Assert.AreEqual("SDK-CustomMetricsTest-string", _stringCustomMetricItem.Name);
            Assert.AreEqual("plan", _stringCustomMetricItem.Context);
            Assert.IsNull(_stringCustomMetricItem.Type.Enum);
            Assert.IsNull(_stringCustomMetricItem.Type.Number);
            Assert.IsNotNull(_stringCustomMetricItem.Type.String);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            await _proKnow.CustomMetrics.DeleteAsync(_enumCustomMetricItem.Id);
            await _proKnow.CustomMetrics.DeleteAsync(_numberCustomMetricItem.Id);
            await _proKnow.CustomMetrics.DeleteAsync(_stringCustomMetricItem.Id);
            var customMetrics = await _proKnow.CustomMetrics.QueryAsync();
            Assert.IsNull(customMetrics.FirstOrDefault(c => c.Name.Contains(_baseName)));
        }

        [TestMethod]
        public async Task FindAsyncTest()
        {
            var customMetric = await _proKnow.CustomMetrics.FindAsync(m => m.Name == _numberCustomMetricItem.Name);
            Assert.AreEqual(_numberCustomMetricItem.Id, customMetric.Id);
        }

        [TestMethod]
        public async Task QueryAsyncTest()
        {
            var customMetrics = await _proKnow.CustomMetrics.QueryAsync();

            // enum custom metric
            var enumCustomMetricItem = customMetrics.First(c => c.Name == $"{_baseName}-enum");
            Assert.AreEqual(_enumCustomMetricItem.Id, enumCustomMetricItem.Id);
            Assert.AreEqual(_enumCustomMetricItem.Name, enumCustomMetricItem.Name);
            Assert.AreEqual(_enumCustomMetricItem.Context, enumCustomMetricItem.Context);
            Assert.AreEqual(3, enumCustomMetricItem.Type.Enum.Values.Count());
            Assert.AreEqual(_enumCustomMetricItem.Type.Enum.Values[0], enumCustomMetricItem.Type.Enum.Values[0]);
            Assert.AreEqual(_enumCustomMetricItem.Type.Enum.Values[1], enumCustomMetricItem.Type.Enum.Values[1]);
            Assert.AreEqual(_enumCustomMetricItem.Type.Enum.Values[2], enumCustomMetricItem.Type.Enum.Values[2]);
            Assert.IsNull(enumCustomMetricItem.Type.Number);
            Assert.IsNull(enumCustomMetricItem.Type.String);

            // number custom metric
            var numberCustomMetricItem = customMetrics.First(c => c.Name == $"{_baseName}-number");
            Assert.AreEqual(_numberCustomMetricItem.Id, numberCustomMetricItem.Id);
            Assert.AreEqual(_numberCustomMetricItem.Name, numberCustomMetricItem.Name);
            Assert.AreEqual(_numberCustomMetricItem.Context, numberCustomMetricItem.Context);
            Assert.IsNull(numberCustomMetricItem.Type.Enum);
            Assert.IsNotNull(numberCustomMetricItem.Type.Number);
            Assert.IsNull(numberCustomMetricItem.Type.String);

            // string custom metric
            var stringCustomMetricItem = customMetrics.First(c => c.Name == $"{_baseName}-string");
            Assert.AreEqual(_stringCustomMetricItem.Id, stringCustomMetricItem.Id);
            Assert.AreEqual(_stringCustomMetricItem.Name, stringCustomMetricItem.Name);
            Assert.AreEqual(_stringCustomMetricItem.Context, stringCustomMetricItem.Context);
            Assert.IsNull(stringCustomMetricItem.Type.Enum);
            Assert.IsNull(stringCustomMetricItem.Type.Number);
            Assert.IsNotNull(stringCustomMetricItem.Type.String);
        }

        [TestMethod]
        public async Task ResolveAsyncTest_Id()
        {
            var customMetric = await _proKnow.CustomMetrics.ResolveAsync(_enumCustomMetricItem.Id);
            Assert.AreEqual(_enumCustomMetricItem.Id, customMetric.Id);
        }

        [TestMethod]
        public async Task ResolveAsyncTest_Name()
        {
            var customMetric = await _proKnow.CustomMetrics.ResolveAsync(_enumCustomMetricItem.Name);
            Assert.AreEqual(_enumCustomMetricItem.Id, customMetric.Id);
        }

        [TestMethod]
        public async Task ResolveByIdAsyncTest()
        {
            var customMetric = await _proKnow.CustomMetrics.ResolveByIdAsync(_numberCustomMetricItem.Id);
            Assert.AreEqual(_numberCustomMetricItem.Id, customMetric.Id);
        }

        [TestMethod]
        public async Task ResolveByNameAsyncTest()
        {
            var customMetric = await _proKnow.CustomMetrics.ResolveByNameAsync(_stringCustomMetricItem.Name);
            Assert.AreEqual(_stringCustomMetricItem.Id, customMetric.Id);
        }
    }
}
