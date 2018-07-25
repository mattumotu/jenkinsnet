namespace JenkinsNetClient.Request
{
    public class XmlRequest : IRequest
    {
        /// <summary>
        /// Holds the origin IRequest to be decorated
        /// </summary>
        private readonly IRequest origin;

        public XmlRequest(IRequest request)
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
            req.ContentType = "text/xml";
            return req;
        }
    }
}