namespace Probe
{
    using System.Collections.Generic;

    /// <summary>
    /// Describes an <see cref="IProbe"/>.
    /// </summary>
    public class ProbeInfo
    {
        /// <summary>
        /// An identifier for the Probe
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A description of the Probe
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A set of <see cref="ProbeArg"/> that can be used with the Probe
        /// </summary>
        public ISet<ProbeArg> Args { get; set; }
    }
}
