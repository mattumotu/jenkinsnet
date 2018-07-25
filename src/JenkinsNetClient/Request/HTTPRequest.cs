namespace JenkinsNetClient.Request
{
    using System.Net;

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