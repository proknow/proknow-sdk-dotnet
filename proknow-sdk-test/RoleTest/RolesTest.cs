using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Role.Test
{
    [TestClass]
    public class RolesTest
    {
        private static readonly string _testClassName = nameof(RolesTest);
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
                if (role.Name.Contains(_testClassName)) {
                    await _proKnow.Roles.DeleteAsync(role.Id);
                }
            }
        }

        [TestMethod]
        public async Task CreateAsyncTest()
        {
            int testNumber = 1;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a role
            var name = $"SDK-{_testClassName}-{testNumber}";
            var workspacePermissions = new WorkspacePermissions(workspaceId: workspaceItem.Id, canReadPatients: true, canReadCollections: true, canViewPhi: true);
            var workspacesPermissions = new List<WorkspacePermissions>() { workspacePermissions };
            var organizationPermissions = new OrganizationPermissions(workspaces: workspacesPermissions);
            var roleItem = await _proKnow.Roles.CreateAsync(name, organizationPermissions);

            // Verify the created role
            Assert.AreEqual(name, roleItem.Name);
            Assert.IsFalse(roleItem.CanCreateApiKeys);
            Assert.IsFalse(roleItem.CanManageAccess);
            Assert.IsFalse(roleItem.CanManageCustomMetrics);
            Assert.IsFalse(roleItem.CanManageScorecardTemplates);
            Assert.IsFalse(roleItem.CanManageRenamingRules);
            Assert.IsFalse(roleItem.CanManageChecklistTemplates);
            Assert.IsFalse(roleItem.IsCollaborator);
            Assert.IsFalse(roleItem.CanReadPatients);
            Assert.IsFalse(roleItem.CanReadCollections);
            Assert.IsFalse(roleItem.CanViewPhi);
            Assert.IsFalse(roleItem.CanDownloadDicom);
            Assert.IsFalse(roleItem.CanWriteCollections);
            Assert.IsFalse(roleItem.CanWritePatients);
            Assert.IsFalse(roleItem.CanContourPatients);
            Assert.IsFalse(roleItem.CanDeleteCollections);
            Assert.IsFalse(roleItem.CanDeletePatients);
            Assert.AreEqual(1, roleItem.Workspaces.Count);
            Assert.AreEqual(workspaceItem.Id, roleItem.Workspaces[0].WorkspaceId);
            Assert.IsFalse(roleItem.Workspaces[0].IsCollaborator);
            Assert.IsTrue(roleItem.Workspaces[0].CanReadPatients);
            Assert.IsTrue(roleItem.Workspaces[0].CanReadCollections);
            Assert.IsTrue(roleItem.Workspaces[0].CanViewPhi);
            Assert.IsFalse(roleItem.Workspaces[0].CanDownloadDicom);
            Assert.IsFalse(roleItem.Workspaces[0].CanWriteCollections);
            Assert.IsFalse(roleItem.Workspaces[0].CanWritePatients);
            Assert.IsFalse(roleItem.Workspaces[0].CanContourPatients);
            Assert.IsFalse(roleItem.Workspaces[0].CanDeleteCollections);
            Assert.IsFalse(roleItem.Workspaces[0].CanDeletePatients);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            int testNumber = 2;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a role
            var name = $"SDK-{_testClassName}-{testNumber}";
            var workspacePermissions = new WorkspacePermissions(workspaceId: workspaceItem.Id, canReadPatients: true, canReadCollections: true, canViewPhi: true);
            var workspacesPermissions = new List<WorkspacePermissions>() { workspacePermissions };
            var organizationPermissions = new OrganizationPermissions(workspaces: workspacesPermissions);
            var roleItem = await _proKnow.Roles.CreateAsync(name, organizationPermissions);

            // Verify the role was created
            Assert.IsNotNull(_proKnow.Roles.FindAsync(x => x.Id == roleItem.Id));

            // Delete the role
            await _proKnow.Roles.DeleteAsync(roleItem.Id);

            // Verify the role was deleted
            Assert.IsNull(await _proKnow.Roles.FindAsync(x => x.Id == roleItem.Id));
        }

        [TestMethod]
        public async Task FindAsyncTest()
        {
            int testNumber = 3;

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

            // Verify the summary of the role that was found
            Assert.AreEqual(createdRoleItem.Id, foundRoleSummary.Id);
            Assert.AreEqual(createdRoleItem.Name, foundRoleSummary.Name);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            int testNumber = 4;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a role
            var name = $"SDK-{_testClassName}-{testNumber}";
            var workspacePermissions = new WorkspacePermissions(workspaceId: workspaceItem.Id, canReadPatients: true, canReadCollections: true, canViewPhi: true);
            var workspacesPermissions = new List<WorkspacePermissions>() { workspacePermissions };
            var organizationPermissions = new OrganizationPermissions(workspaces: workspacesPermissions);
            var createdRoleItem = await _proKnow.Roles.CreateAsync(name, organizationPermissions);

            // Get the role just created
            var gottenRoleItem = await _proKnow.Roles.GetAsync(createdRoleItem.Id);

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

        [TestMethod]
        public async Task QueryAsyncTest()
        {
            int testNumber = 5;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a role
            var name = $"SDK-{_testClassName}-{testNumber}";
            var workspacePermissions = new WorkspacePermissions(workspaceId: workspaceItem.Id, canReadPatients: true, canReadCollections: true, canViewPhi: true);
            var workspacesPermissions = new List<WorkspacePermissions>() { workspacePermissions };
            var organizationPermissions = new OrganizationPermissions(workspaces: workspacesPermissions);
            var createdRoleItem = await _proKnow.Roles.CreateAsync(name, organizationPermissions);

            // Query for roles
            var roleSummaries = await _proKnow.Roles.QueryAsync();

            // Verify the returned roles contained the role just created
            Assert.IsTrue(roleSummaries.Any(x => x.Id == createdRoleItem.Id && x.Name == createdRoleItem.Name));
        }
    }
}
