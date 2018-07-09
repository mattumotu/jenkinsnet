namespace JenkinsNetClient.Request
{
    public class JsonRequest : IRequest
    {
        private IRequest origin;

        public JsonRequest(IRequest request)
        {
            this.origin = request;
        }

        public System.Net.HttpWebRequest Build()
        {
            var req = this.origin.Build();
            req.ContentType = "text/json";
            return req;
        }
    }
}
