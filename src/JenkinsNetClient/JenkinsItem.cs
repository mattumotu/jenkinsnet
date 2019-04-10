using System.Collections.Generic;

namespace JenkinsNetClient
{
    /// <summary>
    /// Defines the <see cref="JenkinsItem" />
    /// </summary>
    public abstract class JenkinsItem
    {
        /// <summary>
        /// Holds the jenkins connection
        /// </summary>
        protected readonly IJenkinsConnection JenkinsConnection;

        /// <summary>
        /// Defines the existsCommand
        /// </summary>
        private readonly string existsCommand;

        /// <summary>
        /// Defines the createCommand
        /// </summary>
        private readonly string createCommand;

        /// <summary>
        /// Defines the deleteCommand
        /// </summary>
        private readonly string deleteCommand;

        /// <summary>
        /// Defines the buildCommand
        /// </summary>
        private readonly string buildCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="JenkinsItem"/> class.
        /// </summary>
        /// <param name="jenkinsConnection">The jenkinsConnection<see cref="IJenkinsConnection"/></param>
        /// <param name="model">The model<see cref="string"/></param>
        /// <param name="name">The name<see cref="string"/></param>
        /// <param name="existsCommand">The command for exist check<see cref="string"/></param>
        /// <param name="createCommand">The command for create<see cref="string"/></param>
        /// <param name="deleteCommand">The command for delete<see cref="string"/></param>
        /// <param name="buildcommand">The command for build<see cref="string"/></param>
        protected JenkinsItem(
            IJenkinsConnection jenkinsConnection,
            string model,
            string name,
            string existsCommand,
            string createCommand,
            string deleteCommand,
            string buildCommand)
        {
            this.JenkinsConnection = jenkinsConnection;
            this.Model = model;
            this.Name = name;
            this.existsCommand = existsCommand;
            this.createCommand = createCommand;
            this.deleteCommand = deleteCommand;
            this.buildCommand = buildCommand;
        }

        /// <summary>
        /// Gets the Model
        /// </summary>
        public string Model { get; private set; }

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether Exists
        /// </summary>
        public bool Exists
        {
            get
            {
                var result = this.JenkinsConnection.Get(this.existsCommand);
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
                    this.createCommand,
                    "application/x-www-form-urlencoded",
                    string.Format("json={{'name': '{0}', 'mode': '{1}'}}", this.Name, this.Model));
            }

            return !failIfExists;
        }

        /// <summary>
        /// Delete this job on jenkins, if it exists
        /// </summary>
        /// <param name="failIfNotExists">flag to indicate return value if job doesn't exist</param>
        /// <returns>true if job deleted, false if deletion failed. If job doesn't exist return !<c>failIfNotExists</c></returns>
        public bool Delete(bool failIfNotExists = false)
        {
            if (this.Exists)
            {
                return this.JenkinsConnection.TryPost(
                    this.deleteCommand,
                    "application/x-www-form-urlencoded",
                    string.Empty);
            }

            return !failIfNotExists;
        }

        /// <summary>
        /// Starts a build for this job on jenkins, if it exists
        /// </summary>
        /// <param name="failIfNotExists">flag to indicate return value if job doesn't exist</param>
        /// <returns>true if job has executed, false if execution failed. If job doesn't exist return !<c>failIfNotExists</c></returns>
        public bool BuildWithParameters(Dictionary<string, string> parameters,  bool failIfNotExists = false)
        {
            if (this.Exists)
            {
                string queryString = "?";
                foreach (KeyValuePair<string, string> parameter in parameters)
                {
                    if (queryString != "?")
                        queryString += "&";
                    queryString += $"{System.Uri.EscapeDataString(parameter.Key)}={System.Uri.EscapeDataString(parameter.Value)}";
                }
                return this.JenkinsConnection.TryPost(
                    this.buildCommand + queryString,
                    "application/x-www-form-urlencoded",
                    string.Empty);
            }

            return !failIfNotExists;
        }
    }
}