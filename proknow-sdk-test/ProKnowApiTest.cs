using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Exceptions;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Test
{
    [TestClass]
    public class ProKnowApiTest
    {
        [TestMethod]
        public void Constructor_MissingCredentialsFile()
        {
            var credentialsFile = Path.Combine(TestSettings.TestDataRootDirectory, "./nothing_here");
            try
            {
                new ProKnowApi(TestSettings.BaseUrl, credentialsFile);
                Assert.Fail();
            }
            catch (ProKnowException ex)
            {
                Assert.AreEqual($"The credentials file '{credentialsFile}' was not found.", ex.Message);
            }
        }

        [TestMethod]
        public void Constructor_CredentialsFileNotJson()
        {
            var credentialsFile = Path.Combine(TestSettings.TestDataRootDirectory, "./dummy.pdf");
            try
            {
                 new ProKnowApi(TestSettings.BaseUrl, credentialsFile);
                Assert.Fail();
            }
            catch (ProKnowException ex)
            {
                Assert.AreEqual($"The credentials file '{credentialsFile}' is not valid JSON.", ex.Message);
            }
        }

        [TestMethod]
        public void Constructor_CredentialsFileNotCredentialsJson()
        {
            var credentialsFile = Path.Combine(TestSettings.TestDataRootDirectory, "./not_credentials.json");
            try
            {
                new ProKnowApi(TestSettings.BaseUrl, credentialsFile);
                Assert.Fail();
            }
            catch (ProKnowException ex)
            {
                Assert.AreEqual($"The 'id' and/or 'secret' in the credentials file '{credentialsFile}' are missing.", ex.Message);
            }
        }

        [TestMethod]
        public async Task GetStatusAsync_GoodCredentials()
        {
            var proKnowApi = new ProKnowApi(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var status = await proKnowApi.GetStatusAsync();
            Assert.AreEqual("OK", status);
        }

        [TestMethod]
        public async Task GetStatusAsync_BadBaseUrl()
        {
            var proKnowApi = new ProKnowApi("https://example.proknow.com", TestSettings.CredentialsFile);
            var status = await proKnowApi.GetStatusAsync();
            Assert.AreEqual("HttpError(Unauthorized, Invalid or missing credentials)", status);
        }

        [TestMethod]
        public async Task GetStatusAsync_BogusCredentials()
        {
            var credentialsFile = Path.Combine(TestSettings.TestDataRootDirectory, "./bogus_credentials.json");
            var proKnowApi = new ProKnowApi(TestSettings.BaseUrl, credentialsFile);
            var status = await proKnowApi.GetStatusAsync();
            Assert.AreEqual("HttpError(Unauthorized, Invalid or missing credentials)", status);
        }
    }
}
