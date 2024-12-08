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

            // BGM AudioSource 설정
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;

            // SFX AudioSource 설정
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        // 사운드 재생
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

        // 특정 타입의 사운드 정지
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

        // 볼륨 조절
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
