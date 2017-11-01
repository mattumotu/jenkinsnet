namespace JenkinsNet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;

    public class MockConnection : IJenkinsConnection
    {
        private Dictionary<string, JenkinsView> views;
        private Dictionary<string, JenkinsJob> jobs;
        private Dictionary<string, List<string>> viewJobs;

        public MockConnection()
        {
            // TODO: Add a numch of fake jobs
            this.jobs = new Dictionary<string, JenkinsJob>();
            this.jobs.Add("job 1", new JenkinsJob(this, "hudson.model.FreeStyleProject", "job 1"));
            this.jobs.Add("job 2", new JenkinsJob(this, "hudson.model.FreeStyleProject", "job 2"));
            this.jobs.Add("job 3", new JenkinsJob(this, "hudson.model.FreeStyleProject", "job 3"));
            this.jobs.Add("job 4", new JenkinsJob(this, "hudson.model.FreeStyleProject", "job 4"));
            this.jobs.Add("job 5", new JenkinsJob(this, "hudson.model.FreeStyleProject", "job 5"));
            this.jobs.Add("job 6", new JenkinsJob(this, "hudson.model.FreeStyleProject", "job 6"));

            this.views = new Dictionary<string, JenkinsView>();
            this.viewJobs = new Dictionary<string, List<string>>();

            // Add all view containing all jobs;
            this.views.Add("all", new JenkinsView(this, "hudson.model.AllView", "all"));
            this.viewJobs.Add("all", new List<string>());
            this.viewJobs["all"].Add("job 1");
            this.viewJobs["all"].Add("job 2");
            this.viewJobs["all"].Add("job 3");
            this.viewJobs["all"].Add("job 4");
            this.viewJobs["all"].Add("job 5");
            this.viewJobs["all"].Add("job 6");

            // Add a bunch of views containing some jobs
            this.views.Add("view 1", new JenkinsView(this, "hudson.model.ListView", "view 1"));
            this.viewJobs.Add("view 1", new List<string>());
            this.viewJobs["view 1"].Add("job 1");
            this.viewJobs["view 1"].Add("job 2");
            this.viewJobs["view 1"].Add("job 3");

            this.views.Add("view 2", new JenkinsView(this, "hudson.model.ListView", "view 2"));
            this.viewJobs.Add("view 2", new List<string>());
            this.viewJobs["view 2"].Add("job 4");
            this.viewJobs["view 2"].Add("job 5");

            this.views.Add("view 3", new JenkinsView(this, "hudson.model.ListView", "view 3"));
            this.viewJobs.Add("view 3", new List<string>());
            this.viewJobs["view 3"].Add("job 5");
            this.viewJobs["view 3"].Add("job 6");
        }

        public string Get(string command)
        {
            if (command == "/api/xml?tree=views[name]")
            {
                return this.GetAllViewsXML();
            }

            Match match = Regex.Match(command, @"/view/(.+)/api/xml\?tree=jobs\[name\]$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return this.GetJobsForViewXML(match.Groups[1].Value);
            }

            match = Regex.Match(command, @"/viewExistsCheck\?value=(.+)$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return this.ViewExists(match.Groups[1].Value);
            }

            throw new NotImplementedException(command);
        }

        public string Post(string command, string contentType, string postData)
        {
            Match match = Regex.Match(command, @"/createView\?name=(.+)&mode=(.+)$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return this.CreateView(match.Groups[1].Value, match.Groups[2].Value);
            }

            match = Regex.Match(command, @"/view/(.+)/doDelete$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return this.DeleteView(match.Groups[1].Value);
            }

            match = Regex.Match(command, @"/view/(.+)/addJobToView\?name=(.+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return this.ViewAddJob(match.Groups[1].Value, match.Groups[2].Value);
            }

            match = Regex.Match(command, @"/view/(.+)/removeJobFromView\?name=(.+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return this.ViewRemoveJob(match.Groups[1].Value, match.Groups[2].Value);
            }

            throw new NotImplementedException(command);
        }

        private string GetAllViewsXML()
        {
            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("root");
            xml.AppendChild(root);

            foreach (var view in this.views.Values.ToList())
            {
                XmlElement viewElement = xml.CreateElement("view");

                XmlAttribute ClassAttribute = xml.CreateAttribute("_class");
                ClassAttribute.Value = view.Class;
                viewElement.Attributes.Append(ClassAttribute);

                XmlElement nameElement = xml.CreateElement("name");
                nameElement.InnerText = view.Name;
                viewElement.AppendChild(nameElement);

                root.AppendChild(viewElement);
            }

            return xml.OuterXml;
        }

        private string GetJobsForViewXML(string viewName)
        {
            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("root");
            xml.AppendChild(root);

            foreach (var jobName in this.viewJobs[viewName])
            {
                var job = this.jobs[jobName];

                XmlElement jobElement = xml.CreateElement("job");

                XmlAttribute ClassAttribute = xml.CreateAttribute("_class");
                ClassAttribute.Value = job.Class;
                jobElement.Attributes.Append(ClassAttribute);

                XmlElement nameElement = xml.CreateElement("name");
                nameElement.InnerText = job.Name;
                jobElement.AppendChild(nameElement);

                root.AppendChild(jobElement);
            }

            return xml.OuterXml;
        }

        private string ViewExists(string viewName)
        {
            if (this.views.Keys.Contains(viewName))
            {
                return "error";
            }
            return "";
        }

        private string CreateView(string name, string className)
        {
            if (!this.views.Keys.Contains(name))
            {
                this.views.Add(name, new JenkinsView(this, className, name));
                this.viewJobs.Add(name, new List<string>());
                return string.Empty;
            }
            throw new Exception("Create View");
        }

        private string DeleteView(string viewName)
        {
            if (this.views.Keys.Contains(viewName))
            {
                this.viewJobs.Remove(viewName);
                this.views.Remove(viewName);
                return true.ToString();
            }
            return false.ToString();
        }

        private string ViewAddJob(string viewName, string jobName)
        {
            if (this.views.Keys.Contains(viewName) && this.jobs.Keys.Contains(jobName))
            {
                if (!this.viewJobs[viewName].Contains(jobName))
                {
                    this.viewJobs[viewName].Add(jobName);
                }
                return string.Empty;
            }
            throw new Exception("View Add Job");
        }

        private string ViewRemoveJob(string viewName, string jobName)
        {
            if (this.views.Keys.Contains(viewName) && this.jobs.Keys.Contains(jobName) && this.viewJobs[viewName].Contains(jobName))
            {
                this.viewJobs[viewName].Remove(jobName);
                return string.Empty;
            }
            throw new Exception("View Remove Job");
        }
    }
}
