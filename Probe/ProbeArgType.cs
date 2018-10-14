namespace Probe
{
    /// <summary>
    /// Enumeration indicating the type of input accepted by a probe argument
    /// </summary>
    public enum ProbeArgType
    {
        /// <summary>
        /// The argument is a string type
        /// </summary>
        String,

        /// <summary>
        /// The argument is a date type (no time)
        /// </summary>
        Date,

        /// <summary>
        /// The argument is a number (floating or whole) type
        /// </summary>
        Number,

        /// <summary>
        /// The argument is a date time type
        /// </summary>
        DateTime
    }
}
