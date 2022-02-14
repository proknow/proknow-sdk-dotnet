using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            // Delete test users
            var users = await _proKnow.Users.QueryAsync();
            foreach (var user in users)
            {
                if (user.Name.Contains(_testClassName))
                {
                    await _proKnow.Users.DeleteAsync(user.Id);
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
            var userItem = await _proKnow.Users.CreateAsync(email, name);

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

            // Create a user
            var email0 = $"User{testNumber}@SDK-{_testClassName}.com";
            var name0 = $"SDK-{_testClassName}-{testNumber}";
            var userItem = await _proKnow.Users.CreateAsync(email0, name0);

            // Verify the current properties
            Assert.AreEqual(name0, userItem.Name);
            Assert.AreEqual(email0, userItem.Email);
            Assert.IsTrue(userItem.IsActive);

            // Modify the email and name of the user, make them inactive
            var email1 = $"User{testNumber}@SDK-{_testClassName}-1.com";
            var name1 = $"SDK-{_testClassName}-{testNumber}-1";
            userItem.Email = email1;
            userItem.Name = name1;
            userItem.IsActive = false;
            await userItem.SaveAsync();

            // Retrieve the modified user
            userItem = await _proKnow.Users.GetAsync(userItem.Id);

            // Verify that the modifications were saved
            Assert.AreEqual(name1, userItem.Name);
            Assert.AreEqual(email1, userItem.Email);
            Assert.IsFalse(userItem.IsActive);
        }
    }
}
