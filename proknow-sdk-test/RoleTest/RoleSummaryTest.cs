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
    }
}
