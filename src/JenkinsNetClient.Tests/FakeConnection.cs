namespace JenkinsNetClient.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;

    public class FakeConnection : IJenkinsConnection
    {
        private Dictionary<string, JenkinsView> views;
        private Dictionary<string, JenkinsJob> jobs;
        private Dictionary<string, List<string>> viewJobs;
        private Dictionary<string, string> jobConfigs;

        public FakeConnection()
        {
            // Init
            this.jobs = new Dictionary<string, JenkinsJob>();
            this.jobConfigs = new Dictionary<string, string>();
            this.views = new Dictionary<string, JenkinsView>();
            this.viewJobs = new Dictionary<string, List<string>>();

            // Create some fake jobs
            this.CreateJob("job 1", "hudson.model.FreeStyleProject");
            this.CreateJob("job 2", "hudson.model.FreeStyleProject");
            this.CreateJob("job 3", "hudson.model.FreeStyleProject");
            this.CreateJob("job 4", "hudson.model.FreeStyleProject");
            this.CreateJob("job 5", "hudson.model.FreeStyleProject");
            this.CreateJob("job 6", "hudson.model.FreeStyleProject");

            // Create some fake views
            this.CreateView("all", "hudson.model.AllView");
            this.CreateView("view 1", "hudson.model.ListView");
            this.CreateView("view 2", "hudson.model.ListView");
            this.CreateView("view 3", "hudson.model.ListView");

            // Add all jobs to all view
            foreach (var jobName in this.jobs.Keys.ToList())
            {
                this.AddJobToView("all", jobName);
            }

            // Add some jobs to views
            this.AddJobToView("view 1", "job 1");
            this.AddJobToView("view 1", "job 2");
            this.AddJobToView("view 1", "job 3");

            this.AddJobToView("view 2", "job 4");
            this.AddJobToView("view 2", "job 5");

            this.AddJobToView("view 3", "job 5");
            this.AddJobToView("view 3", "job 6");
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

            match = Regex.Match(command, @"/checkJobName\?value=(.+)$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return this.JobExists(match.Groups[1].Value);
            }

            match = Regex.Match(command, @"/job/(.+)/config.xml$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return this.GetJobConfig(match.Groups[1].Value);
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
                return this.AddJobToView(match.Groups[1].Value, match.Groups[2].Value);
            }

            match = Regex.Match(command, @"/view/(.+)/removeJobFromView\?name=(.+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return this.RemoveJobFromView(match.Groups[1].Value, match.Groups[2].Value);
            }

            match = Regex.Match(command, @"/createItem\?name=(.+)&mode=(.+)$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return this.CreateJob(match.Groups[1].Value, match.Groups[2].Value);
            }

            match = Regex.Match(command, @"/job/(.+)/doDelete$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return this.DeleteJob(match.Groups[1].Value);
            }

            match = Regex.Match(command, @"/job/(.+)/config.xml$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return this.SetJobConfig(match.Groups[1].Value, postData);
            }

            throw new NotImplementedException(command);
        }

        public bool TryPost(string command, string contentType, string postData)
        {
            try
            {
                this.Post(
                    command,
                    contentType,
                    postData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region "Get"

        private string GetAllViewsXML()
        {
            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("root");
            xml.AppendChild(root);

            foreach (var view in this.views.Values.ToList())
            {
                XmlElement viewElement = xml.CreateElement("view");

                XmlAttribute ClassAttribute = xml.CreateAttribute("_class");
                ClassAttribute.Value = view.Model;
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
                ClassAttribute.Value = job.Model;
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

        private string JobExists(string jobName)
        {
            if (this.jobs.Keys.Contains(jobName))
            {
                return "error";
            }
            return "";
        }

        private string GetJobConfig(string jobName)
        {
            if (this.jobConfigs.Keys.Contains(jobName))
            {
                return this.jobConfigs[jobName];
            }
            throw new NotImplementedException();
        }

        #endregion "Get"

        #region "Post"

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

        private string AddJobToView(string viewName, string jobName)
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

        private string RemoveJobFromView(string viewName, string jobName)
        {
            if (this.views.Keys.Contains(viewName) && this.jobs.Keys.Contains(jobName) && this.viewJobs[viewName].Contains(jobName))
            {
                this.viewJobs[viewName].Remove(jobName);
                return string.Empty;
            }
            throw new Exception("View Remove Job");
        }

        private string CreateJob(string name, string className)
        {
            if (!this.jobs.Keys.Contains(name))
            {
                this.jobs.Add(name, new JenkinsJob(this, className, name));
                this.jobConfigs.Add(name, "<project></project>");
                return string.Empty;
            }
            throw new Exception("Create Job");
        }

        private string DeleteJob(string jobName)
        {
            if (this.jobs.Keys.Contains(jobName))
            {
                foreach (var jobList in this.viewJobs.Values.ToList())
                {
                    if (jobList.Contains(jobName))
                    {
                        jobList.Remove(jobName);
                    }
                }
                this.jobs.Remove(jobName);
                return true.ToString();
            }
            return false.ToString();
        }

        private string SetJobConfig(string jobName, string xml)
        {
            if (this.jobConfigs.Keys.Contains(jobName))
            {
                this.jobConfigs[jobName] = xml;
                return string.Empty;
            }
            throw new NotImplementedException();
        }

        #endregion "Post"
    }
}