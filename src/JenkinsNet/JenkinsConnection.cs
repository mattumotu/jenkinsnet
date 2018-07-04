namespace JenkinsNet
{
    using System.IO;
    using System.Net;

    /// <summary>
    /// A connection to a jenkins server
    /// </summary>
    public class JenkinsConnection : JenkinsNet.IJenkinsConnection
    {
        /// <summary>
        /// Holds the jenkins server url
        /// </summary>
        private string url;

        /// <summary>
        /// Holds the jenkins username
        /// </summary>
        private string username;

        /// <summary>
        /// Holds the jenkins api token
        /// </summary>
        private string apiToken;

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
        /// <param name="apiToken">the jenkins api token</param>
        public JenkinsConnection(string url, string username, string apiToken)
        {
            this.url = url;
            this.username = username;
            this.apiToken = apiToken;
        }

        /// <summary>
        /// Make a GET request to jenkins.
        /// </summary>
        /// <remarks>Content Type is <c>text/json</c></remarks>
        /// <param name="command">the uri to GET (appended to server address)</param>
        /// <returns>the response from the jenkins server</returns>
        public string Get(string command)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(this.url + command);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "GET";

            if (!string.IsNullOrEmpty(this.username))
            {
                string mergedCredentials = string.Format("{0}:{1}", this.username, this.apiToken);
                byte[] byteCredentials = System.Text.UTF8Encoding.UTF8.GetBytes(mergedCredentials);
                string base64Credentials = System.Convert.ToBase64String(byteCredentials);
                httpWebRequest.Headers.Add("Authorization", "Basic " + base64Credentials);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
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
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(this.url + command);
            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = "POST";

            if (!string.IsNullOrEmpty(this.username))
            {
                string mergedCredentials = string.Format("{0}:{1}", this.username, this.apiToken);
                byte[] byteCredentials = System.Text.UTF8Encoding.UTF8.GetBytes(mergedCredentials);
                string base64Credentials = System.Convert.ToBase64String(byteCredentials);
                httpWebRequest.Headers.Add("Authorization", "Basic " + base64Credentials);
            }

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(postData);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
