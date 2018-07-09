namespace JenkinsNetClient.Tests
{
    using System.Collections.Generic;
    using System.Configuration;
    using JenkinsNetClient;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JenkinsServerTests
    {
        private IJenkinsConnection jenkinsConnection;

        public JenkinsServerTests()
        {
            if (bool.Parse(ConfigurationManager.AppSettings["UseFakeConnection"] ?? "false"))
            {
                this.jenkinsConnection = new FakeConnection();
            }
            else
            {
                this.jenkinsConnection = new JenkinsConnection(
                    ConfigurationManager.AppSettings["JenkinsURL"],
                    ConfigurationManager.AppSettings["JenkinsUser"],
                    ConfigurationManager.AppSettings["JenkinsApiToken"]);
            }
        }

        [TestMethod]
        public void JenkinsServer_Views()
        {
            // Arrange
            var jenkins = new JenkinsServer(this.jenkinsConnection);

            // Act
            List<JenkinsView> views = jenkins.Views();

            // Assert
            Assert.AreNotEqual(0, views.Count);
        }

        [TestMethod]
        public void JenkinsServer_Jobs()
        {
            // Arrange
            var jenkins = new JenkinsServer(this.jenkinsConnection);

            // Act
            List<JenkinsJob> jobs = jenkins.Jobs();

            // Assert
            Assert.AreNotEqual(0, jobs.Count);
        }
    }
}
