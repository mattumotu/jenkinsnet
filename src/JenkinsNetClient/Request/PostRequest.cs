namespace JenkinsNetClient.Request
{
    using System.IO;

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
                    streamWriter.Close();
                }
            }

            return req;
        }
    }
}