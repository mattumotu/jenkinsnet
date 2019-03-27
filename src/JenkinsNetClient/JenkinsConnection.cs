namespace JenkinsNetClient
{
    using System.IO;
    using System.Net;
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
        /// Initializes a new instance of the <see cref="JenkinsConnection" /> class.
        /// </summary>
        /// <param name="url">the target jenkins server url</param>
        public JenkinsConnection(string url)
            : this(url, null, null)
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
        /// <param name="crumb">the jenkins crumb</param>
        public JenkinsConnection(string url, string crumb)
        {
            this.url = url;
            this.crumb = crumb;
        }

        /// <summary>
        /// Make a GET request to jenkins.
        /// </summary>
        /// <remarks>Content Type is <c>text/json</c></remarks>
        /// <param name="command">the uri to GET (appended to server address)</param>
        /// <returns>the response from the jenkins server</returns>
        public string Get(string command)
        {
            return this.CallRequest(
                new AuthorisedRequest(
                    new GetRequest(
                        new JsonRequest(
                            new HttpRequest(this.url, command))),
                    this.username,
                    this.apiToken,
                    this.crumb));
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
            return this.CallRequest(
                new ContentTypeRequest(
                    new AuthorisedRequest(
                        new PostRequest(
                            new HttpRequest(this.url, command),
                            postData),
                        this.username,
                        this.apiToken,
                        this.crumb),
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