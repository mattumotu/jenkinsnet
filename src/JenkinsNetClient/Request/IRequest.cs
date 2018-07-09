using System.Net;

namespace JenkinsNetClient.Request
{
    public interface IRequest
    {
        HttpWebRequest Build();
    }
}
