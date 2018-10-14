namespace Probe.Service
{
    /// <summary>
    /// Defines behaviour of a builder use in configuring the available <see cref="IProbe"/>s.
    /// </summary>
    public interface IProbeBuilder
    {
        /// <summary>
        /// Adds an <see cref="IProbe"/> of the given type to the configured probes.
        /// </summary>
        /// <typeparam name="T">The type of the Probe to add</typeparam>
        /// <returns>Reference to the current IProbeBuilder object</returns>
        IProbeBuilder AddProbe<T>() where T : IProbe;
    }
}
