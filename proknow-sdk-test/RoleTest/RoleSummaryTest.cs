using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Collections.Generic;
using System.Reflection;
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

            // Create a role
            var name = $"SDK-{_testClassName}-{testNumber}";
            var description = "Test";
            var permissions = new Permissions();
            var createdRoleItem = await _proKnow.Roles.CreateAsync(name, description, permissions);

            // Find the summary of the role just created
            var foundRoleSummary = await _proKnow.Roles.FindAsync(x => x.Id == createdRoleItem.Id);

            // Get the full representation of that role
            var gottenRoleItem = await foundRoleSummary.GetAsync();

            // Verify the returned role
            Assert.AreEqual(name, gottenRoleItem.Name);
            Assert.AreEqual(description, gottenRoleItem.Description);

            // Verify all other permissions are false
            HashSet<string> rolePermissions = new HashSet<string>
            {
                "CanReadCustomMetrics", "CanReadRenamingRules", "CanReadWorkflows",
                "CanReadChecklistTemplates", "CanReadStructureSetTemplates", "CanReadScorecardTemplates",
                "CanReadObjectiveTemplates", "CanReadWorkspaceAlgorithms", "CanReadGroups",
                "CanReadUsers", "CanReadRoles", "CanListGroupMembers", "CanResolveResourcePermissions"
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
    }
}
