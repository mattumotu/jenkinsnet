namespace JenkinsNetClient.Request
{
    public class JsonRequest : IRequest
    {
        /// <summary>
        /// Holds the origin IRequest to be decorated
        /// </summary>
        private readonly IRequest origin;

        public JsonRequest(IRequest request)
        {
            this.origin = request;
        }

        /// <summary>
        /// Build a populated HttpWebRequest
        /// </summary>
        /// <returns>The <see cref="HttpWebRequest"/></returns>
        public System.Net.HttpWebRequest Build()
        {
            var req = this.origin.Build();

            // TODO: should this be application/json? need to test with Jenkins
            req.ContentType = "text/json";
            return req;
        }
    }
}