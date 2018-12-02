namespace Probe
{
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Options for configuring the Probe service
    /// </summary>
    public class ProbeOptions
    {
        internal const string DefaultApiPath = "/api/probe";
        internal const string DefaultClientPath = "/";

        /// <summary>
        /// Gets or sets a value indicating whether the in-built web client should be configured
        /// </summary>
        public bool UseWebClient { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the root relative path (i.e. starting with "/") where the web client should be served
        /// </summary>
        public string WebClientPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the root relative path (i.e. starting with "/") where the api should be served
        /// </summary>
        public string ProbeApiPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the Api root as a PathString
        /// </summary>
        internal PathString ApiBase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the WebClient root as a PathString
        /// </summary>
        internal PathString ClientBase { get; set; }

        /// <summary>
        /// Initialises the object using default options.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default values for the options are:
        /// </para>
        /// <list type="bullet">
        /// <item>UseWebClient: true</item>
        /// <item>WebClientPath: "/"</item>
        /// <item>ProbeApiPath: "/api/probe"</item>
        /// </list>
        /// </remarks>
        public ProbeOptions()
        {
            UseWebClient = true;
            WebClientPath = DefaultClientPath;
            ProbeApiPath = DefaultApiPath;
        }
    }
}
