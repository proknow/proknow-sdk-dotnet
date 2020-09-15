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
            Assert.AreEqual(1, roleItem.Permissions.Workspaces.Count);
            Assert.AreEqual(workspaceItem.Id, roleItem.Permissions.Workspaces[0].WorkspaceId);
            Assert.IsFalse(roleItem.Permissions.Workspaces[0].IsCollaborator);
            Assert.IsTrue(roleItem.Permissions.Workspaces[0].CanReadPatients);
            Assert.IsTrue(roleItem.Permissions.Workspaces[0].CanReadCollections);
            Assert.IsTrue(roleItem.Permissions.Workspaces[0].CanViewPhi);
            Assert.IsFalse(roleItem.Permissions.Workspaces[0].CanDownloadDicom);
            Assert.IsFalse(roleItem.Permissions.Workspaces[0].CanWriteCollections);
            Assert.IsFalse(roleItem.Permissions.Workspaces[0].CanWritePatients);
            Assert.IsFalse(roleItem.Permissions.Workspaces[0].CanContourPatients);
            Assert.IsFalse(roleItem.Permissions.Workspaces[0].CanDeleteCollections);
            Assert.IsFalse(roleItem.Permissions.Workspaces[0].CanDeletePatients);

            // Verify that the ExtensionData does not contain the permissions
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("create_api_keys"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("manage_access"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("manage_custom_metrics"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("manage_template_metric_sets"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("manage_renaming_rules"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("manage_template_checklists"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("organization_collaborator"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("organization_read_patients"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("organization_read_collections"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("organization_view_phi"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("organization_download_dicom"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("organization_write_collections"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("organization_write_patients"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("organization_contour_patients"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("organization_delete_collections"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("organization_delete_patients"));
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("workspaces"));
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
            Assert.IsFalse(gottenRoleItem.Permissions.CanCreateApiKeys);
            Assert.IsFalse(gottenRoleItem.Permissions.CanManageAccess);
            Assert.IsFalse(gottenRoleItem.Permissions.CanManageCustomMetrics);
            Assert.IsFalse(gottenRoleItem.Permissions.CanManageScorecardTemplates);
            Assert.IsFalse(gottenRoleItem.Permissions.CanManageRenamingRules);
            Assert.IsFalse(gottenRoleItem.Permissions.CanManageChecklistTemplates);
            Assert.IsFalse(gottenRoleItem.Permissions.IsCollaborator);
            Assert.IsFalse(gottenRoleItem.Permissions.CanReadPatients);
            Assert.IsFalse(gottenRoleItem.Permissions.CanReadCollections);
            Assert.IsFalse(gottenRoleItem.Permissions.CanViewPhi);
            Assert.IsFalse(gottenRoleItem.Permissions.CanDownloadDicom);
            Assert.IsFalse(gottenRoleItem.Permissions.CanWriteCollections);
            Assert.IsFalse(gottenRoleItem.Permissions.CanWritePatients);
            Assert.IsFalse(gottenRoleItem.Permissions.CanContourPatients);
            Assert.IsFalse(gottenRoleItem.Permissions.CanDeleteCollections);
            Assert.IsFalse(gottenRoleItem.Permissions.CanDeletePatients);
            Assert.AreEqual(1, gottenRoleItem.Permissions.Workspaces.Count);
            Assert.AreEqual(workspaceItem.Id, gottenRoleItem.Permissions.Workspaces[0].WorkspaceId);
            Assert.IsFalse(gottenRoleItem.Permissions.Workspaces[0].IsCollaborator);
            Assert.IsTrue(gottenRoleItem.Permissions.Workspaces[0].CanReadPatients);
            Assert.IsTrue(gottenRoleItem.Permissions.Workspaces[0].CanReadCollections);
            Assert.IsTrue(gottenRoleItem.Permissions.Workspaces[0].CanViewPhi);
            Assert.IsFalse(gottenRoleItem.Permissions.Workspaces[0].CanDownloadDicom);
            Assert.IsFalse(gottenRoleItem.Permissions.Workspaces[0].CanWriteCollections);
            Assert.IsFalse(gottenRoleItem.Permissions.Workspaces[0].CanWritePatients);
            Assert.IsFalse(gottenRoleItem.Permissions.Workspaces[0].CanContourPatients);
            Assert.IsFalse(gottenRoleItem.Permissions.Workspaces[0].CanDeleteCollections);
            Assert.IsFalse(gottenRoleItem.Permissions.Workspaces[0].CanDeletePatients);

            // Verify that the ExtensionData does not contain the permissions
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("create_api_keys"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("manage_access"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("manage_custom_metrics"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("manage_template_metric_sets"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("manage_renaming_rules"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("manage_template_checklists"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("organization_collaborator"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("organization_read_patients"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("organization_read_collections"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("organization_view_phi"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("organization_download_dicom"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("organization_write_collections"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("organization_write_patients"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("organization_contour_patients"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("organization_delete_collections"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("organization_delete_patients"));
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("workspaces"));
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
