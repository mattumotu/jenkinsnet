namespace JenkinsNetClient
{
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// Defines the <see cref="JenkinsServer" />
    /// </summary>
    public class JenkinsServer
    {
        /// <summary>
        /// Holds the jenkinsConnection
        /// </summary>
        private readonly IJenkinsConnection jenkinsConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="JenkinsServer"/> class.
        /// </summary>
        /// <param name="jenkinsConnection">The jenkinsConnection<see cref="IJenkinsConnection"/></param>
        public JenkinsServer(IJenkinsConnection jenkinsConnection)
        {
            this.jenkinsConnection = jenkinsConnection;
        }

        /// <summary>
        /// The Views that exist on this server
        /// </summary>
        /// <returns>The <see cref="List{JenkinsView}"/></returns>
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

        /// <summary>
        /// The Jobs that exist on this server
        /// </summary>
        /// <returns>The <see cref="List{JenkinsJob}"/></returns>
        public List<JenkinsJob> Jobs()
        {
            return new JenkinsView(this.jenkinsConnection, "hudson.model.AllView", "all").Jobs();
        }
    }
}