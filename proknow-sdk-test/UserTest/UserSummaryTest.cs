using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Test;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.User.Test
{
    [TestClass]
    public class UserSummaryTest
    {
        private static readonly string _testClassName = nameof(UserSummaryTest);
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
        public async Task GetAsyncTest()
        {
            int testNumber = 1;

            // Create a user
            var email = $"User{testNumber}@SDK-{_testClassName}.com";
            var name = $"SDK-{_testClassName}-{testNumber}";
            var createdUserItem = await _proKnow.Users.CreateAsync(email, name);

            // Get the user just created
            var foundUserSummary = await _proKnow.Users.FindAsync(x => x.Id == createdUserItem.Id);
            var gottenUserItem = await foundUserSummary.GetAsync();

            // Verify the returned user
            Assert.AreEqual(name, gottenUserItem.Name);
            Assert.AreEqual(email, gottenUserItem.Email);
            Assert.IsTrue(gottenUserItem.IsActive);
        }
    }
}
