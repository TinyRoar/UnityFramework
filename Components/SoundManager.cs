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

        // Variables
        private bool _musicPlaying = true;

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
        void Start()
        {
            // Check from DataManagement if Sound key is set, if not enable Sound else load Data
            if(DataManagement.Instance.CheckItem(GameConfig.KeySoundAllowmusic) == false)
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
            GameObject go = new GameObject("Audio: " + clip.name);
            go.transform.parent = this.transform;

            //Create the source
            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            if (delay == 0)
                source.Play();
            else
                source.PlayDelayed(delay);
            source.loop = loop;

            // Destroy after playing
            if (loop == false || deleteAfterSec != 0)
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

            int index = -1;
            for (int i = 0; i < AudioList.Count; i++)
            {
                if (AudioList[i] == null)
                    continue;
                if (AudioList[i].name == name)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
            {
                if(GameConfig.Instance.Debug && name != "")
                    Debug.LogWarning("Sound '" + name + "' not found :'(");
                return null;
            }

            return Play(AudioList[index], volume, 1f, loop, delay, deleteAfterSec);

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
                    AllowSoundeffect = !AllowSoundeffect;
                    break;
            }
        }

        private void DestroyMusicObjects()
        {
            foreach (var item in BgMusic)
            {
                var obj = this.transform.FindChild("Audio: " + item);
                if (obj != null)
                    Destroy(obj.gameObject);
            }
        }

        public void Stop(AudioSource audioSource)
        {
            Destroy(audioSource.gameObject);
        }

        // pause sounds, example: while playing ads
        internal void StopMusic()
        {
            if (!AllowMusic || !_musicPlaying)
                return;

            _musicPlaying = false;

            DestroyMusicObjects();
            if (OnMusicStop != null)
                OnMusicStop();
        }

        // opposite of PauseSounds
        internal void RestoreSounds()
        {
            if (_musicPlaying)
                return;

            _musicPlaying = true;

            if (OnMusicPlay != null)
                OnMusicPlay();
        }

    }

}