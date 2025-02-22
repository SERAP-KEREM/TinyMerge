using UnityEngine;
using TriInspector;

namespace _Main._Data
{
    /// <summary>
    /// Contains all the properties related to an audio clip.
    /// </summary>
    [System.Serializable]
    public class Audio
    {
        [Header("Audio Settings")]
        [Required, PropertyTooltip("The name of the audio.")]
        public string Name;

        [Header("Audio Settings"), PropertyTooltip("Array of audio clips that can be played.")]
        [SerializeField]
        private AudioClip[] _clipArray;

        /// <summary>
        /// Gets a random clip from the audio clip array.
        /// </summary>
        [ShowInInspector, ReadOnly, PropertyTooltip("Randomly selected clip from the array.")]
        public AudioClip Clip => _clipArray.Length > 0 ? _clipArray[Random.Range(0, _clipArray.Length)] : null;

        [Range(0f, 1f), PropertyTooltip("Volume level for the audio.")]
        public float Volume = 1f;

        [Range(0.1f, 3f), PropertyTooltip("Pitch level for the audio.")]
        public float Pitch = 1f;

        [PropertyTooltip("Should the audio loop?")]
        public bool Loop = false;
    }
}