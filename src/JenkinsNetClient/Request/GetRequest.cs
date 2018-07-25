namespace JenkinsNetClient.Request
{
    public class GetRequest : IRequest
    {
        private readonly IRequest origin;

        public GetRequest(IRequest request)
        {
            this.origin = request;
        }

        public System.Net.HttpWebRequest Build()
        {
            var req = this.origin.Build();
            req.Method = "GET";
            return req;
        }
    }
}
