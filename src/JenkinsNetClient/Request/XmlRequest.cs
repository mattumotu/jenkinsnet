namespace JenkinsNetClient.Request
{
    public class XmlRequest : IRequest
    {
        private readonly IRequest origin;

        public XmlRequest(IRequest request)
        {
            this.origin = request;
        }

        public System.Net.HttpWebRequest Build()
        {
            var req = this.origin.Build();
            req.ContentType = "text/xml";
            return req;
        }
    }
}
