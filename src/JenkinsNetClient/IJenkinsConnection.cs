namespace JenkinsNetClient
{
    using System;

    /// <summary>
    /// Interface for a connection to a jenkins server
    /// </summary>
    public interface IJenkinsConnection
    {
        /// <summary>
        /// Make a GET request to jenkins.
        /// </summary>
        /// <remarks>Content Type is <c>text/json</c></remarks>
        /// <param name="command">the uri to GET (appended to server address)</param>
        /// <returns>the response from the jenkins server</returns>
        string Get(string command);

        /// <summary>
        /// Make a POST request to jenkins
        /// </summary>
        /// <param name="command">the uri to POST (appended to server address)</param>
        /// <param name="contentType">the content type</param>
        /// <param name="postData">the data to be posted</param>
        /// <returns>the response from the jenkins server</returns>
        string Post(string command, string contentType, string postData);
    }
}
