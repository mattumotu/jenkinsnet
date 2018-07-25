namespace JenkinsNetClient.Request
{
    public class ContentTypeRequest : IRequest
    {
        private readonly IRequest origin;
        private readonly string contentType;

        public ContentTypeRequest(IRequest request, string contentType)
        {
            this.origin = request;
            this.contentType = contentType;
        }

        public System.Net.HttpWebRequest Build()
        {
            var req = this.origin.Build();
            req.ContentType = this.contentType;
            return req;
        }
    }
}
