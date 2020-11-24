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
            : base(
                  jenkinsConnection,
                  model,
                  name,
                  string.Format("/viewExistsCheck?value={0}", name),
                  string.Format("/createView?name={0}&mode={1}", name, model),
                  string.Format("/view/{0}/doDelete", name),
                  string.Format("/job/{0}/buildWithParameters", name))
        {
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