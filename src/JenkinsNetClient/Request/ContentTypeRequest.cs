namespace JenkinsNetClient.Request
{
    /// <summary>
    /// Defines the <see cref="ContentTypeRequest" />
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTypeRequest"/> class.
        /// </summary>
        /// <param name="request">The request<see cref="IRequest"/></param>
        /// <param name="contentType">The contentType<see cref="string"/></param>
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