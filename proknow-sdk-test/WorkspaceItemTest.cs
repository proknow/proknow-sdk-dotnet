using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ProKnow.Test
{
    [TestClass]
    public class WorkspaceItemTest
    {
        private static string _testClassName = nameof(WorkspaceItemTest);
        private ProKnowApi _proKnow = TestSettings.ProKnow;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // Delete test workspaces, if necessary
            await TestHelper.DeleteWorkspacesAsync(_testClassName);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            int testNumber = 1;

            // Create the workspace
            var workspaceName = $"SDK-{_testClassName}-{testNumber}";
            var workspaceItem = await _proKnow.Workspaces.CreateAsync(workspaceName.ToLower(), workspaceName, false);

            // Verify that the workspace does exist
            Assert.IsNotNull(await _proKnow.Workspaces.FindAsync(w => w.Name == workspaceName));

            // Delete the workspace
            await workspaceItem.DeleteAsync();

            // Verify that the workspace does not exist
            Assert.IsNull(await _proKnow.Workspaces.FindAsync(w => w.Name == workspaceName));
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            int testNumber = 2;

            // Create a workspace
            var workspaceName1 = $"SDK-{_testClassName}-{testNumber}-A";
            var workspaceItem = await _proKnow.Workspaces.CreateAsync(workspaceName1.ToLower(), workspaceName1, false);

            // Change the workspace slug, name, and protected flag
            var workspaceName2 = $"SDK-{_testClassName}-{testNumber}-B";
            workspaceItem.Slug = workspaceName2.ToLower();
            workspaceItem.Name = workspaceName2;
            workspaceItem.Protected = true;

            // Save the workspace changes
            await workspaceItem.SaveAsync();

            // Verify the workspace changes were saved
            var workspaceItem2 = await _proKnow.Workspaces.FindAsync(w => w.Id == workspaceItem.Id);
            Assert.AreEqual(workspaceItem.Slug, workspaceItem2.Slug);
            Assert.AreEqual(workspaceItem.Name, workspaceItem2.Name);
            Assert.AreEqual(workspaceItem.Protected, workspaceItem2.Protected);
        }
    }
}
