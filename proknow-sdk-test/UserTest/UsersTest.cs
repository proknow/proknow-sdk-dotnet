using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Role;
using ProKnow.Test;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.User.Test
{
    [TestClass]
    public class UsersTest
    {
        private static readonly string _testClassName = nameof(UsersTest);
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
        public async Task CreateAsyncTest()
        {
            int testNumber = 1;

            // Create a user
            var email = $"User{testNumber}@SDK-{_testClassName}.com";
            var name = $"SDK-{_testClassName}-{testNumber}";
            var userItem = await _proKnow.Users.CreateAsync(email, name, _publicRole.Id);

            // Verify the created user
            Assert.AreEqual(email, userItem.Email);
            Assert.AreEqual(name, userItem.Name);
            Assert.IsTrue(userItem.IsActive);
            Assert.AreEqual(_publicRole.Id, userItem.RoleId);
            Assert.IsNull(userItem.Role);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            int testNumber = 2;

            // Create a user
            var email = $"User{testNumber}@SDK-{_testClassName}.com";
            var name = $"SDK-{_testClassName}-{testNumber}";
            var userItem = await _proKnow.Users.CreateAsync(email, name, _publicRole.Id);

            // Verify the user was created
            Assert.IsNotNull(_proKnow.Users.FindAsync(x => x.Id == userItem.Id));

            // Delete the user
            await _proKnow.Users.DeleteAsync(userItem.Id);

            // Verify the user was deleted
            Assert.IsNull(await _proKnow.Users.FindAsync(x => x.Id == userItem.Id));
        }

        [TestMethod]
        public async Task FindAsyncTest()
        {
            int testNumber = 3;

            // Create a user
            var email = $"User{testNumber}@SDK-{_testClassName}.com";
            var name = $"SDK-{_testClassName}-{testNumber}";
            var createdUserItem = await _proKnow.Users.CreateAsync(email, name, _publicRole.Id);

            // Find the summary of the user just created
            var foundUserSummary = await _proKnow.Users.FindAsync(x => x.Id == createdUserItem.Id);

            // Verify the summary of the user that was found
            Assert.AreEqual(createdUserItem.Id, foundUserSummary.Id);
            Assert.AreEqual(createdUserItem.Name, foundUserSummary.Name);
        }

        [TestMethod]
        public async Task GetAsyncTest_PublicRole()
        {
            int testNumber = 4;

            // Create a user
            var email = $"User{testNumber}@SDK-{_testClassName}.com";
            var name = $"SDK-{_testClassName}-{testNumber}";
            var createdUserItem = await _proKnow.Users.CreateAsync(email, name, _publicRole.Id);

            // Get the user just created
            var gottenUserItem = await _proKnow.Users.GetAsync(createdUserItem.Id);

            // Verify the returned user
            Assert.AreEqual(name, gottenUserItem.Name);
            Assert.AreEqual(email, gottenUserItem.Email);
            Assert.IsTrue(gottenUserItem.IsActive);
            Assert.AreEqual(_publicRole.Id, gottenUserItem.RoleId);
            Assert.IsNull(gottenUserItem.Role);
        }

        [TestMethod]
        public async Task GetAsyncTest_PrivateRole()
        {
            int testNumber = 5;

            // Create a user
            var email = $"User{testNumber}@SDK-{_testClassName}.com";
            var name = $"SDK-{_testClassName}-{testNumber}";
            var createdUserItem = await _proKnow.Users.CreateAsync(email, name, _publicRole.Id);

            // Give the user a private role and save the changes
            createdUserItem.RoleId = null;
            var permissions = new OrganizationPermissions(canReadPatients: true, canReadCollections: true, canViewPhi: true);
            createdUserItem.Role = new RoleItem(permissions);
            await createdUserItem.SaveAsync();

            // Retrieve this user
            var gottenUserItem = await _proKnow.Users.GetAsync(createdUserItem.Id);

            // Verify the returned user
            Assert.AreEqual(name, gottenUserItem.Name);
            Assert.AreEqual(email, gottenUserItem.Email);
            Assert.IsTrue(gottenUserItem.IsActive);
            Assert.IsNull(gottenUserItem.RoleId);
            Assert.IsTrue(gottenUserItem.Role.IsPrivate);
            Assert.IsFalse(gottenUserItem.Role.Permissions.CanCreateApiKeys);
            Assert.IsFalse(gottenUserItem.Role.Permissions.CanManageAccess);
            Assert.IsFalse(gottenUserItem.Role.Permissions.CanManageCustomMetrics);
            Assert.IsFalse(gottenUserItem.Role.Permissions.CanManageScorecardTemplates);
            Assert.IsFalse(gottenUserItem.Role.Permissions.CanManageRenamingRules);
            Assert.IsFalse(gottenUserItem.Role.Permissions.CanManageChecklistTemplates);
            Assert.IsFalse(gottenUserItem.Role.Permissions.IsCollaborator);
            Assert.IsTrue(gottenUserItem.Role.Permissions.CanReadPatients);
            Assert.IsTrue(gottenUserItem.Role.Permissions.CanReadCollections);
            Assert.IsTrue(gottenUserItem.Role.Permissions.CanViewPhi);
            Assert.IsFalse(gottenUserItem.Role.Permissions.CanDownloadDicom);
            Assert.IsFalse(gottenUserItem.Role.Permissions.CanWriteCollections);
            Assert.IsFalse(gottenUserItem.Role.Permissions.CanWritePatients);
            Assert.IsFalse(gottenUserItem.Role.Permissions.CanContourPatients);
            Assert.IsFalse(gottenUserItem.Role.Permissions.CanDeleteCollections);
            Assert.IsFalse(gottenUserItem.Role.Permissions.CanDeletePatients);
            Assert.AreEqual(0, gottenUserItem.Role.Permissions.Workspaces.Count);
        }

        [TestMethod]
        public async Task QueryAsyncTest()
        {
            int testNumber = 6;

            // Create a user
            var email = $"User{testNumber}@SDK-{_testClassName}.com";
            var name = $"SDK-{_testClassName}-{testNumber}";
            var createdUserItem = await _proKnow.Users.CreateAsync(email, name, _publicRole.Id);

            // Query for users
            var userSummaries = await _proKnow.Users.QueryAsync();

            // Verify the returned users contained the user just created
            Assert.IsTrue(userSummaries.Any(x => x.Id == createdUserItem.Id && x.Email == createdUserItem.Email && x.Name == createdUserItem.Name));
        }
    }
}
