using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProKnow.Role.Test
{
    [TestClass]
    public class RoleItemTest
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
        public async Task DeleteAsyncTest()
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

            // Verify the role was created
            Assert.IsNotNull(_proKnow.Roles.FindAsync(x => x.Id == roleItem.Id));

            // Delete the role
            await roleItem.DeleteAsync();

            // Verify the role was deleted
            Assert.IsNull(await _proKnow.Roles.FindAsync(x => x.Id == roleItem.Id));
        }
        [TestMethod]
        public async Task SaveAsyncTest()
        {
            int testNumber = 2;

            // Create a workspace
            var workspaceItem = await TestHelper.CreateWorkspaceAsync(_testClassName, testNumber);

            // Create a role with no permissions
            var name = $"SDK-{_testClassName}-{testNumber}";
            var roleItem = await _proKnow.Roles.CreateAsync(name, new OrganizationPermissions());

            // Verify that the created role has no permissions
            Assert.AreEqual(name, roleItem.Name);
            Assert.IsFalse(roleItem.Permissions.CanCreateApiKeys);
            Assert.IsFalse(roleItem.Permissions.CanManageAccess);
            Assert.IsFalse(roleItem.Permissions.CanManageCustomMetrics);
            Assert.IsFalse(roleItem.Permissions.CanManageScorecardTemplates);
            Assert.IsFalse(roleItem.Permissions.CanManageRenamingRules);
            Assert.IsFalse(roleItem.Permissions.CanManageChecklistTemplates);
            Assert.IsFalse(roleItem.Permissions.IsCollaborator);
            Assert.IsFalse(roleItem.Permissions.CanReadPatients);
            Assert.IsFalse(roleItem.Permissions.CanReadCollections);
            Assert.IsFalse(roleItem.Permissions.CanViewPhi);
            Assert.IsFalse(roleItem.Permissions.CanDownloadDicom);
            Assert.IsFalse(roleItem.Permissions.CanWriteCollections);
            Assert.IsFalse(roleItem.Permissions.CanWritePatients);
            Assert.IsFalse(roleItem.Permissions.CanContourPatients);
            Assert.IsFalse(roleItem.Permissions.CanDeleteCollections);
            Assert.IsFalse(roleItem.Permissions.CanDeletePatients);
            Assert.AreEqual(0, roleItem.Permissions.Workspaces.Count);

            // Change the name and add all permissions
            roleItem.Name = $"{name}-Copy";
            roleItem.Permissions.CanCreateApiKeys = true;
            roleItem.Permissions.CanManageAccess = true;
            roleItem.Permissions.CanManageCustomMetrics = true;
            roleItem.Permissions.CanManageScorecardTemplates = true;
            roleItem.Permissions.CanManageRenamingRules = true;
            roleItem.Permissions.CanManageChecklistTemplates = true;
            roleItem.Permissions.IsCollaborator = false;
            roleItem.Permissions.CanReadPatients = true;
            roleItem.Permissions.CanReadCollections = true;
            roleItem.Permissions.CanViewPhi = true;
            roleItem.Permissions.CanDownloadDicom = true;
            roleItem.Permissions.CanWriteCollections = true;
            roleItem.Permissions.CanWritePatients = true;
            roleItem.Permissions.CanContourPatients = true;
            roleItem.Permissions.CanDeleteCollections = true;
            roleItem.Permissions.CanDeletePatients = true;
            roleItem.Permissions.Workspaces.Add(new WorkspacePermissions());
            roleItem.Permissions.Workspaces[0].WorkspaceId = workspaceItem.Id;
            roleItem.Permissions.Workspaces[0].IsCollaborator = false;
            roleItem.Permissions.Workspaces[0].CanReadPatients = true;
            roleItem.Permissions.Workspaces[0].CanReadCollections = true;
            roleItem.Permissions.Workspaces[0].CanViewPhi = true;
            roleItem.Permissions.Workspaces[0].CanDownloadDicom = true;
            roleItem.Permissions.Workspaces[0].CanWriteCollections = true;
            roleItem.Permissions.Workspaces[0].CanWritePatients = true;
            roleItem.Permissions.Workspaces[0].CanContourPatients = true;
            roleItem.Permissions.Workspaces[0].CanDeleteCollections = true;
            roleItem.Permissions.Workspaces[0].CanDeletePatients = true;

            // Save the changes
            await roleItem.SaveAsync();

            // Get the role again
            roleItem = await _proKnow.Roles.GetAsync(roleItem.Id);

            // Verify the changes were saved
            Assert.AreEqual($"{name}-Copy", roleItem.Name);
            Assert.IsTrue(roleItem.Permissions.CanCreateApiKeys);
            Assert.IsTrue(roleItem.Permissions.CanManageAccess);
            Assert.IsTrue(roleItem.Permissions.CanManageCustomMetrics);
            Assert.IsTrue(roleItem.Permissions.CanManageScorecardTemplates);
            Assert.IsTrue(roleItem.Permissions.CanManageRenamingRules);
            Assert.IsTrue(roleItem.Permissions.CanManageChecklistTemplates);
            Assert.IsFalse(roleItem.Permissions.IsCollaborator);
            Assert.IsTrue(roleItem.Permissions.CanReadPatients);
            Assert.IsTrue(roleItem.Permissions.CanReadCollections);
            Assert.IsTrue(roleItem.Permissions.CanViewPhi);
            Assert.IsTrue(roleItem.Permissions.CanDownloadDicom);
            Assert.IsTrue(roleItem.Permissions.CanWriteCollections);
            Assert.IsTrue(roleItem.Permissions.CanWritePatients);
            Assert.IsTrue(roleItem.Permissions.CanContourPatients);
            Assert.IsTrue(roleItem.Permissions.CanDeleteCollections);
            Assert.IsTrue(roleItem.Permissions.CanDeletePatients);
            Assert.AreEqual(1, roleItem.Permissions.Workspaces.Count);
            Assert.AreEqual(workspaceItem.Id, roleItem.Permissions.Workspaces[0].WorkspaceId);
            Assert.IsFalse(roleItem.Permissions.Workspaces[0].IsCollaborator);
            Assert.IsTrue(roleItem.Permissions.Workspaces[0].CanReadPatients);
            Assert.IsTrue(roleItem.Permissions.Workspaces[0].CanReadCollections);
            Assert.IsTrue(roleItem.Permissions.Workspaces[0].CanViewPhi);
            Assert.IsTrue(roleItem.Permissions.Workspaces[0].CanDownloadDicom);
            Assert.IsTrue(roleItem.Permissions.Workspaces[0].CanWriteCollections);
            Assert.IsTrue(roleItem.Permissions.Workspaces[0].CanWritePatients);
            Assert.IsTrue(roleItem.Permissions.Workspaces[0].CanContourPatients);
            Assert.IsTrue(roleItem.Permissions.Workspaces[0].CanDeleteCollections);
            Assert.IsTrue(roleItem.Permissions.Workspaces[0].CanDeletePatients);
        }
    }
}
