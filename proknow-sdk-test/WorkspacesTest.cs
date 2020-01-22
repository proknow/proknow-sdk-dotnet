using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;
using ProKnow;

namespace ProKnow.Test
{
    [TestClass]
    public class WorkspacesTest
    {
        [TestMethod]
        public async Task QueryTest()
        {
            var proKnow = new ProKnow(TestSettings.BaseUrl, TestSettings.CredentialsFile);
            var workspaces = await new Workspaces(proKnow.Requestor).Query();
            Assert.IsTrue(workspaces.Count > 0);
        }
    }
}
