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

        private readonly string existsCommand;
        private readonly string createCommand;
        private readonly string deleteCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="JenkinsItem"/> class.
        /// </summary>
        /// <param name="jenkinsConnection">The jenkinsConnection<see cref="IJenkinsConnection"/></param>
        /// <param name="model">The model<see cref="string"/></param>
        /// <param name="name">The name<see cref="string"/></param>
        public JenkinsItem(
            IJenkinsConnection jenkinsConnection,
            string model,
            string name,
            string existsCommand,
            string createCommand,
            string deleteCommand)
        {
            this.JenkinsConnection = jenkinsConnection;
            this.Model = model;
            this.Name = name;
            this.existsCommand = existsCommand;
            this.createCommand = createCommand;
            this.deleteCommand = deleteCommand;
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

        //public bool ExistsTryPost(bool failIfExists, string command, string contentType, string postData)
        //{
        //    if (!this.Exists)
        //    {
        //        return this.JenkinsConnection.TryPost(command, contentType, postData);
        //    }

        //    return !failIfExists;
        //}
    }
}