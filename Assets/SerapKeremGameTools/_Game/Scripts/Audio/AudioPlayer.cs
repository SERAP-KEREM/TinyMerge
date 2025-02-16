using UnityEngine;

namespace SerapKeremGameTools._Game._AudioSystem
{
    /// <summary>
    /// Manages the audio playback for each AudioPlayer instance.
    /// </summary>
    public class AudioPlayer : MonoBehaviour
    {
        private AudioSource audioSource;

        void Awake()
        {
            // Initialize the AudioSource component
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Plays the given audio clip using PlayOneShot.
        /// </summary>
        /// <param name="clip">Audio clip to play</param>
        /// <param name="volume">Volume of the audio</param>
        public void PlayOneShot(AudioClip clip, float volume)
        {
            audioSource.volume = volume;
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
