namespace JenkinsNetClient.Tests
{
    using System.Configuration;
    using System.Xml;
    using JenkinsNetClient;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JenkinsJobTests
    {
        private readonly IJenkinsConnection jenkinsConnection;

        public JenkinsJobTests()
        {
            if (bool.Parse(ConfigurationManager.AppSettings["UseFakeConnection"] ?? "false"))
            {
                this.jenkinsConnection = new FakeConnection();
            }
            else
            {
                this.jenkinsConnection = new JenkinsConnection(ConfigurationManager.AppSettings["JenkinsURL"]);
            }
        }

        [TestMethod]
        public void JenkinsJob_Create_Exists_Delete()
        {
            var job = new JenkinsJob(this.jenkinsConnection, "hudson.model.FreeStyleProject", "JenkinsJob_Create_Exists_Delete");

            Assert.IsFalse(job.Exists);

            Assert.IsTrue(job.Create());
            Assert.IsTrue(job.Create());
            Assert.IsFalse(job.Create(true));

            Assert.IsTrue(job.Exists);

            Assert.IsTrue(job.Delete());
            Assert.IsTrue(job.Delete());
            Assert.IsFalse(job.Delete(true));
        }

        [TestMethod]
        public void JenkinsJob_Create_SetConfig_GetConfig_Delete()
        {
            var job = new JenkinsJob(this.jenkinsConnection, "hudson.model.FreeStyleProject", "JenkinsJob_Create_Exists_Delete");

            Assert.IsTrue(job.Create());

            XmlDocument xmlDoc = new XmlDocument();
            // Load config
            xmlDoc.LoadXml(job.Config);
            // Doesn't have description element
            Assert.IsNull(xmlDoc.SelectSingleNode("./project/description"));

            // Add Description element
            XmlElement desc = xmlDoc.CreateElement("description");
            desc.InnerText = "hello";
            xmlDoc["project"].AppendChild(desc);

            // Save config
            job.Config = xmlDoc.OuterXml;

            // Load config
            xmlDoc.LoadXml(job.Config);

            // Has Description element
            Assert.IsNotNull(xmlDoc.SelectSingleNode("./project/description"));

            Assert.IsTrue(job.Delete());
        }

        [TestMethod]
        public void JenkinsJob_Duplicate_Is_Equal()
        {
            // Arrange
            JenkinsJob origin;
            JenkinsJob other;

            // Act
            origin = new JenkinsJob(this.jenkinsConnection, "hudson.model.FreeStyleProject", "SameJob");
            other = new JenkinsJob(this.jenkinsConnection, "hudson.model.FreeStyleProject", "SameJob");

            // Assert
            Assert.AreEqual(origin, other);
            Assert.IsTrue(origin.Equals(other));
            Assert.AreEqual(origin.GetHashCode(), other.GetHashCode());
        }

        [TestMethod]
        public void JenkinsJob_Null_Not_Equal()
        {
            // Arrange
            JenkinsJob origin;
            JenkinsJob other;

            // Act
            origin = new JenkinsJob(this.jenkinsConnection, "hudson.model.FreeStyleProject", "SameJob");
            other = null;

            // Assert
            Assert.AreNotEqual(origin, other);
            Assert.IsFalse(origin.Equals(other));
        }

        [TestMethod]
        public void JenkinsJob_Different_Not_Equal()
        {
            // Arrange
            JenkinsJob origin;
            JenkinsJob other;

            // Act
            origin = new JenkinsJob(this.jenkinsConnection, "hudson.model.FreeStyleProject", "SameJob");
            other = new JenkinsJob(this.jenkinsConnection, "hudson.model.FreeStyleProject", "DifferentJob");

            // Assert
            Assert.AreNotEqual(origin, other);
            Assert.IsFalse(origin.Equals(other));
            Assert.AreNotEqual(origin.GetHashCode(), other.GetHashCode());
        }
    }
}