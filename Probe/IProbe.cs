namespace Probe
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a Probe used in the Probe System
    /// </summary>
    public interface IProbe
    {
        /// <summary>
        /// The identifier of the Probe
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The description of the Probe
        /// </summary>
        string Description { get; }

        /// <summary>
        /// A set of <see cref="ProbeArg"/> that can be used with the Probe
        /// </summary>
        ISet<ProbeArg> Args { get; }

        /// <summary>
        /// A handler that will be called to resolve requests for the Probe
        /// </summary>
        /// <param name="args">An instance of <see cref="ProbeRunArgs"/> indicating parameters for the Probe</param>
        /// <returns><see cref="Task{TResult}"/> containing a dynamic object</returns>
        Task<dynamic> OnHandle(ProbeRunArgs args);
    }
}
