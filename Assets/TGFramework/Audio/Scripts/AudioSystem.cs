using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;

namespace TGFramework
{
    public class AudioSystem : MonoBehaviour
    {
        public static AudioSystem Instance;

        [Header("Audio Settings")]
        [SerializeField] private AudioClipDataModel _audioDatabase;
        [SerializeField] private AudioSource _musicSource;
        [Range(0f, 1f)] [SerializeField] private float _musicVolume = 1f;
        [Range(0f, 1f)] [SerializeField] private float _sfxVolume = 1f;

        [Header("SFX Pooling")]
        [SerializeField] private LeanGameObjectPool _sfxPool;

        private Dictionary<string, Dictionary<string, AudioClip>> _audioCollections;
        
        public float MusicVolume => _musicVolume;
        public float SfxVolume => _sfxVolume;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (_audioDatabase == null)
            {
                _audioDatabase = Resources.Load<AudioClipDataModel>("Audio/AudioDatabase");
            }

            BuildAudioCollections();

            _musicSource.loop = true;
            _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            _sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
            ApplyVolume();
        }

        private void BuildAudioCollections()
        {
            _audioCollections = new Dictionary<string, Dictionary<string, AudioClip>>();

            if (_audioDatabase == null)
                return;

            if (_audioDatabase.Collections != null && _audioDatabase.Collections.Count > 0)
            {
                foreach (var collection in _audioDatabase.Collections)
                {
                    if (collection == null || string.IsNullOrEmpty(collection.CollectionName))
                        continue;

                    if (!_audioCollections.TryGetValue(collection.CollectionName, out var clipsByKey))
                    {
                        clipsByKey = new Dictionary<string, AudioClip>();
                        _audioCollections.Add(collection.CollectionName, clipsByKey);
                    }

                    if (collection.Clips == null)
                        continue;

                    foreach (var entry in collection.Clips)
                    {
                        if (entry == null || string.IsNullOrEmpty(entry.Key) || entry.Clip == null)
                            continue;

                        if (clipsByKey.ContainsKey(entry.Key))
                        {
                            Debug.LogWarning($"AudioSystem: Duplicate key '{entry.Key}' in collection '{collection.CollectionName}'.");
                            continue;
                        }

                        clipsByKey.Add(entry.Key, entry.Clip);
                    }
                }
            }
        }

        private void ApplyVolume()
        {
            if (_musicSource != null)
            {
                _musicSource.volume = _musicVolume;
            }
        }

        /// <summary>
        /// Sets the global music volume.
        /// </summary>
        /// <param name="volume">Volume between 0 and 1.</param>
        public static void SetMusicVolume(float volume)
        {
            if (Instance == null) return;
            Instance._musicVolume = Mathf.Clamp01(volume);
            Instance._musicSource.volume = Instance._musicVolume;
            
            PlayerPrefs.SetFloat("MusicVolume", Instance._musicVolume);
        }

        /// <summary>
        /// Sets the global sound effects volume.
        /// </summary>
        /// <param name="volume">Volume between 0 and 1.</param>
        public static void SetSFXVolume(float volume)
        {
            if (Instance == null) return;
            Instance._sfxVolume = Mathf.Clamp01(volume);
            
            PlayerPrefs.SetFloat("SfxVolume", Instance._sfxVolume);
        }

        /// <summary>
        /// Plays background music from the given collection and key.
        /// </summary>
        /// <param name="collection">Name of the collection that contains the audio clip.</param>
        /// <param name="key">Key of the audio clip in the database.</param>
        /// <param name="fadeDuration">Fade-in duration in seconds. If 0, the music starts immediately.</param>
        public static void PlayMusic(string collection, string key, float fadeDuration = 0f)
        {
            if (Instance == null)
                return;

            if (!Instance.TryGetClip(collection, key, out var clip))
                return;

            Instance.StopAllCoroutines();
            if (fadeDuration > 0f)
            {
                Instance.StartCoroutine(Instance.FadeMusic(clip, fadeDuration));
            }
            else
            {
                Instance._musicSource.clip = clip;
                Instance._musicSource.Play();
            }
        }

        /// <summary>
        /// Stops the currently playing music with an optional fade-out.
        /// </summary>
        /// <param name="fadeDuration">Fade-out duration in seconds. If 0, the music stops immediately.</param>
        public static void StopMusic(float fadeDuration = 0f)
        {
            if (Instance == null || Instance._musicSource == null) return;

            Instance.StopAllCoroutines();
            if (fadeDuration > 0f)
            {
                Instance.StartCoroutine(Instance.FadeOutMusic(fadeDuration));
            }
            else
            {
                Instance._musicSource.Stop();
                Instance._musicSource.clip = null;
            }
        }

        private IEnumerator FadeMusic(AudioClip newClip, float duration)
        {
            float startVolume = _musicSource.volume;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _musicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                yield return null;
            }
            _musicSource.Stop();
            _musicSource.volume = 0f;

            _musicSource.clip = newClip;
            _musicSource.Play();
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _musicSource.volume = Mathf.Lerp(0f, _musicVolume, t / duration);
                yield return null;
            }
            _musicSource.volume = _musicVolume;
        }

        private IEnumerator FadeOutMusic(float duration)
        {
            float startVolume = _musicSource.volume;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _musicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                yield return null;
            }
            _musicSource.Stop();
            _musicSource.clip = null;
            _musicSource.volume = _musicVolume;
        }

        /// <summary>
        /// Plays a sound effect from the given collection and key.
        /// </summary>
        /// <param name="collection">Name of the collection that contains the audio clip.</param>
        /// <param name="key">Key of the audio clip in the database.</param>
        public static void PlaySFX(string collection, string key)
        {
            if (Instance == null)
                return;

            if (!Instance.TryGetClip(collection, key, out var clip))
                return;

            Instance.PlaySFXFromPool(clip);
        }

        /// <summary>
        /// Plays a random sound effect from the given collection and keys.
        /// </summary>
        /// <param name="collection">Name of the collection that contains the audio clips.</param>
        /// <param name="keys">Array of audio keys to randomly choose from.</param>
        public static void PlayRandomSFX(string collection, params string[] keys)
        {
            if (Instance == null || keys == null || keys.Length == 0)
                return;

            if (Instance._audioCollections == null || !Instance._audioCollections.TryGetValue(collection, out var clipsByKey))
                return;

            var validKeys = new List<string>();
            foreach (var key in keys)
            {
                if (clipsByKey.ContainsKey(key))
                {
                    validKeys.Add(key);
                }
            }

            if (validKeys.Count == 0)
                return;

            string randomKey = validKeys[Random.Range(0, validKeys.Count)];
            if (!clipsByKey.TryGetValue(randomKey, out var clip) || clip == null)
                return;

            Instance.PlaySFXFromPool(clip);
        }

        private void PlaySFXFromPool(AudioClip clip)
        {
            GameObject audioSourceObject = _sfxPool.Spawn(transform);
            AudioSource audioSource = audioSourceObject.GetComponent<AudioSource>();
            
            if (audioSource == null)
            {
                Debug.LogError("AudioSystem: Spawned GameObject doesn't have AudioSource component!");
                LeanPool.Despawn(audioSourceObject);
                return;
            }

            audioSource.clip = clip;
            audioSource.volume = _sfxVolume;
            audioSource.Play();

            StartCoroutine(DespawnAfterPlay(audioSourceObject, clip.length));
        }

        private IEnumerator DespawnAfterPlay(GameObject audioSourceObject, float duration)
        {
            yield return new WaitForSeconds(duration);
            
            if (audioSourceObject != null)
            {
                AudioSource audioSource = audioSourceObject.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Stop();
                    audioSource.clip = null;
                }
                
                _sfxPool.Despawn(audioSourceObject);
            }
        }
        
        /// <summary>
        /// Plays multiple sound effects sequentially.
        /// </summary>
        /// <param name="collection">Name of the collection that contains the audio clips.</param>
        /// <param name="keys">Array of audio keys to play in order.</param>
        public static void PlaySequentialSFX(string collection, params string[] keys)
        {
            if (Instance == null)
                return;

            Instance.StartCoroutine(Instance.PlaySequentialSFXCoroutine(collection, keys));
        }

        private IEnumerator PlaySequentialSFXCoroutine(string collection, string[] keys)
        {
            foreach (var key in keys)
            {
                if (!TryGetClipStatic(collection, key, out var clip))
                    continue;

                Instance.PlaySFXFromPool(clip);

                yield return new WaitForSeconds(clip.length);
            }
        }

        private bool TryGetClip(string collection, string key, out AudioClip clip)
        {
            clip = null;

            if (string.IsNullOrEmpty(collection) || string.IsNullOrEmpty(key))
                return false;

            if (_audioCollections == null)
                return false;

            if (!_audioCollections.TryGetValue(collection, out var clipsByKey))
                return false;

            if (!clipsByKey.TryGetValue(key, out clip))
                return false;

            return clip != null;
        }

        private static bool TryGetClipStatic(string collection, string key, out AudioClip clip)
        {
            clip = null;

            if (Instance == null)
                return false;

            return Instance.TryGetClip(collection, key, out clip);
        }

    }
}
