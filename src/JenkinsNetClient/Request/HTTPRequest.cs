namespace JenkinsNetClient.Request
{
    using System.Net;

    /// <summary>
    /// Defines the <see cref="HttpRequest" />
    /// </summary>
    public class HttpRequest : IRequest
    {
        /// <summary>
        /// Holds the url
        /// </summary>
        private readonly string url;

        /// <summary>
        /// Holds the command
        /// </summary>
        private readonly string command;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequest"/> class.
        /// </summary>
        /// <param name="url">The url<see cref="string"/></param>
        /// <param name="command">The command<see cref="string"/></param>
        public HttpRequest(string url, string command)
        {
            this.url = url;
            this.command = command;
        }

        /// <summary>
        /// Build a populated HttpWebRequest
        /// </summary>
        /// <returns>The <see cref="HttpWebRequest"/></returns>
        public HttpWebRequest Build()
        {
            return (HttpWebRequest)WebRequest.Create(this.url + this.command);
        }
    }
}