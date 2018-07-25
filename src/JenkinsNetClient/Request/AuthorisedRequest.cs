namespace JenkinsNetClient.Request
{
    public class AuthorisedRequest : IRequest
    {
        /// <summary>
        /// Holds the origin IRequest to be decorated
        /// </summary>
        private readonly IRequest origin;

        /// <summary>
        /// Holds the username
        /// </summary>
        private readonly string username;

        /// <summary>
        /// Holds the api token
        /// </summary>
        private readonly string apiToken;

        public AuthorisedRequest(IRequest request, string username, string apiToken)
        {
            this.origin = request;
            this.username = username;
            this.apiToken = apiToken;
        }

        /// <summary>
        /// Build a populated HttpWebRequest
        /// </summary>
        /// <returns>The <see cref="HttpWebRequest"/></returns>
        public System.Net.HttpWebRequest Build()
        {
            var req = this.origin.Build();

            if (!string.IsNullOrEmpty(this.username))
            {
                string mergedCredentials = string.Format("{0}:{1}", this.username, this.apiToken);
                byte[] byteCredentials = System.Text.UTF8Encoding.UTF8.GetBytes(mergedCredentials);
                string base64Credentials = System.Convert.ToBase64String(byteCredentials);
                req.Headers.Add("Authorization", "Basic " + base64Credentials);
            }

            return req;
        }
    }
}