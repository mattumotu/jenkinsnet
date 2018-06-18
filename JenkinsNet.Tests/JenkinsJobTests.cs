namespace JenkinsNet.Tests
{
    using System.Configuration;
    using System.Xml;
    using JenkinsNet;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JenkinsJobTests
    {
        private IJenkinsConnection jenkinsConnection;

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

            Assert.IsFalse(job.Exists());

            Assert.IsTrue(job.Create());
            Assert.IsTrue(job.Create());
            Assert.IsFalse(job.Create(true));

            Assert.IsTrue(job.Exists());

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
    }
}
