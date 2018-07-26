namespace JenkinsNetClient.Tests.Request
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using JenkinsNetClient.Request;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RequestTest
    {
        [TestMethod]
        public void Authorised_Request_Test()
        {
            //Assign
            IRequest baseRequest = new HttpRequest("http://test.com/", "command");
            IRequest target;
            HttpWebRequest actual;

            //Act
            target = new AuthorisedRequest(baseRequest, "username", "apiToken");
            actual = target.Build();

            //Assert
            Assert.IsTrue(actual.Headers.AllKeys.Contains("Authorization"));
        }

        [TestMethod]
        public void ContentType_Request_Test()
        {
            //Assign
            IRequest baseRequest = new HttpRequest("http://test.com/", "command");
            string contentType = "contentType";
            IRequest target;
            HttpWebRequest actual;

            //Act
            target = new ContentTypeRequest(baseRequest, contentType);
            actual = target.Build();

            //Assert
            Assert.AreEqual(contentType, actual.ContentType);
        }

        [TestMethod]
        public void Get_Request_Test()
        {
            //Assign
            IRequest baseRequest = new HttpRequest("http://test.com/", "command");
            IRequest target;
            HttpWebRequest actual;

            //Act
            target = new GetRequest(baseRequest);
            actual = target.Build();

            //Assert
            Assert.AreEqual("GET", actual.Method);
        }

        [TestMethod]
        public void HttpRequest_Request_Test()
        {
            //Assign
            string url = "http://test.com/";
            string command = "command";
            IRequest target;
            HttpWebRequest actual;

            //Act
            target = new HttpRequest(url, command);
            actual = target.Build();

            //Assert
            Assert.AreEqual(new Uri(url + command), actual.RequestUri);
        }

        [TestMethod]
        public void Json_Request_Test()
        {
            //Assign
            IRequest baseRequest = new HttpRequest("http://test.com/", "command");
            IRequest target;
            HttpWebRequest actual;

            //Act
            target = new JsonRequest(baseRequest);
            actual = target.Build();

            //Assert
            Assert.AreEqual("application/json", actual.ContentType);
        }

        [TestMethod]
        public void Post_Request_Test()
        {
            //Assign
            IRequest baseRequest = new HttpRequest("http://test.com/", "command");
            IRequest target;
            string postData = "postData";
            HttpWebRequest actual;

            //Act
            target = new PostRequest(baseRequest, postData);
            actual = target.Build();

            //Assert
            Assert.AreEqual("POST", actual.Method);
            //TODO: Check postdata
        }

        [TestMethod]
        public void Xml_Request_Test()
        {
            //Assign
            IRequest baseRequest = new HttpRequest("http://test.com/", "command");
            IRequest target;
            HttpWebRequest actual;

            //Act
            target = new XmlRequest(baseRequest);
            actual = target.Build();

            //Assert
            Assert.AreEqual("text/xml", actual.ContentType);
        }
    }
}