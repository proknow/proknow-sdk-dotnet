using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace ProKnow.Test
{
    [TestClass]
    public class WorkspacesTest
    {
        [TestMethod]
        public async Task QueryTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspaces = await proKnow.Workspaces.QueryAsync();
            Assert.IsTrue(workspaces.Count > 0);
        }
    }
}
