using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;
using proknow_sdk;

namespace proknow_sdk_test
{
    [TestClass]
    public class WorkspacesTest
    {
        [TestMethod]
        public async Task QueryReturnsAllWorkspaces()
        {;
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspaces = await new Workspaces(proKnow.Requestor).Query();
            Assert.IsTrue(workspaces.Count > 0);
        }
    }
}
