namespace JenkinsNetClient.Request
{
    public class ContentTypeRequest : IRequest
    {
        /// <summary>
        /// Holds the origin IRequest to be decorated
        /// </summary>
        private readonly IRequest origin;

        /// <summary>
        /// Holds the content type
        /// </summary>
        private readonly string contentType;

        public ContentTypeRequest(IRequest request, string contentType)
        {
            this.origin = request;
            this.contentType = contentType;
        }

        /// <summary>
        /// Build a populated HttpWebRequest
        /// </summary>
        /// <returns>The <see cref="HttpWebRequest"/></returns>
        public System.Net.HttpWebRequest Build()
        {
            var req = this.origin.Build();
            req.ContentType = this.contentType;
            return req;
        }
    }
}