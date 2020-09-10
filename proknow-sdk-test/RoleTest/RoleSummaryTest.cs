using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProKnow.Role.Test
{
    [TestClass]
    public class RoleSummaryTest
    {
        private static readonly string _testClassName = nameof(RoleSummaryTest);
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Cleanup from previous test stoppage or failure, if necessary
            await ClassCleanup();
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);

            // Delete test roles
            var roles = await _proKnow.Roles.QueryAsync();
            foreach (var role in roles)
            {
                if (role.Name.Contains(_testClassName))
                {
                    await _proKnow.Roles.DeleteAsync(role.Id);
                }
            }
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            int testNumber = 1;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a role
            var name = $"SDK-{_testClassName}-{testNumber}";
            var workspacePermissions = new WorkspacePermissions(workspaceId: workspaceItem.Id, canReadPatients: true, canReadCollections: true, canViewPhi: true);
            var workspacesPermissions = new List<WorkspacePermissions>() { workspacePermissions };
            var organizationPermissions = new OrganizationPermissions(workspaces: workspacesPermissions);
            var createdRoleItem = await _proKnow.Roles.CreateAsync(name, organizationPermissions);

            // Find the summary of the role just created
            var foundRoleSummary = await _proKnow.Roles.FindAsync(x => x.Id == createdRoleItem.Id);

            // Get the full representation of that role
            var gottenRoleItem = await foundRoleSummary.GetAsync();

            // Verify the returned role
            Assert.AreEqual(name, gottenRoleItem.Name);
            Assert.IsFalse(gottenRoleItem.CanCreateApiKeys);
            Assert.IsFalse(gottenRoleItem.CanManageAccess);
            Assert.IsFalse(gottenRoleItem.CanManageCustomMetrics);
            Assert.IsFalse(gottenRoleItem.CanManageScorecardTemplates);
            Assert.IsFalse(gottenRoleItem.CanManageRenamingRules);
            Assert.IsFalse(gottenRoleItem.CanManageChecklistTemplates);
            Assert.IsFalse(gottenRoleItem.IsCollaborator);
            Assert.IsFalse(gottenRoleItem.CanReadPatients);
            Assert.IsFalse(gottenRoleItem.CanReadCollections);
            Assert.IsFalse(gottenRoleItem.CanViewPhi);
            Assert.IsFalse(gottenRoleItem.CanDownloadDicom);
            Assert.IsFalse(gottenRoleItem.CanWriteCollections);
            Assert.IsFalse(gottenRoleItem.CanWritePatients);
            Assert.IsFalse(gottenRoleItem.CanContourPatients);
            Assert.IsFalse(gottenRoleItem.CanDeleteCollections);
            Assert.IsFalse(gottenRoleItem.CanDeletePatients);
            Assert.AreEqual(1, gottenRoleItem.Workspaces.Count);
            Assert.AreEqual(workspaceItem.Id, gottenRoleItem.Workspaces[0].WorkspaceId);
            Assert.IsFalse(gottenRoleItem.Workspaces[0].IsCollaborator);
            Assert.IsTrue(gottenRoleItem.Workspaces[0].CanReadPatients);
            Assert.IsTrue(gottenRoleItem.Workspaces[0].CanReadCollections);
            Assert.IsTrue(gottenRoleItem.Workspaces[0].CanViewPhi);
            Assert.IsFalse(gottenRoleItem.Workspaces[0].CanDownloadDicom);
            Assert.IsFalse(gottenRoleItem.Workspaces[0].CanWriteCollections);
            Assert.IsFalse(gottenRoleItem.Workspaces[0].CanWritePatients);
            Assert.IsFalse(gottenRoleItem.Workspaces[0].CanContourPatients);
            Assert.IsFalse(gottenRoleItem.Workspaces[0].CanDeleteCollections);
            Assert.IsFalse(gottenRoleItem.Workspaces[0].CanDeletePatients);
        }
    }
}
