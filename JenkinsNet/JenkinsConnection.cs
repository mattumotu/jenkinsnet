namespace JenkinsNet
{
    using System.IO;
    using System.Net;

    /// <summary>
    /// A connection to a jenkins server
    /// </summary>
    public class JenkinsConnection
    {
        /// <summary>
        /// Holds the jenkins server url
        /// </summary>
        private string url;

        /// <summary>
        /// Initializes a new instance of the <see cref="JenkinsConnection" /> class.
        /// </summary>
        /// <param name="url">the target jenkins server url</param>
        public JenkinsConnection(string url)
        {
            this.url = url;
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
