namespace JenkinsNetClient.Request
{
    using System.IO;

    public class PostRequest : IRequest
    {
        private readonly IRequest origin;
        private readonly string postData;

        public PostRequest(IRequest request, string postData)
        {
            this.origin = request;
            this.postData = postData;
        }

        public System.Net.HttpWebRequest Build()
        {
            var req = this.origin.Build();
            req.Method = "POST";

            if (!string.IsNullOrEmpty(this.postData))
            {
                using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                {
                    streamWriter.Write(postData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            return req;
        }
    }
}
