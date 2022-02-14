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

            // Create a role
            var name = $"SDK-{_testClassName}-{testNumber}";
            var permissions = new Permissions(canReadPatients: true, canReadCollections: true);
            var roleItem = await _proKnow.Roles.CreateAsync(name, "", permissions);

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

            // Create a role with no permissions
            var name = $"SDK-{_testClassName}-{testNumber}";
            var roleItem = await _proKnow.Roles.CreateAsync(name, "", new Permissions(canReadPatients: true));

            // Verify the created role name and permissions
            Assert.AreEqual(name, roleItem.Name);
            Assert.IsFalse(roleItem.Permissions.CanCreateApiKeys);
            Assert.IsTrue(roleItem.Permissions.CanReadPatients);

            // Change the name and permissions
            roleItem.Name = $"{name}-Copy";
            roleItem.Permissions.CanCreateApiKeys = true;
            roleItem.Permissions.CanReadPatients = false;

            // Save the changes
            await roleItem.SaveAsync();

            // Get the role again
            roleItem = await _proKnow.Roles.GetAsync(roleItem.Id);

            // Verify the changes were saved
            Assert.AreEqual($"{name}-Copy", roleItem.Name);
            Assert.IsTrue(roleItem.Permissions.CanCreateApiKeys);
            Assert.IsFalse(roleItem.Permissions.CanReadPatients);
        }
    }
}
