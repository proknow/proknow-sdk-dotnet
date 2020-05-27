using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using ProKnow.Exceptions;

namespace ProKnow.Test
{
    [TestClass]
    public class WorkspacesTest
    {
        private static string _testClassName = nameof(WorkspacesTest);
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
        public async Task CreateAsyncTest()
        {
            int testNumber = 1;

            // Verify that the workspace does not exist
            var workspaceName = $"SDK-{_testClassName}-{testNumber}";
            Assert.IsNull(await _proKnow.Workspaces.FindAsync(w => w.Name == workspaceName));

            // Create the workspace
            await _proKnow.Workspaces.CreateAsync(workspaceName.ToLower(), workspaceName, false);

            // Verify that the workspace does exist
            Assert.IsNotNull(await _proKnow.Workspaces.FindAsync(w => w.Name == workspaceName));
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            int testNumber = 2;

            // Create the workspace
            var workspaceName = $"SDK-{_testClassName}-{testNumber}";
            var workspaceItem = await _proKnow.Workspaces.CreateAsync(workspaceName.ToLower(), workspaceName, false);

            // Verify that the workspace does exist
            Assert.IsNotNull(await _proKnow.Workspaces.FindAsync(w => w.Name == workspaceName));

            // Delete the workspace
            await _proKnow.Workspaces.DeleteAsync(workspaceItem.Id);

            // Verify that the workspace does not exist
            Assert.IsNull(await _proKnow.Workspaces.FindAsync(w => w.Name == workspaceName));
        }

        [TestMethod]
        public async Task FindAsyncTest()
        {
            int testNumber = 3;

            // Create two workspaces
            var workspaceName1 = $"SDK-{_testClassName}-{testNumber}-A";
            await _proKnow.Workspaces.CreateAsync(workspaceName1.ToLower(), workspaceName1, false);
            var workspaceName2 = $"SDK-{_testClassName}-{testNumber}-B";
            var workspaceItem2 = await _proKnow.Workspaces.CreateAsync(workspaceName2.ToLower(), workspaceName2, false);

            // Find the second workspace
            var workspaceItem = await _proKnow.Workspaces.FindAsync(w => w.Name == workspaceName2);

            // Verify that the correct workspace was found
            Assert.AreEqual(workspaceItem2.Id, workspaceItem.Id);
            Assert.AreEqual(workspaceItem2.Slug, workspaceItem.Slug);
            Assert.AreEqual(workspaceItem2.Name, workspaceItem.Name);
            Assert.AreEqual(workspaceItem2.Protected, workspaceItem.Protected);
        }

        [TestMethod]
        public async Task QueryAsyncTest()
        {
            int testNumber = 4;

            // Create two workspaces
            var workspaceName1 = $"SDK-{_testClassName}-{testNumber}-A";
            var workspaceItem1 = await _proKnow.Workspaces.CreateAsync(workspaceName1.ToLower(), workspaceName1);
            var workspaceName2 = $"SDK-{_testClassName}-{testNumber}-B";
            var workspaceItem2 = await _proKnow.Workspaces.CreateAsync(workspaceName2.ToLower(), workspaceName2, false);

            // Query the workspaces
            var workspaces = await _proKnow.Workspaces.QueryAsync();

            // Verify that both workspaces were found
            Assert.IsTrue(workspaces.Any(w => w.Id == workspaceItem1.Id && w.Slug == workspaceItem1.Slug &&
                w.Name == workspaceItem1.Name && w.Protected == workspaceItem1.Protected));
            Assert.IsTrue(workspaces.Any(w => w.Id == workspaceItem2.Id && w.Slug == workspaceItem2.Slug &&
                w.Name == workspaceItem2.Name && w.Protected == workspaceItem2.Protected));
        }

        [TestMethod]
        public async Task ResolveAsyncTest_Id()
        {
            int testNumber = 5;

            // Create two workspaces
            var workspaceName1 = $"SDK-{_testClassName}-{testNumber}-A";
            var workspaceItem1 = await _proKnow.Workspaces.CreateAsync(workspaceName1.ToLower(), workspaceName1);
            var workspaceName2 = $"SDK-{_testClassName}-{testNumber}-B";
            var workspaceItem2 = await _proKnow.Workspaces.CreateAsync(workspaceName2.ToLower(), workspaceName2, false);

            // Resolve the first workspace by ID
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspaceItem1.Id);

            // Verify that the first workspace was returned
            Assert.AreEqual(workspaceItem1.Id, workspaceItem.Id);
        }

        [TestMethod]
        public async Task ResolveAsyncTest_Name()
        {
            int testNumber = 6;

            // Create two workspaces
            var workspaceName1 = $"SDK-{_testClassName}-{testNumber}-A";
            var workspaceItem1 = await _proKnow.Workspaces.CreateAsync(workspaceName1.ToLower(), workspaceName1);
            var workspaceName2 = $"SDK-{_testClassName}-{testNumber}-B";
            var workspaceItem2 = await _proKnow.Workspaces.CreateAsync(workspaceName2.ToLower(), workspaceName2, false);

            // Resolve the second workspace by name
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspaceItem2.Name);

            // Verify that the second workspace was returned
            Assert.AreEqual(workspaceItem2.Id, workspaceItem.Id);
        }

        [TestMethod]
        public async Task ResolveByIdAsyncTest()
        {
            int testNumber = 7;

            // Create two workspaces
            var workspaceName1 = $"SDK-{_testClassName}-{testNumber}-A";
            var workspaceItem1 = await _proKnow.Workspaces.CreateAsync(workspaceName1.ToLower(), workspaceName1);
            var workspaceName2 = $"SDK-{_testClassName}-{testNumber}-B";
            var workspaceItem2 = await _proKnow.Workspaces.CreateAsync(workspaceName2.ToLower(), workspaceName2, false);

            // Resolve the first workspace by ID
            var workspaceItem = await _proKnow.Workspaces.ResolveByIdAsync(workspaceItem1.Id);

            // Verify that the first workspace was returned
            Assert.AreEqual(workspaceItem1.Id, workspaceItem.Id);
        }

        [TestMethod]
        public async Task ResolveByIdAsyncTest_InvalidId()
        {
            int testNumber = 8;

            // Create two workspaces
            var workspaceName1 = $"SDK-{_testClassName}-{testNumber}-A";
            var workspaceItem1 = await _proKnow.Workspaces.CreateAsync(workspaceName1.ToLower(), workspaceName1);
            var workspaceName2 = $"SDK-{_testClassName}-{testNumber}-B";
            var workspaceItem2 = await _proKnow.Workspaces.CreateAsync(workspaceName2.ToLower(), workspaceName2, false);

            // Try to resolve a workspace with an invalid ID
            var invalidId = "12345678901234567890123456789012";
            try
            {
                await _proKnow.Workspaces.ResolveByIdAsync(invalidId);
                Assert.Fail();
            }
            catch (ProKnowWorkspaceLookupException ex)
            {
                Assert.AreEqual(ex.Message, $"There is no workspace with a ProKnow ID of '{invalidId}'.");
            }
        }

        [TestMethod]
        public async Task ResolveByNameAsyncTest()
        {
            int testNumber = 9;

            // Create two workspaces
            var workspaceName1 = $"SDK-{_testClassName}-{testNumber}-A";
            var workspaceItem1 = await _proKnow.Workspaces.CreateAsync(workspaceName1.ToLower(), workspaceName1);
            var workspaceName2 = $"SDK-{_testClassName}-{testNumber}-B";
            var workspaceItem2 = await _proKnow.Workspaces.CreateAsync(workspaceName2.ToLower(), workspaceName2, false);

            // Resolve the second workspace by name
            var workspaceItem = await _proKnow.Workspaces.ResolveByNameAsync(workspaceItem2.Name);

            // Verify that the second workspace was returned
            Assert.AreEqual(workspaceItem2.Id, workspaceItem.Id);
        }

        [TestMethod]
        public async Task ResolveByNameAsyncTest_InvalidName()
        {
            int testNumber = 10;

            // Create two workspaces
            var workspaceName1 = $"SDK-{_testClassName}-{testNumber}-A";
            var workspaceItem1 = await _proKnow.Workspaces.CreateAsync(workspaceName1.ToLower(), workspaceName1);
            var workspaceName2 = $"SDK-{_testClassName}-{testNumber}-B";
            var workspaceItem2 = await _proKnow.Workspaces.CreateAsync(workspaceName2.ToLower(), workspaceName2, false);

            // Try to resolve a workspace with an invalid name
            try
            {
                await _proKnow.Workspaces.ResolveByNameAsync("Invalid Name");
                Assert.Fail();
            }
            catch (ProKnowWorkspaceLookupException ex)
            {
                Assert.AreEqual(ex.Message, $"There is no workspace with a Name of 'Invalid Name'.");
            }
        }
    }
}
