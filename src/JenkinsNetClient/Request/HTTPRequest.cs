namespace JenkinsNetClient.Request
{
    using System.Net;

    public class HttpRequest : IRequest
    {
        private readonly string url;
        private readonly string command;

        public HttpRequest(string url, string command)
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
