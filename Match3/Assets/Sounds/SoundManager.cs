using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Sounds
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;
        [SerializeField] private GameObject _soundPrefab;
        [SerializeField] private AudioMixer _audioMixer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            DontDestroyOnLoad(gameObject);
        }

        public AudioSource PlaySound(AudioClip clip, AudioMixerGroup audioMixerGroup = null, float volume = 1)
        {
            var newSound = Instantiate(_soundPrefab);
            var audio = newSound.GetComponent<AudioSource>();
            if (audioMixerGroup != null) audio.outputAudioMixerGroup = audioMixerGroup;
            audio.volume = volume;
            audio.clip = clip;
            audio.Play();
            return audio;
        }

        public AudioSource PlaySoundRandom(AudioClip[] clips, AudioMixerGroup audioMixerGroup = null, float volume = 1)
        {
            var newSound = Instantiate(_soundPrefab);
            var audio = newSound.GetComponent<AudioSource>();
            var audioClip = clips[Random.Range(0, clips.Length)];
            if (audioMixerGroup != null) audio.outputAudioMixerGroup = audioMixerGroup;
            audio.volume = volume;
            audio.clip = audioClip;
            audio.Play();
            return audio;
        }

        public void OffSound()
        {
            _audioMixer.SetFloat("VolumeMaster", -80);
        }

        public void OnSound()
        {
            _audioMixer.SetFloat("VolumeMaster", 0);
        }

        private void ResetMixer(Scene arg0, LoadSceneMode arg1)
        {
            OnSound();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += ResetMixer;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded += ResetMixer;
        }
    }
}

