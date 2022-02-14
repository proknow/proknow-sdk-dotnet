using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            // Create a role
            var name = $"SDK-{_testClassName}-{testNumber}";
            var Permissions = new Permissions(canReadPatients: true, canReadCollections: true);
            var roleItem = await _proKnow.Roles.CreateAsync(name, "", Permissions);

            // Verify the created role
            Assert.AreEqual(name, roleItem.Name);
            Assert.AreEqual("", roleItem.Description);

            // Verify all other permissions are false
            HashSet<string> rolePermissions = new HashSet<string>
            {
                "CanReadCustomMetrics", "CanReadRenamingRules", "CanReadWorkflows",
                "CanReadChecklistTemplates", "CanReadStructureSetTemplates", "CanReadScorecardTemplates",
                "CanReadObjectiveTemplates", "CanReadWorkspaceAlgorithms", "CanReadGroups",
                "CanReadUsers", "CanReadRoles", "CanListGroupMembers", "CanResolveResourcePermissions",
                "CanReadPatients", "CanReadCollections"
            };
            foreach (PropertyInfo prop in roleItem.Permissions.GetType().GetProperties())
            {
                if (!rolePermissions.Contains(prop.Name) && prop.Name != "ExtensionData")
                {
                    Assert.IsFalse((bool)prop.GetValue(roleItem.Permissions, null));
                }
            }

            // Verify that the ExtensionData does not contain the permissions
            Assert.IsFalse(roleItem.ExtensionData.ContainsKey("permissions"));
            Assert.IsTrue(roleItem.ExtensionData.ContainsKey("system"));
            Assert.IsTrue(roleItem.ExtensionData.ContainsKey("created_at"));
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            int testNumber = 2;

            // Create a role
            var name = $"SDK-{_testClassName}-{testNumber}";
            var permissions = new Permissions(canReadPatients: true, canReadCollections: true);
            var roleItem = await _proKnow.Roles.CreateAsync(name, "Test", permissions);

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

            // Create a role
            var name = $"SDK-{_testClassName}-{testNumber}";
            var permissions = new Permissions();
            var createdRoleItem = await _proKnow.Roles.CreateAsync(name, "Test", permissions);

            // Find the summary of the role just created
            var foundRoleSummary = await _proKnow.Roles.FindAsync(x => x.Id == createdRoleItem.Id);

            // Verify the summary of the role that was found
            Assert.AreEqual(createdRoleItem.Id, foundRoleSummary.Id);
            Assert.AreEqual(createdRoleItem.Name, foundRoleSummary.Name);
            Assert.AreEqual(createdRoleItem.Description, foundRoleSummary.Description);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            int testNumber = 4;

            // Create a role
            var name = $"SDK-{_testClassName}-{testNumber}";
            var permissions = new Permissions(canReadWorkspaces: true, canReadPatients: true, canCreatePatients: true);
            var createdRoleItem = await _proKnow.Roles.CreateAsync(name, "", permissions);

            // Get the role just created
            var gottenRoleItem = await _proKnow.Roles.GetAsync(createdRoleItem.Id);

            // Verify the returned role
            Assert.AreEqual(name, gottenRoleItem.Name);
            Assert.AreEqual("", gottenRoleItem.Description);

            // Verify all other permissions are false
            HashSet<string> rolePermissions = new HashSet<string>
            {
                "CanReadCustomMetrics", "CanReadRenamingRules", "CanReadWorkflows",
                "CanReadChecklistTemplates", "CanReadStructureSetTemplates", "CanReadScorecardTemplates",
                "CanReadObjectiveTemplates", "CanReadWorkspaceAlgorithms", "CanReadGroups",
                "CanReadUsers", "CanReadRoles", "CanListGroupMembers", "CanResolveResourcePermissions",
                "CanReadWorkspaces", "CanReadPatients", "CanCreatePatients"
            };
            foreach (PropertyInfo prop in gottenRoleItem.Permissions.GetType().GetProperties())
            {
                if (!rolePermissions.Contains(prop.Name) && prop.Name != "ExtensionData")
                {
                    Assert.IsFalse((bool)prop.GetValue(gottenRoleItem.Permissions, null));
                }
            }

            // Verify that the ExtensionData does not contain the permissions
            Assert.IsFalse(gottenRoleItem.ExtensionData.ContainsKey("permissions"));
            Assert.IsTrue(gottenRoleItem.ExtensionData.ContainsKey("system"));
            Assert.IsTrue(gottenRoleItem.ExtensionData.ContainsKey("created_at"));
        }

        [TestMethod]
        public async Task QueryAsyncTest()
        {
            int testNumber = 5;

            // Create a role
            var name = $"SDK-{_testClassName}-{testNumber}";
            var permissions = new Permissions(canReadPatients: true, canReadCollections: true);
            var createdRoleItem = await _proKnow.Roles.CreateAsync(name, "", permissions);

            // Query for roles
            var roleSummaries = await _proKnow.Roles.QueryAsync();

            // Verify the returned roles contained the role just created
            Assert.IsTrue(roleSummaries.Any(x => x.Id == createdRoleItem.Id && x.Name == createdRoleItem.Name));
        }
    }
}
