namespace JenkinsNetClient.Request
{
    using System.IO;

    /// <summary>
    /// Defines the <see cref="PostRequest" />
    /// </summary>
    public class PostRequest : IRequest
    {
        /// <summary>
        /// Holds the origin IRequest to be decorated
        /// </summary>
        private readonly IRequest origin;

        /// <summary>
        /// Holds the post data
        /// </summary>
        private readonly string postData;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostRequest"/> class.
        /// </summary>
        /// <param name="request">The request<see cref="IRequest"/></param>
        /// <param name="postData">The postData<see cref="string"/></param>
        public PostRequest(IRequest request, string postData)
        {
            this.origin = request;
            this.postData = postData;
        }

        /// <summary>
        /// Build a populated HttpWebRequest
        /// </summary>
        /// <returns>The <see cref="HttpWebRequest"/></returns>
        public System.Net.HttpWebRequest Build()
        {
            var req = this.origin.Build();
            req.Method = "POST";

            if (!string.IsNullOrEmpty(this.postData))
            {
                using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                {
                    streamWriter.Write(this.postData);
                    streamWriter.Flush();
                }
            }

            return req;
        }
    }
}