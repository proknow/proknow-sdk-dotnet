using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

using ProKnow.Test;

namespace ProKnow.CustomMetric.Test
{
    [TestClass]
    public class CustomMetricsTest
    {
        [TestMethod]
        public async Task QueryTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var customMetrics = await proKnow.CustomMetrics.QueryAsync();
            Assert.IsTrue(customMetrics.Count > 0);
        }
    }
}
