namespace Probe
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines a collection of run arguments for <see cref="ProbeArg"/>s, formatted as strings 
    /// with methods to parse the arguments to the correct type
    /// </summary>
    public class ProbeRunArgs : Dictionary<string, string>
    {
        /// <summary>
        /// Initialises the object
        /// </summary>
        public ProbeRunArgs() : base() { }

        /// <summary>
        /// Parses the argument value in the collection representing a <see cref="ProbeArgType.Date"/> 
        /// as a <see cref="DateTime"/>
        /// </summary>
        /// <exception cref="ProbeArgParseException">Thrown when the argument cannot be parsed to the desired type</exception>
        /// <exception cref="ArgumentNullException">Thrown when the argName parameter is not usable</exception>
        /// <exception cref="FormatException">Thrown when value of the argument specified is null in the collection</exception>
        /// <param name="argName">The name of the argument</param>
        /// <returns><see cref="DateTime"/></returns>
        public DateTime ParseDateArg(string argName)
        {
            return ParseArg<DateTime>(argName);
        }

        /// <summary>
        /// Parses the argument value in the collection representing a <see cref="ProbeArgType.DateTime"/> 
        /// as a <see cref="DateTime"/>
        /// </summary>
        /// <exception cref="ProbeArgParseException">Thrown when the argument cannot be parsed to the desired type</exception>
        /// <exception cref="ArgumentNullException">Thrown when the argName parameter is not usable</exception>
        /// <exception cref="FormatException">Thrown when value of the argument specified is null in the collection</exception>
        /// <param name="argName">The name of the argument</param>
        /// <returns><see cref="DateTime"/></returns>
        public DateTime ParseDateTimeArg(string argName)
        {
            return ParseArg<DateTime>(argName);
        }

        /// <summary>
        /// Parses the argument value in the collection representing a <see cref="ProbeArgType.Number"/> 
        /// as a <see cref="long"/>
        /// </summary>
        /// <exception cref="ProbeArgParseException">Thrown when the argument cannot be parsed to the desired type</exception>
        /// <exception cref="ArgumentNullException">Thrown when the argName parameter is not usable</exception>
        /// <exception cref="FormatException">Thrown when value of the argument specified is null in the collection</exception>
        /// <param name="argName">The name of the argument</param>
        /// <returns><see cref="long"/></returns>
        public long ParseLongNumberArg(string argName)
        {
            return ParseArg<long>(argName);
        }

        /// <summary>
        /// Parses the argument value in the collection representing a <see cref="ProbeArgType.Number"/> 
        /// as a <see cref="double"/>
        /// </summary>
        /// <exception cref="ProbeArgParseException">Thrown when the argument cannot be parsed to the desired type</exception>
        /// <exception cref="ArgumentNullException">Thrown when the argName parameter is not usable</exception>
        /// <exception cref="FormatException">Thrown when value of the argument specified is null in the collection</exception>
        /// <param name="argName">The name of the argument</param>
        /// <returns><see cref="double"/></returns>
        public double ParseDoubleNumberArg(string argName)
        {
            return ParseArg<double>(argName);
        }

        /// <summary>
        /// Parses the argument value in the collection representing a <see cref="ProbeArgType.String"/> 
        /// as a <see cref="long"/>
        /// </summary>
        /// <exception cref="ProbeArgParseException">Thrown when the argument cannot be parsed to the desired type</exception>
        /// <exception cref="ArgumentNullException">Thrown when the argName parameter is not usable</exception>
        /// <exception cref="FormatException">Thrown when value of the argument specified is null in the collection</exception>
        /// <param name="argName">The name of the argument</param>
        /// <returns><see cref="string"/></returns>
        public string ParseStringArg(string argName)
        {
            return ParseArg<string>(argName);
        }

        /// <summary>
        /// Parses the argument value in the collection as the requested type
        /// </summary>
        /// <exception cref="ProbeArgParseException">Thrown when the argument cannot be parsed to the desired type</exception>
        /// <exception cref="ArgumentNullException">Thrown when the argName parameter is not usable</exception>
        /// <exception cref="FormatException">Thrown when value of the argument specified is null in the collection</exception>
        /// <typeparam name="T">The type to parse the argument as</typeparam>
        /// <param name="argName">The name of the argument</param>
        /// <returns>Value in type of the type parameter specified</returns>
        private T ParseArg<T>(string argName)
        {
            if (string.IsNullOrWhiteSpace(argName))
            {
                throw new ArgumentNullException(nameof(argName));
            }

            string val = this[argName];

            if (val == null)
            {
                throw new FormatException($"The argument {argName} is null");
            }

            object outVal = null;
            try
            {
                outVal = Convert.ChangeType(val, typeof(T));
            }
            catch
            {
                throw new ProbeArgParseException(argName, typeof(T));
            }

            return (T)outVal;
        }

        /// <summary>
        /// Outputs the collection of arguments and values in a Json Object Format
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in Keys)
            {
                sb.Append($"\"{key}\": \"{this[key]}\", ");
            }
            return $"{{ {sb.ToString().TrimEnd(' ', ',')} }}";
        }
    }
}
