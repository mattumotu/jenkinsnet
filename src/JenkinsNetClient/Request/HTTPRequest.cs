namespace JenkinsNetClient.Request
{
    using System.Net;

    public class HTTPRequest : IRequest
    {
        private readonly string url;
        private readonly string command;

        public HTTPRequest(string url, string command)
        {
            this.url = url;
            this.command = command;
        }

        public HttpWebRequest Build()
        {
            return (HttpWebRequest)WebRequest.Create(this.url + this.command);
        }
    }
}
