using UnityEngine;
using System;
using TriInspector;

namespace _Main._Data
{
    /// <summary>
    /// Represents the configuration for a particle system in the game.
    /// </summary>
    [Serializable]
    public class ParticleData
    {
        #region Serialized Fields

        [Header("Particle Settings")]
        [PropertyTooltip("The name of the particle system.")]
        public string ParticleName;

        [PropertyTooltip("The number of particles to create.")]
        public int ParticleCount = 5;

        [PropertyTooltip("The particle system reference.")]
        public ParticleSystem ParticleSystem;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleData"/> class.
        /// </summary>
        /// <param name="particleSystem">The particle system reference.</param>
        /// <param name="count">The number of particles to create.</param>
        /// <param name="name">The name of the particle system.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="particleSystem"/> or <paramref name="name"/> is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count"/> is less than or equal to zero.</exception>
        public ParticleData(ParticleSystem particleSystem, int count, string name)
        {
            // Validate particleSystem
            ParticleSystem = particleSystem ?? throw new ArgumentNullException(nameof(particleSystem), "Particle system cannot be null.");

            // Validate count
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Particle count must be greater than zero.");
            }
            ParticleCount = count;

            // Validate name
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "Particle name cannot be null or empty.");
            }
            ParticleName = name;
        }

        #endregion
    }
}