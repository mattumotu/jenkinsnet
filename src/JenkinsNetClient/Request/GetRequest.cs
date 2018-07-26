namespace JenkinsNetClient.Request
{
    /// <summary>
    /// Defines the <see cref="GetRequest" />
    /// </summary>
    public class GetRequest : IRequest
    {
        /// <summary>
        /// Holds the origin IRequest to be decorated
        /// </summary>
        private readonly IRequest origin;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRequest"/> class.
        /// </summary>
        /// <param name="request">The request<see cref="IRequest"/></param>
        public GetRequest(IRequest request)
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
            req.Method = "GET";
            return req;
        }
    }
}