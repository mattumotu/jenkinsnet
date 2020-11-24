namespace JenkinsNetClient.Request
{
    /// <summary>
    /// Defines the <see cref="AuthorisedRequest" />
    /// </summary>
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
        /// Holds the API token
        /// </summary>
        private readonly string apiToken;

        /// <summary>
        /// Holds the crumb
        /// </summary>
        private readonly string crumb;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorisedRequest"/> class.
        /// </summary>
        /// <param name="request">The request<see cref="IRequest"/></param>
        /// <param name="username">The username<see cref="string"/></param>
        /// <param name="apiToken">The API token<see cref="string"/></param>
        public AuthorisedRequest(IRequest request, string username, string apiToken)
        {
            this.origin = request;
            this.username = username;
            this.apiToken = apiToken;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorisedRequest"/> class.
        /// </summary>
        /// <param name="request">The request<see cref="IRequest"/></param>
        /// <param name="username">The username<see cref="string"/></param>
        /// <param name="apiToken">The API token<see cref="string"/></param>
        public AuthorisedRequest(IRequest request, string username, string apiToken, string crumb)
        {
            this.origin = request;
            this.username = username;
            this.apiToken = apiToken;
            this.crumb = crumb;
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
            if (!string.IsNullOrEmpty(this.crumb))
            {
                req.Headers.Add("Jenkins-Crumb", crumb);
            }

            return req;
        }
    }
}