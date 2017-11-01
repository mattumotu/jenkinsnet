namespace JenkinsNet
{
    using System.Collections.Generic;
    using System.Xml;

    public class JenkinsServer
    {
        private JenkinsConnection jenkinsConnection;

        public JenkinsServer(JenkinsConnection jenkinsConnection)
        {
            this.jenkinsConnection = jenkinsConnection;
        }

        public List<JenkinsView> Views()
        {
            var views = new List<JenkinsView>();

            string responseText = this.jenkinsConnection.Get("/api/xml?tree=views[name]&pretty=true");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseText);

            XmlNodeList viewNodes = xmlDoc.GetElementsByTagName("view");
            foreach (XmlNode viewNode in viewNodes)
            {
                views.Add(new JenkinsView(this.jenkinsConnection, viewNode.Attributes["_class"].InnerText, viewNode["name"].InnerText));
            }

            return views;
        }

        public List<JenkinsJob> Jobs()
        {
            return new JenkinsView(this.jenkinsConnection, "hudson.model.AllView", "all").Jobs();
        }
    }
}
