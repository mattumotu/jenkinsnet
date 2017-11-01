namespace JenkinsNet
{
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// Represents a Jenkins View, which is a non-exclusive collection of jobs
    /// </summary>
    public class JenkinsView
    {
        /// <summary>
        /// Holds the jenkins connection
        /// </summary>
        private JenkinsConnection jenkinsConnection;

        /// <summary>
        /// Holds the name
        /// </summary>
        private string name;

        /// <summary>
        /// Holds the class (e.g. view type, such as: hudson.model.ListView)
        /// </summary>
        private string className;

        public JenkinsView(JenkinsConnection jenkinsConnection, string className, string name)
        {
            this.jenkinsConnection = jenkinsConnection;
            this.className = className;
            this.name = name;
        }

        public bool Create(bool failIfExists = false)
        {
            if (!this.Exists())
            {
                try
                {
                    this.jenkinsConnection.Post(
                        string.Format("/createView?name={0}&mode={1}", this.name, this.className),
                        "application/x-www-form-urlencoded",
                        string.Format("json={{'name': '{0}', 'mode': '{1}'}}", this.name, this.className));
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return !failIfExists;
        }

        public bool Delete(bool failIfNotExists = false)
        {
            if (this.Exists())
            {
                try
                {
                    this.jenkinsConnection.Post(string.Format("/view/{0}/doDelete", this.name), "application/x-www-form-urlencoded", string.Empty);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return !failIfNotExists;
        }

        public bool Exists()
        {
            var result = this.jenkinsConnection.Get(string.Format("/viewExistsCheck?value={0}", this.name));
            return result.Contains("error");
        }

        public bool Add(JenkinsJob job, bool failIfExists = false)
        {
            if (!this.Contains(job))
            {
                try
                {
                    this.jenkinsConnection.Post(string.Format("/view/{0}/addJobToView?name={1}", this.name, job.Name), string.Empty, string.Empty);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return !failIfExists;
        }

        public bool Remove(JenkinsJob job, bool failIfNotExists = false)
        {
            if (this.Contains(job))
            {
                try
                {
                    this.jenkinsConnection.Post(string.Format("/view/{0}/removeJobFromView?name={1}", this.name, job.Name), string.Empty, string.Empty);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return !failIfNotExists;
        }

        public bool Contains(JenkinsJob job)
        {
            return this.Jobs().Contains(job);
        }

        public List<JenkinsJob> Jobs()
        {
            var jobs = new List<JenkinsJob>();

            string responseText = this.jenkinsConnection.Get(string.Format("/view/{0}/api/xml?tree=jobs[name]&pretty=true", this.name));
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseText);

            XmlNodeList jobNodes = xmlDoc.GetElementsByTagName("job");
            foreach (XmlNode jobNode in jobNodes)
            {
                jobs.Add(new JenkinsJob(this.jenkinsConnection, jobNode.Attributes["_class"].Value, jobNode["name"].InnerText));
            }

            return jobs;
        }
    }
}
