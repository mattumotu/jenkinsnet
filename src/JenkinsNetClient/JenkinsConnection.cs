namespace JenkinsNetClient
{
    using System.IO;
    using System.Net;
    using System.Xml;
    using JenkinsNetClient.Request;

    /// <summary>
    /// A connection to a jenkins server
    /// </summary>
    public class JenkinsConnection : JenkinsNetClient.IJenkinsConnection
    {
        /// <summary>
        /// Holds the jenkins server url
        /// </summary>
        private readonly string url;

        /// <summary>
        /// Holds the jenkins username
        /// </summary>
        private readonly string username;

        /// <summary>
        /// Holds the jenkins API token
        /// </summary>
        private readonly string apiToken;

        /// <summary>
        /// Holds the jenkins crumb
        /// </summary>
        private readonly string crumb;

        /// <summary>
        /// Determines if crumb should be automatically retrieved
        /// </summary>
        private readonly bool autoRetrieveCrumb;

        /// <summary>
        /// Initializes a new instance of the <see cref="JenkinsConnection" /> class.
        /// </summary>
        /// <param name="url">the target jenkins server url</param>
        public JenkinsConnection(string url, bool autoRetrieveCrumb = false)
            : this(url, null, null, autoRetrieveCrumb)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JenkinsConnection" /> class.
        /// </summary>
        /// <param name="url">the target jenkins server url</param>
        /// <param name="username">the jenkins username</param>
        /// <param name="apiToken">the jenkins API token</param>
        public JenkinsConnection(string url, string username, string apiToken)
        {
            this.url = url;
            this.username = username;
            this.apiToken = apiToken;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JenkinsConnection" /> class.
        /// </summary>
        /// <param name="url">the target jenkins server url</param>
        /// <param name="username">the jenkins username</param>
        /// <param name="apiToken">the jenkins API token</param>
        /// <param name="crumb">the jenkins crumb</param>
        public JenkinsConnection(string url, string username, string apiToken, string crumb)
        {
            this.url = url;
            this.username = username;
            this.apiToken = apiToken;
            this.crumb = crumb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JenkinsConnection" /> class.
        /// </summary>
        /// <param name="url">the target jenkins server url</param>
        /// <param name="username">the jenkins username</param>
        /// <param name="apiToken">the jenkins API token</param>
        /// <param name="autoRetrieveCrumb">determines if the crumb will be automatically retrieved</param>
        public JenkinsConnection(string url, string username, string apiToken, bool autoRetrieveCrumb)
        {
            this.url = url;
            this.username = username;
            this.apiToken = apiToken;
            this.autoRetrieveCrumb = autoRetrieveCrumb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JenkinsConnection" /> class.
        /// </summary>
        /// <param name="url">the target jenkins server url</param>
        /// <param name="crumb">the jenkins crumb</param>
        public JenkinsConnection(string url, string crumb)
        {
            this.url = url;
            this.crumb = crumb;
        }

        /// <summary>
        /// Get a crumb from jenkins.
        /// </summary>
        /// <returns>the crumb</returns>
        private string RetrieveCrumb()
        {
            var crumbXml = this.CallRequest(
                    new GetRequest(
                        new XmlRequest(
                            new HttpRequest(this.url, "/crumbIssuer/api/xml"))));
            XmlDocument crumbDoc = new XmlDocument();
            crumbDoc.LoadXml(crumbXml);
            XmlNode requestedCrumb = crumbDoc.SelectSingleNode("defaultCrumbIssuer/crumb");
            return requestedCrumb.InnerText;
        }

        /// <summary>
        /// Make a GET request to jenkins.
        /// </summary>
        /// <remarks>Content Type is <c>text/json</c></remarks>
        /// <param name="command">the uri to GET (appended to server address)</param>
        /// <returns>the response from the jenkins server</returns>
        public string Get(string command)
        {
            string crumb = this.crumb;
            if (this.autoRetrieveCrumb)
            {
                crumb = RetrieveCrumb();
            }
            return this.CallRequest(
                new AuthorisedRequest(
                    new GetRequest(
                        new JsonRequest(
                            new HttpRequest(this.url, command))),
                    this.username,
                    this.apiToken,
                    crumb));
        }

        /// <summary>
        /// Make a POST request to jenkins
        /// </summary>
        /// <param name="command">the uri to POST (appended to server address)</param>
        /// <param name="contentType">the content type</param>
        /// <param name="postData">the data to be posted</param>
        /// <returns>the response from the jenkins server</returns>
        public string Post(string command, string contentType, string postData)
        {
            string crumb = this.crumb;
            if (this.autoRetrieveCrumb)
            {
                crumb = RetrieveCrumb();
            }
            return this.CallRequest(
                new ContentTypeRequest(
                    new AuthorisedRequest(
                        new PostRequest(
                            new HttpRequest(this.url, command),
                            postData),
                        this.username,
                        this.apiToken,
                        crumb),
                    contentType));
        }

        /// <summary>
        /// Make a POST request to jenkins, return True/False depending on success
        /// </summary>
        /// <param name="command">the uri to POST (appended to server address)</param>
        /// <param name="contentType">the content type</param>
        /// <param name="postData">the data to be posted</param>
        /// <returns>a flag indicating whether the request was successfully POSTed</returns>
        public bool TryPost(string command, string contentType, string postData)
        {
            try
            {
                this.Post(
                    command,
                    contentType,
                    postData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string CallRequest(IRequest request)
        {
            var response = request.Build().GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}