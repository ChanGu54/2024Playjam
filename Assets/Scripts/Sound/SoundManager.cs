using UnityEngine;
using System;

namespace PlayJam.Sound
{
    public enum ESoundType
    {
        BGM,
        SFX
    }

    public class SoundManager : MonoBehaviour
    {
        private static SoundManager _instance;
        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.Find("SoundManager")?.GetComponent<SoundManager>();
                    _instance?.Initialize();
                }
                return _instance;
            }
        }

        [SerializeField]
        private AudioClip[] bgmSounds;
        [SerializeField]
        private AudioClip[] sfxSounds;

        private AudioSource bgmSource;
        private AudioSource sfxSource;

        public void Initialize()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // BGM AudioSource ����
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;

            // SFX AudioSource ����
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        // ���� ���
        public void Play(ESoundType type, string soundName, bool isLoop = false)
        {
            AudioSource source = null;
            AudioClip clip = null;

            switch (type)
            {
                case ESoundType.BGM:
                    if (UserDataHelper.Instance.IsBGMOn == false)
                    {
                        return;
                    }

                    clip = Array.Find(bgmSounds, s => s.name == soundName);
                    source = bgmSource;
                    break;
                case ESoundType.SFX:
                    if (UserDataHelper.Instance.IsSFXOn == false)
                    {
                        return;
                    }

                    clip = Array.Find(sfxSounds, s => s.name == soundName);
                    source = sfxSource;
                    break;
            }

            if (clip != null && source != null)
            {
                source.clip = clip;
                source.loop = isLoop;
                source.Play();
            }
            else
            {
                Debug.LogWarning($"Sound {soundName} not found!");
            }
        }

        // Ư�� Ÿ���� ���� ����
        public void Stop(ESoundType type)
        {
            switch (type)
            {
                case ESoundType.BGM:
                    bgmSource.Stop();
                    break;
                case ESoundType.SFX:
                    sfxSource.Stop();
                    break;
            }
        }

        // ���� ����
        public void SetVolume(ESoundType type, float volume)
        {
            volume = Mathf.Clamp01(volume);

            switch (type)
            {
                case ESoundType.BGM:
                    bgmSource.volume = volume;
                    break;
                case ESoundType.SFX:
                    sfxSource.volume = volume;
                    break;
            }
        }
    }
}
