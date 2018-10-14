namespace Probe
{
    /// <summary>
    /// Represents an argument that can be passed to a <see cref="IProbe"/>
    /// </summary>
    public struct ProbeArg
    {
        /// <summary>
        /// Gets a name for the argument, used as an identifier
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a <see cref="ProbeArgType"/> value indicating the type of the argument
        /// </summary>
        public ProbeArgType Type { get; }

        /// <summary>
        /// Gets a value indicating whether this argument is required when requesting a Probe
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        /// Initialises the ProbeArg with the given values
        /// </summary>
        /// <param name="name">The name of the argument, used as an identifier</param>
        /// <param name="type">The <see cref="ProbeArgType"/> value indicating the type of the argument</param>
        /// <param name="isRequired">Indicates whether this argument is required when requesting a Probe (optional, default is false)</param>
        public ProbeArg(string name, ProbeArgType type, bool isRequired = false)
        {
            Name = name;
            Type = type;
            IsRequired = isRequired;
        }
    }
}
