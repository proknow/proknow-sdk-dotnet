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
        public async Task GetCredentialsStatusAsync_GoodCredentials()
        {
            var proKnowApi = new ProKnowApi(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var credentialsStatus = await proKnowApi.GetCredentialsStatusAsync();
            Assert.IsTrue(credentialsStatus.IsValid);
        }

        [TestMethod]
        public async Task GetCredentialsStatusAsync_BogusCredentials()
        {
            var credentialsFile = Path.Combine(TestSettings.TestDataRootDirectory, "bogus_credentials.json");
            var proKnowApi = new ProKnowApi(TestSettings.BaseUrl, credentialsFile);
            var credentialsStatus = await proKnowApi.GetCredentialsStatusAsync();
            Assert.IsFalse(credentialsStatus.IsValid);
            Assert.AreEqual("Invalid or missing credentials", credentialsStatus.ErrorMessage);
        }

        [TestMethod]
        public async Task GetDomainStatusAsync_GoodDomain()
        {
            var credentialsFile = Path.Combine(TestSettings.TestDataRootDirectory, "bogus_credentials.json");
            var proKnowApi = new ProKnowApi("https://thisisaninvalidsubdomain.proknow.com", credentialsFile);
            var domainStatus = await proKnowApi.GetDomainStatusAsync();
            Assert.IsTrue(domainStatus.IsOk);
        }

        [TestMethod]
        public async Task GetDomainStatusAsync_BadDomain()
        {
            var credentialsFile = Path.Combine(TestSettings.TestDataRootDirectory, "bogus_credentials.json");
            var proKnowApi = new ProKnowApi("https://justan.example.com", credentialsFile);
            var domainStatus = await proKnowApi.GetDomainStatusAsync();
            Assert.IsFalse(domainStatus.IsOk);
            Assert.AreEqual("Exception occurred making HTTP request. No such host is known.", domainStatus.ErrorMessage);
        }
    }
}
