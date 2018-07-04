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
        private IJenkinsConnection jenkinsConnection;

        public JenkinsView(IJenkinsConnection jenkinsConnection, string className, string name)
        {
            this.jenkinsConnection = jenkinsConnection;
            this.Class = className;
            this.Name = name;
        }

        /// <summary>
        /// Gets the class (e.g. view type, such as: hudson.model.ListView)
        /// </summary>
        public string Class { get; private set; }

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name { get; private set; }

        public bool Create(bool failIfExists = false)
        {
            if (!this.Exists())
            {
                try
                {
                    this.jenkinsConnection.Post(
                        string.Format("/createView?name={0}&mode={1}", this.Name, this.Class),
                        "application/x-www-form-urlencoded",
                        string.Format("json={{'name': '{0}', 'mode': '{1}'}}", this.Name, this.Class));
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
                    this.jenkinsConnection.Post(string.Format("/view/{0}/doDelete", this.Name), "application/x-www-form-urlencoded", string.Empty);
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
            var result = this.jenkinsConnection.Get(string.Format("/viewExistsCheck?value={0}", this.Name));
            return result.Contains("error");
        }

        public bool Add(JenkinsJob job, bool failIfExists = false)
        {
            if (!this.Contains(job))
            {
                try
                {
                    this.jenkinsConnection.Post(string.Format("/view/{0}/addJobToView?name={1}", this.Name, job.Name), string.Empty, string.Empty);
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
                    this.jenkinsConnection.Post(string.Format("/view/{0}/removeJobFromView?name={1}", this.Name, job.Name), string.Empty, string.Empty);
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

            string responseText = this.jenkinsConnection.Get(string.Format("/view/{0}/api/xml?tree=jobs[name]", this.Name));
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
