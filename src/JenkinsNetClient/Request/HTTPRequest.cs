namespace JenkinsNetClient.Request
{
    using System.Net;

    public class HTTPRequest : IRequest
    {
        private string url;
        private string command;

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
