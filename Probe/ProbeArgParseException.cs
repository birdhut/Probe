namespace Probe
{
    using System;

    /// <summary>
    /// Thrown when a <see cref="ProbeArg"/> cannot be parsed to the requested Format
    /// </summary>
    public class ProbeArgParseException : ArgumentException
    {
        /// <summary>
        /// Initialises the Exception object with the argument name and the expected type that could not be parsed.
        /// </summary>
        /// <param name="argName">The name of the <see cref="ProbeArg"/> that was being parsed</param>
        /// <param name="expectedType">The <see cref="Type"/> that was requested and failed to parse</param>
        public ProbeArgParseException(string argName, Type expectedType) : base($"Unable to parse {argName} to type {expectedType.Name}", argName)
        {
            Data.Add(nameof(argName), argName);
            Data.Add(nameof(expectedType), expectedType);
        }
    }
}
