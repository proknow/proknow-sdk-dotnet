using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace ProKnow.Test
{
    [TestClass]
    public class TestSettings
    {
        public TestContext TestContext { get; set; }

        public static string BaseUrl { get; set; }

        public static string CredentialsFile { get; set; }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // Executes once before the test run
            if (!context.Properties.Contains("baseUrl") || !context.Properties.Contains("credentialsFile"))
            {
                throw new Exception("Test .runsettings are missing or misconfigured.  See project README.");
            }
            BaseUrl = context.Properties["baseUrl"].ToString();
            CredentialsFile = context.Properties["credentialsFile"].ToString();
        }
    }
}
