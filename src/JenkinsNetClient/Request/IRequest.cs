namespace JenkinsNetClient.Request
{
    using System.Net;

    /// <summary>
    /// Defines the <see cref="IRequest" />
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Build a populated HttpWebRequest
        /// </summary>
        /// <returns>The <see cref="HttpWebRequest"/></returns>
        HttpWebRequest Build();
    }
}