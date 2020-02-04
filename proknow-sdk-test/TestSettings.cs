using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;

namespace ProKnow.Test
{
    /// <summary>
    /// Provides the user test settings from the project .runsettings (see README.md)
    /// </summary>
    [TestClass]
    public class TestSettings
    {
        /// <summary>
        /// The base URL to ProKnow, e.g. 'https://example.proknow.com'
        /// </summary>
        public static string BaseUrl { get; set; }

        /// <summary>
        /// The path to the ProKnow credentials JSON file
        /// </summary>
        public static string CredentialsFile { get; set; }

        /// <summary>
        /// The workspace name for testing
        /// </summary>
        public static string TestWorkspaceName { get; set; }

        /// <summary>
        /// The patient name for testing
        /// </summary>
        public static string TestPatientName { get; set; }

        /// <summary>
        /// The full path to the root directory for test data
        /// </summary>
        public static string TestDataRootDirectory { get; set; }

        /// <summary>
        /// Initializes the test run
        /// </summary>
        /// <param name="context">Information that is passed to the unit tests</param>
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            if (!context.Properties.Contains("baseUrl") || !context.Properties.Contains("credentialsFile"))
            {
                throw new Exception("Test .runsettings are missing or misconfigured.  See project README.");
            }
            BaseUrl = context.Properties["baseUrl"].ToString();
            CredentialsFile = context.Properties["credentialsFile"].ToString();
            TestWorkspaceName = context.Properties["testWorkspaceName"].ToString();
            TestPatientName = context.Properties["testPatientName"].ToString();
            TestDataRootDirectory = Path.Combine(context.DeploymentDirectory, "TestData");
        }
    }
}
