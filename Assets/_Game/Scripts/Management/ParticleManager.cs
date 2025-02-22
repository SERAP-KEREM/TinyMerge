using _Main._Data;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using Zenject;

namespace _Main._Management
{
    /// <summary>
    /// Manages the generation, activation, and deactivation of particle effects in the game.
    /// </summary>
    public class ParticleManager : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Particle Configuration")]
        [PropertyTooltip("List of particle data configurations.")]
        public List<ParticleData> ParticleDataList = new List<ParticleData>();

        [HideInInspector]
        public List<ParticleSystem> ParticleList = new List<ParticleSystem>();

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Initializes the particle manager by generating particles and resetting their states.
        /// </summary>
        private void Awake()
        {
            GenerateParticles();
            StopAndDeactivateAllParticles();
        }

        #endregion

        #region Particle Generation

        /// <summary>
        /// Generates particles based on the configurations in ParticleDataList.
        /// </summary>
        private void GenerateParticles()
        {
            foreach (var data in ParticleDataList)
            {
                for (int i = 0; i < data.ParticleCount; i++)
                {
                    ParticleSystem particleInstance = Instantiate(data.ParticleSystem);
                    particleInstance.Stop();
                    particleInstance.name = data.ParticleName;
                    particleInstance.transform.SetParent(transform);
                    ParticleList.Add(particleInstance);
                }
            }
        }

        /// <summary>
        /// Stops and deactivates all particles, resetting them to the manager's transform.
        /// </summary>
        private void StopAndDeactivateAllParticles()
        {
            foreach (var particle in ParticleList)
            {
                particle.Stop();
                particle.gameObject.SetActive(false);
                particle.transform.SetParent(transform);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Plays a particle effect at the specified position with the given rotation and parent transform.
        /// </summary>
        /// <param name="particleName">The name of the particle effect to play.</param>
        /// <param name="position">The position to play the particle at.</param>
        /// <param name="rotation">The rotation to apply to the particle.</param>
        /// <param name="parent">The parent transform to attach the particle to (optional).</param>
        public void PlayParticleAtPoint(string particleName, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var particle = GetAvailableParticle(particleName);
            if (particle != null)
            {
                ActivateParticle(particle, position, rotation, parent);
            }
        }

        /// <summary>
        /// Plays a particle effect at the specified position with an optional parent transform.
        /// </summary>
        /// <param name="particleName">The name of the particle effect to play.</param>
        /// <param name="position">The position to play the particle at.</param>
        /// <param name="parent">The parent transform to attach the particle to (optional).</param>
        public void PlayParticleAtPoint(string particleName, Vector3 position, Transform parent = null)
        {
            PlayParticleAtPoint(particleName, position, Quaternion.identity, parent);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Retrieves an available particle system that matches the given name and is not currently playing.
        /// </summary>
        /// <param name="particleName">The name of the particle effect to retrieve.</param>
        /// <returns>An available ParticleSystem, or null if none are found.</returns>
        private ParticleSystem GetAvailableParticle(string particleName)
        {
            foreach (var particle in ParticleList)
            {
                if (particle.name == particleName && !particle.isPlaying)
                {
                    return particle;
                }
            }
            Debug.LogWarning($"No available particle found with name: {particleName}");
            return null;
        }

        /// <summary>
        /// Activates and plays a particle system at the specified position, rotation, and parent.
        /// </summary>
        /// <param name="particle">The particle system to activate and play.</param>
        /// <param name="position">The position to set for the particle.</param>
        /// <param name="rotation">The rotation to set for the particle.</param>
        /// <param name="parent">The parent transform to attach the particle to (optional).</param>
        private void ActivateParticle(ParticleSystem particle, Vector3 position, Quaternion rotation, Transform parent)
        {
            particle.transform.SetParent(parent);
            particle.transform.position = position;
            particle.transform.rotation = rotation;
            particle.gameObject.SetActive(true);
            particle.Play();

            if (!particle.main.loop)
            {
                StartCoroutine(DeactivateAfterTime(particle, particle.main.duration));
            }
        }

        /// <summary>
        /// Deactivates the particle system after a specified time.
        /// </summary>
        /// <param name="particle">The particle system to deactivate.</param>
        /// <param name="time">The time after which the particle system will be deactivated.</param>
        private IEnumerator DeactivateAfterTime(ParticleSystem particle, float time)
        {
            yield return new WaitForSeconds(time);
            particle.gameObject.SetActive(false);
        }

        #endregion
    }
}