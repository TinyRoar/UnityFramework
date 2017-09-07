using UnityEngine;
using System.Collections.Generic;
using System;

namespace TinyRoar.Framework
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        public event Action OnMusicStop;
        public event Action OnMusicPlay;

        public enum SoundType
        {
            None,
            Music,
            Soundeffect
        }

        // Serializable
        public List<AudioClip> AudioList;
        public List<string> BgMusic; // a list of all possible background musics to stop if music should stop
        public List<AudioSource> ExternalAudioSources; // a list of all AudioSource componenets created by an artist

        // Variables
        private bool _musicPlaying = true;
        private float _overridePitch = 0f;

        // Properties
        private bool _allowMusic = true;
        public bool AllowMusic
        {
            get
            {
                return this._allowMusic;
            }
            private set
            {
                this._allowMusic = value;
                DataManagement.Instance.Set(GameConfig.KeySoundAllowmusic, this._allowMusic);
            }
        }

        private bool _allowSoundeffect = true;
        public bool AllowSoundeffect
        {
            get
            {
                return this._allowSoundeffect;
            }
            private set
            {
                this._allowSoundeffect = value;
                DataManagement.Instance.Set(GameConfig.KeySoundAlloweffects, this._allowSoundeffect);
            }
        }

        // Initialize
        protected override void Awake()
        {
            base.Awake();

            // Check from DataManagement if Sound key is set, if not enable Sound else load Data
            if (DataManagement.Instance.CheckItem(GameConfig.KeySoundAllowmusic) == false)
                this._allowMusic = true;
            else
                this._allowMusic = DataManagement.Instance.Get(GameConfig.KeySoundAllowmusic).Bool;

            if (DataManagement.Instance.CheckItem(GameConfig.KeySoundAlloweffects) == false)
                this._allowSoundeffect = true;
            else
                this._allowSoundeffect = DataManagement.Instance.Get(GameConfig.KeySoundAlloweffects).Bool;
        }

        private AudioSource Play(AudioClip clip, float volume, float pitch, bool loop = false, float delay = 0, float deleteAfterSec = 0)
        {
            //Create an empty game object
            var go = new GameObject("Audio: " + clip.name);
            go.transform.parent = this.transform;

            //Create the source
            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;

            if (Math.Abs(_overridePitch) > 0.01f)
                source.pitch = _overridePitch;
            else
                source.pitch = pitch;

            if (Math.Abs(delay) < 0.01f)
                source.Play();
            else
                source.PlayDelayed(delay);

            source.loop = loop;

            // Destroy after playing
            if (loop == false || Math.Abs(deleteAfterSec) > 0.01f)
            {
                Destroy(go, clip.length + delay + deleteAfterSec);
            }

            return source;
        }

        public AudioSource Play(string name, SoundType type, bool loop = false, float volume = 1.0f, float delay = 0, float deleteAfterSec = 0)
        {
            // Check if Music / Sounds are not disabled by LayerManager
            switch (type)
            {
                case SoundType.Music:
                    if (AllowMusic == false)
                        return null;
                    break;
                case SoundType.Soundeffect:
                    if (AllowSoundeffect == false)
                        return null;
                    break;
            }

            if (GameConfig.Instance.Debug && name != "")
                Debug.Log("Playing sound '" + name + "' ");

            var audioClip = GetAudioClip(name);

            return Play(audioClip, volume, 1f, loop, delay, deleteAfterSec);
        }

        private AudioClip GetAudioClip(string name)
        {
            for (var i = 0; i < AudioList.Count; i++)
            {
                if (AudioList[i] == null)
                    continue;
                if (AudioList[i].name != name)
                    continue;
                return AudioList[i];
            }
            return null;
        }

        internal void EnableDisableSound(SoundType SoundType)
        {
            switch (SoundType)
            {
                case SoundManager.SoundType.Music:
                    AllowMusic = !AllowMusic;

                    if (SoundType == SoundManager.SoundType.Music)
                    {
                        if (AllowMusic == false)
                        {
                            DestroyMusicObjects();
                            if (OnMusicStop != null)
                                OnMusicStop();
                        }
                        else
                        {
                            if (OnMusicPlay != null)
                                OnMusicPlay();
                        }
                    }

                    break;
                case SoundManager.SoundType.Soundeffect:
                    MuteExternalAudioSource(AllowSoundeffect);
                    AllowSoundeffect = !AllowSoundeffect;
                    break;
            }
        }

        private void DestroyMusicObjects()
        {
            for (var i = 0; i < BgMusic.Count; i++)
            {
                var item = BgMusic[i];
                var obj = this.transform.Find("Audio: " + item);
                if (obj != null)
                    Destroy(obj.gameObject);
            }
        }

        private void MuteExternalAudioSource(bool doMute)
        {
            for (var i = 0; i < ExternalAudioSources.Count; i++)
            {
                var audioSource = ExternalAudioSources[i];
                audioSource.mute = doMute;
            }
        }

        public void Stop(AudioSource audioSource)
        {
            Destroy(audioSource.gameObject);
        }

        /// <summary>
        /// Override Pitch for all AudioSources
        /// </summary>
        /// <param name="pitch"></param>
        public void OverridePitch(float pitch)
        {
            var audioSources = GetComponentsInChildren<AudioSource>(true);

            foreach (var item in audioSources)
            {
                item.pitch = pitch;
                _overridePitch = pitch;
            }
        }

        // pause sounds, example: while playing ads
        public void StopMusic()
        {
            if (!AllowMusic || !_musicPlaying)
                return;

            _musicPlaying = false;

            DestroyMusicObjects();
            MuteExternalAudioSource(true);
            if (OnMusicStop != null)
                OnMusicStop();
        }

        // opposite of PauseSounds
        public void RestoreSounds()
        {
            if (_musicPlaying)
                return;

            _musicPlaying = true;

            if (OnMusicPlay != null)
                OnMusicPlay();
            MuteExternalAudioSource(false);
        }

        public void Mute(bool isMute)
        {
            AudioListener.volume = isMute ? 0 : 1;
        }
    }

}