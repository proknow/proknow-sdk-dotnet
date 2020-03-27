using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ProKnow.Test
{
    [TestClass]
    public class WorkspacesTest
    {
        //todo--CreateAsyncTest

        //todo--DeleteAsyncTest

        //todo--FindAsyncTest
        
        [TestMethod]
        public async Task QueryAsyncTest()
        {
            //todo--make this test better
            var proKnow = TestSettings.ProKnow;
            var workspaces = await proKnow.Workspaces.QueryAsync();
            Assert.IsTrue(workspaces.Count > 0);
        }

        //todo--ResolveAsyncTest

        //todo--ResolveByIdAsyncTest

        //todo--ResolveByNameAsyncTest
    }
}
