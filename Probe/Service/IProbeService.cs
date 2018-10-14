namespace Probe.Service
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines behaviour of a ProbeService
    /// </summary>
    public interface IProbeService
    {
        /// <summary>
        /// Gets <see cref="ProbeInfo"/> objects representing all commands available to the Probe system.
        /// </summary>
        /// <returns><see cref="IEnumerable{ProbeInfo}"/></returns>
        IEnumerable<ProbeInfo> GetAllProbes();

        /// <summary>
        /// Gets a <see cref="IProbe"/> in the probe system by the id.
        /// </summary>
        /// <param name="id">The id of the <see cref="IProbe"/> to find</param>
        /// <returns><see cref="IProbe"/></returns>
        IProbe GetProbe(string id);
    }
}