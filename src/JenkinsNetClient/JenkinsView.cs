namespace JenkinsNetClient
{
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// Represents a Jenkins View, which is a non-exclusive collection of jobs
    /// </summary>
    public class JenkinsView : JenkinsItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JenkinsView"/> class.
        /// </summary>
        /// <param name="jenkinsConnection">The jenkinsConnection<see cref="IJenkinsConnection"/></param>
        /// <param name="model">The model<see cref="string"/> (e.g. hudson.model.ListView)</param>
        /// <param name="name">The name<see cref="string"/></param>
        public JenkinsView(IJenkinsConnection jenkinsConnection, string model, string name)
            : base(jenkinsConnection, model, name)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this view exists on jenkins
        /// </summary>
        public override bool Exists
        {
            get
            {
                var result = this.JenkinsConnection.Get(string.Format("/viewExistsCheck?value={0}", this.Name));
                return result.Contains("error");
            }
        }

        /// <summary>
        /// Create this view on jenkins, if it doesn't exist
        /// </summary>
        /// <param name="failIfExists">flag<see cref="bool"/> to indicate return value if view already exists</param>
        /// <returns>true<see cref="bool"/>if view created, false<see cref="bool"/> if not created. If view exists returns !<c>failIfExists</c><see cref="bool"/></returns>
        public bool Create(bool failIfExists = false)
        {
            if (!this.Exists)
            {
                return this.JenkinsConnection.TryPost(
                    string.Format("/createView?name={0}&mode={1}", this.Name, this.Model),
                    "application/x-www-form-urlencoded",
                    string.Format("json={{'name': '{0}', 'mode': '{1}'}}", this.Name, this.Model));
            }

            return !failIfExists;
        }

        /// <summary>
        /// Delete this view on jenkins, if it exists
        /// </summary>
        /// <param name="failIfNotExists">flag<see cref="bool"/> to indicate return value if job doesn't exist</param>
        /// <returns>true<see cref="bool"/> if view deleted, false<see cref="bool"/> if not deleted. If view doesn't exist returns !<c>failIfExists</c><see cref="bool"/></returns>
        public bool Delete(bool failIfNotExists = false)
        {
            if (this.Exists)
            {
                return this.JenkinsConnection.TryPost(
                    string.Format("/view/{0}/doDelete", this.Name),
                    "application/x-www-form-urlencoded",
                    string.Empty);
            }

            return !failIfNotExists;
        }

        /// <summary>
        /// The Add
        /// </summary>
        /// <param name="job">The job<see cref="JenkinsJob"/></param>
        /// <param name="failIfExists">The failIfExists<see cref="bool"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool Add(JenkinsJob job, bool failIfExists = false)
        {
            if (!this.Contains(job))
            {
                try
                {
                    this.JenkinsConnection.Post(string.Format("/view/{0}/addJobToView?name={1}", this.Name, job.Name), string.Empty, string.Empty);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return !failIfExists;
        }

        /// <summary>
        /// The Remove
        /// </summary>
        /// <param name="job">The job<see cref="JenkinsJob"/></param>
        /// <param name="failIfNotExists">The failIfNotExists<see cref="bool"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool Remove(JenkinsJob job, bool failIfNotExists = false)
        {
            if (this.Contains(job))
            {
                try
                {
                    this.JenkinsConnection.Post(string.Format("/view/{0}/removeJobFromView?name={1}", this.Name, job.Name), string.Empty, string.Empty);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return !failIfNotExists;
        }

        /// <summary>
        /// The Contains
        /// </summary>
        /// <param name="job">The job<see cref="JenkinsJob"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool Contains(JenkinsJob job)
        {
            return this.Jobs().Contains(job);
        }

        /// <summary>
        /// The Jobs
        /// </summary>
        /// <returns>The <see cref="List{JenkinsJob}"/></returns>
        public List<JenkinsJob> Jobs()
        {
            var jobs = new List<JenkinsJob>();

            string responseText = this.JenkinsConnection.Get(string.Format("/view/{0}/api/xml?tree=jobs[name]", this.Name));
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseText);

            XmlNodeList jobNodes = xmlDoc.GetElementsByTagName("job");
            foreach (XmlNode jobNode in jobNodes)
            {
                jobs.Add(new JenkinsJob(this.JenkinsConnection, jobNode.Attributes["_class"].Value, jobNode["name"].InnerText));
            }

            return jobs;
        }
    }
}