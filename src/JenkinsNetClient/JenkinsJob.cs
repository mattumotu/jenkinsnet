namespace JenkinsNetClient
{
    using System;

    /// <summary>
    /// Represents a jenkins job, or project.
    /// </summary>
    public sealed class JenkinsJob : JenkinsItem, IEquatable<JenkinsJob>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JenkinsJob"/> class.
        /// </summary>
        /// <param name="jenkinsConnection">the jenkins connection</param>
        /// <param name="model">the model (e.g. hudson.model.FreeStyleProject)</param>
        /// <param name="name">the name</param>
        public JenkinsJob(IJenkinsConnection jenkinsConnection, string model, string name)
            : base(
                  jenkinsConnection,
                  model,
                  name,
                  string.Format("/checkJobName?value={0}", name),
                  string.Format("/createItem?name={0}&mode={1}", name, model),
                  string.Format("/job/{0}/doDelete", name),
                  string.Format("/job/{0}/build", name))
        {
        }

        /// <summary>
        /// Gets or sets the Config XML on jenkins server
        /// </summary>
        public string Config
        {
            get
            {
                return this.JenkinsConnection.Get(string.Format("/job/{0}/config.xml", this.Name));
            }

            set
            {
                this.JenkinsConnection.Post(string.Format("/job/{0}/config.xml", this.Name), "text/xml", value);
            }
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