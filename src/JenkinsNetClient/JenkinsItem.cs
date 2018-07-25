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
        /// Initializes a new instance of the <see cref="JenkinsItem"/> class.
        /// </summary>
        /// <param name="jenkinsConnection">The jenkinsConnection<see cref="IJenkinsConnection"/></param>
        /// <param name="model">The model<see cref="string"/></param>
        /// <param name="name">The name<see cref="string"/></param>
        public JenkinsItem(IJenkinsConnection jenkinsConnection, string model, string name)
        {
            this.JenkinsConnection = jenkinsConnection;
            this.Model = model;
            this.Name = name;
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
        public abstract bool Exists { get; }
    }
}