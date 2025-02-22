using UnityEngine;

namespace _Main._Data
{
    /// <summary>
    /// Contains all the properties related to an audio clip.
    /// </summary>
 
    [System.Serializable]
    public class Audio
    {
        [Header("Audio Settings")]
        [Tooltip("The name of the audio.")]
        public string Name;

        [Tooltip("Array of audio clips that can be played.")]
        [SerializeField]
        private AudioClip[] _clipArray;

        /// <summary>
        /// Gets a random clip from the audio clip array.
        /// </summary>
        public AudioClip Clip => _clipArray[Random.Range(0, _clipArray.Length)];

        [Range(0f, 1f)]
        [Tooltip("Volume level for the audio.")]
        public float Volume = 1f;

        [Range(0.1f, 3f)]
        [Tooltip("Pitch level for the audio.")]
        public float Pitch = 1f;

        [Tooltip("Should the audio loop?")]
        public bool Loop = false;
    }
}
