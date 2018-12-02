namespace Probe.Service
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A default, in-memory implementation of the <see cref="IProbeBuilder"/> and <see cref="IProbeService"/> interfaces.
    /// </summary>
    public class ProbeService : IProbeBuilder, IProbeService
    {
        /// <summary>
        /// Types registered by <see cref="IProbeBuilder"/>
        /// </summary>
        private readonly HashSet<Type> probeTypes;

        /// <summary>
        /// Dictionary of <see cref="IProbe"/> instances referenced by Id
        /// </summary>
        private readonly Dictionary<string, IProbe> probes;

        /// <summary>
        /// The aspnetcore <see cref="IServiceProvider"/>
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initialises the object using the specified aspnetcore <see cref="IServiceProvider"/>
        /// </summary>
        /// <param name="serviceProvider">The aspnetcore <see cref="IServiceProvider"/></param>
        public ProbeService(IServiceProvider serviceProvider)
        {
            probeTypes = new HashSet<Type>();
            probes = new Dictionary<string, IProbe>();
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public IProbeBuilder AddProbe<T>() where T : IProbe
        {
            Type type = typeof(T);
            probeTypes.Add(type);
            return this;
        }

        /// <inheritdoc />
        public IEnumerable<ProbeInfo> GetAllProbes()
        {
            return probes.Values.Select(p => new ProbeInfo { Id = p.Id.ToLower(), Description = p.Description, Args = p.Args });
        }

        /// <inheritdoc />
        public IProbe GetProbe(string id)
        {
            var identifier = id.ToLower();
            if (!probes.ContainsKey(identifier))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return probes[id];
        }

        /// <summary>
        /// Called internally and used to create instances of the requested <see cref="IProbe"/> types including any dependencies
        /// specified in the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// This method can only be called once because any registered <see cref="IProbe"/> types are removed as instances are created.
        /// </remarks>
        internal void BuildProbes()
        {
            foreach (Type type in probeTypes)
            {
                var probe = ActivatorUtilities.CreateInstance(serviceProvider, type) as IProbe;

                if (string.IsNullOrEmpty(probe.Id))
                {
                    throw new ArgumentException($"Probe \"{type.Name}\" does not contain an Identifier");
                }

                probes.Add(probe.Id.ToLower(), probe);
            }

            probeTypes.Clear();
        }

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> of registered probe types
        /// </summary>
        internal HashSet<Type> RegisteredProbes => this.probeTypes;

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of Probe Instances
        /// </summary>
        internal Dictionary<string, IProbe> ProbeInstances => this.probes;
    }
}
