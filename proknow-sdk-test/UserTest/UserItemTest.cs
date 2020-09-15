using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Role;
using ProKnow.Test;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.User.Test
{
    [TestClass]
    public class UserItemTest
    {
        private static readonly string _testClassName = nameof(UserItemTest);
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;
        private static RoleItem _publicRole;

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Cleanup from previous test stoppage or failure, if necessary
            await ClassCleanup();

            // Create a public role
            _publicRole = await _proKnow.Roles.CreateAsync($"SDK-{_testClassName}-Public",
                new OrganizationPermissions(canReadPatients: true, canReadCollections: true, canViewPhi: true));
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // Delete test workspaces
            await TestHelper.DeleteWorkspacesAsync(_testClassName);

            // Delete test users
            var users = await _proKnow.Users.QueryAsync();
            foreach (var user in users)
            {
                if (user.Name.Contains(_testClassName))
                {
                    await _proKnow.Users.DeleteAsync(user.Id);
                }
            }

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

            // Create a user
            var email = $"User{testNumber}@SDK-{_testClassName}.com";
            var name = $"SDK-{_testClassName}-{testNumber}";
            var userItem = await _proKnow.Users.CreateAsync(email, name, _publicRole.Id);

            // Verify the user was created
            Assert.IsNotNull(_proKnow.Users.FindAsync(x => x.Id == userItem.Id));

            // Delete the user
            await userItem.DeleteAsync();

            // Verify the user was deleted
            Assert.IsNull(await _proKnow.Users.FindAsync(x => x.Id == userItem.Id));
        }

        [TestMethod]
        public async Task SaveAsyncTest()
        {
            int testNumber = 2;

            // Create a user with a public role
            var email0 = $"User{testNumber}@SDK-{_testClassName}.com";
            var name0 = $"SDK-{_testClassName}-{testNumber}";
            var userItem = await _proKnow.Users.CreateAsync(email0, name0, _publicRole.Id);

            // Verify the current properties
            Assert.AreEqual(name0, userItem.Name);
            Assert.AreEqual(email0, userItem.Email);
            Assert.IsTrue(userItem.IsActive);
            Assert.AreEqual(_publicRole.Id, userItem.RoleId);
            Assert.IsNull(userItem.Role);

            // Modify the email and name of the user, make them inactive, and give them a private role
            var email1 = $"User{testNumber}@SDK-{_testClassName}-1.com";
            var name1 = $"SDK-{_testClassName}-{testNumber}-1";
            userItem.Email = email1;
            userItem.Name = name1;
            userItem.IsActive = false;
            userItem.RoleId = null;
            userItem.Role = new RoleItem();
            userItem.Role.Permissions.CanCreateApiKeys = true;
            userItem.Role.Permissions.CanManageAccess = true;
            userItem.Role.Permissions.CanManageCustomMetrics = true;
            userItem.Role.Permissions.CanManageScorecardTemplates = true;
            userItem.Role.Permissions.CanManageRenamingRules = true;
            userItem.Role.Permissions.CanManageChecklistTemplates = true;
            userItem.Role.Permissions.IsCollaborator = false;
            userItem.Role.Permissions.CanReadPatients = true;
            userItem.Role.Permissions.CanReadCollections = true;
            userItem.Role.Permissions.CanViewPhi = true;
            userItem.Role.Permissions.CanDownloadDicom = true;
            userItem.Role.Permissions.CanWriteCollections = true;
            userItem.Role.Permissions.CanWritePatients = true;
            userItem.Role.Permissions.CanContourPatients = true;
            userItem.Role.Permissions.CanDeleteCollections = true;
            userItem.Role.Permissions.CanDeletePatients = true;
            await userItem.SaveAsync();

            // Retrieve the modified user
            userItem = await _proKnow.Users.GetAsync(userItem.Id);

            // Verify that the modifications were saved
            Assert.AreEqual(name1, userItem.Name);
            Assert.AreEqual(email1, userItem.Email);
            Assert.IsFalse(userItem.IsActive);
            Assert.IsNull(userItem.RoleId);
            Assert.IsTrue(userItem.Role.IsPrivate);
            Assert.IsTrue(userItem.Role.Permissions.CanCreateApiKeys);
            Assert.IsTrue(userItem.Role.Permissions.CanManageAccess);
            Assert.IsTrue(userItem.Role.Permissions.CanManageCustomMetrics);
            Assert.IsTrue(userItem.Role.Permissions.CanManageScorecardTemplates);
            Assert.IsTrue(userItem.Role.Permissions.CanManageRenamingRules);
            Assert.IsTrue(userItem.Role.Permissions.CanManageChecklistTemplates);
            Assert.IsFalse(userItem.Role.Permissions.IsCollaborator);
            Assert.IsTrue(userItem.Role.Permissions.CanReadPatients);
            Assert.IsTrue(userItem.Role.Permissions.CanReadCollections);
            Assert.IsTrue(userItem.Role.Permissions.CanViewPhi);
            Assert.IsTrue(userItem.Role.Permissions.CanDownloadDicom);
            Assert.IsTrue(userItem.Role.Permissions.CanWriteCollections);
            Assert.IsTrue(userItem.Role.Permissions.CanWritePatients);
            Assert.IsTrue(userItem.Role.Permissions.CanContourPatients);
            Assert.IsTrue(userItem.Role.Permissions.CanDeleteCollections);
            Assert.IsTrue(userItem.Role.Permissions.CanDeletePatients);
            Assert.AreEqual(0, userItem.Role.Permissions.Workspaces.Count);
        }
    }
}
