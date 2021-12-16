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
        public void Constructor_MissingCredentialsFolder()
        {
            var credentialsFile = Path.Combine(TestSettings.TestDataRootDirectory, "not a subfolder", "./credentials.json");
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
        public async Task GetConnectionStatusAsync_GoodCredentials()
        {
            var proKnowApi = new ProKnowApi(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var connectionStatus = await proKnowApi.GetConnectionStatusAsync();
            Assert.IsTrue(connectionStatus.IsValid);
        }

        [TestMethod]
        public async Task GetConnectionStatusAsync_BadBaseUrl()
        {
            var proKnowApi = new ProKnowApi("https://example.proknow.com", TestSettings.CredentialsFile);
            var connectionStatus = await proKnowApi.GetConnectionStatusAsync();
            Assert.IsFalse(connectionStatus.IsValid);
            Assert.AreEqual("Invalid or missing credentials", connectionStatus.ErrorMessage);
        }

        [TestMethod]
        public async Task GetConnectionStatusAsync_BogusCredentials()
        {
            var credentialsFile = Path.Combine(TestSettings.TestDataRootDirectory, "./bogus_credentials.json");
            var proKnowApi = new ProKnowApi(TestSettings.BaseUrl, credentialsFile);
            var connectionStatus = await proKnowApi.GetConnectionStatusAsync();
            Assert.IsTrue(connectionStatus.IsValid);
        }
    }
}
