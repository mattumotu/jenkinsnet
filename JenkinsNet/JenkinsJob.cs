namespace JenkinsNet
{
    using System;

    /// <summary>
    /// Represents a jenkins job, or project.
    /// </summary>
    public class JenkinsJob : IEquatable<JenkinsJob>
    {
        /// <summary>
        /// Holds the name
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Holds the jenkins connection
        /// </summary>
        private JenkinsConnection jenkinsConnection;

        /// <summary>
        /// Holds the class (e.g. project type, such as: hudson.model.FreeStyleProject)
        /// </summary>
        private string className;

        /// <summary>
        /// Initializes a new instance of the <see cref="JenkinsJob" /> class.
        /// </summary>
        /// <param name="jenkinsConnection">the jenkins connection</param>
        /// <param name="className">the class</param>
        /// <param name="name">the name</param>
        public JenkinsJob(JenkinsConnection jenkinsConnection, string className, string name)
        {
            this.jenkinsConnection = jenkinsConnection;
            this.className = className;
            this.name = name;
        }

        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets or sets the Config XML on jenkins server
        /// </summary>
        public string Config
        {
            get
            {
                return this.jenkinsConnection.Get(string.Format("/job/{0}/config.xml", this.name));
            }

            set
            {
                this.jenkinsConnection.Post(string.Format("/job/{0}/config.xml", this.name), "text/xml", value);
            }
        }

        /// <summary>
        /// Create this job on jenkins, if it doesn't exist
        /// </summary>
        /// <param name="failIfExists">flag to indicate return value if job already exists</param>
        /// <returns>true if job created, false if not created. If job exists returns !<c>failIfExists</c></returns>
        public bool Create(bool failIfExists = false)
        {
            if (!this.Exists())
            {
                try
                {
                    this.jenkinsConnection.Post(
                        string.Format("/createItem?name={0}&mode={1}", this.name, this.className),
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

        /// <summary>
        /// Delete this job on jenkins, if it exists
        /// </summary>
        /// <param name="failIfNotExists">flag to indicate return value if job doesn't exist</param>
        /// <returns>true if job deleted, false if deletion failed. If job doesn't exists return !<c>failIfNotExists</c></returns>
        public bool Delete(bool failIfNotExists = false)
        {
            if (this.Exists())
            {
                try
                {
                    this.jenkinsConnection.Post(string.Format("/job/{0}/doDelete", this.name), "application/x-www-form-urlencoded", string.Empty);
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
        /// Check if this job exists on jenkins
        /// </summary>
        /// <returns>true if job exists; otherwise false</returns>
        public bool Exists()
        {
            var result = this.jenkinsConnection.Get(string.Format("/checkJobName?value={0}", this.name));
            return result.Contains("error");
        }

        /// <summary>
        /// Determines whether this instance and a specified object, which must also
        /// be a JenkinsJob object, have the same value.
        /// </summary>
        /// <param name="obj">The JenkinsJob to compare to this instance.</param>
        /// <returns>true if <c>obj</c> is a JenkinsJob and its Name is the same as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as JenkinsJob);
        }

        /// <summary>
        /// Determines whether this instance and a specified JenkinsJob have the same value.
        /// </summary>
        /// <param name="other">The JenkinsJob to compare to this instance.</param>
        /// <returns>true if <c>obj</c> is a JenkinsJob and its Name is the same as this instance; otherwise, false.</returns>
        public bool Equals(JenkinsJob other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Name.Equals(other.Name);
        }

        /// <summary>
        /// Returns the hash code for this JenkinsJob.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}
