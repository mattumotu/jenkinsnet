namespace JenkinsNetClient
{
    using System.Collections.Generic;
    using System.Xml;

    public class JenkinsServer
    {
        private readonly IJenkinsConnection jenkinsConnection;

        public JenkinsServer(IJenkinsConnection jenkinsConnection)
        {
            this.jenkinsConnection = jenkinsConnection;
        }

        public List<JenkinsView> Views()
        {
            var views = new List<JenkinsView>();

            string responseText = this.jenkinsConnection.Get("/api/xml?tree=views[name]");
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
