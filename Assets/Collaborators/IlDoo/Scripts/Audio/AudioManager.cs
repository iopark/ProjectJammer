using System;
using ildoo; 
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace ildoo
{
    public class AudioManager : MonoBehaviour
    {
        Sound mainThemeSong;
        public AudioMixer audioMixer;
        AudioSource bgmSource;
        AudioSource sfxSource;
        [SerializeField] AudioMixerGroup[] audioMixerGroup;
        HashSet<Sound> audioList = new HashSet<Sound>();

        #region getset for volumes
        public float CurrentMasterVolume
        {
            get
            {
                float volume;
                audioMixer.GetFloat("Master", out volume);
                return volume;
            }
        }

        public float CurrentBGMVolume
        {
            get
            {
                float volume;
                audioMixer.GetFloat("BGM", out volume);
                return volume;
            }
        }

        public float CurrentSFXVolume
        {
            get
            {
                float volume;
                audioMixer.GetFloat("SFX", out volume);
                return volume;
            }
        }
        #endregion

        private void Awake()
        {
            audioMixer = Resources.Load<AudioMixer>("Sound/GameMasterMixer");
            audioMixerGroup = audioMixer.FindMatchingGroups("Master");
            bgmSource = this.AddComponent<AudioSource>();
            bgmSource.outputAudioMixerGroup = audioMixerGroup[(int)Soundtype.BGM];
            sfxSource = this.AddComponent<AudioSource>();
            sfxSource.outputAudioMixerGroup = audioMixerGroup[(int)Soundtype.SFX];
            //GameManager.Instance.GameSetup += ResetAllMusic; 
            SetMasterVolume(-10f);
            //mainThemeSong = new Sound("IntroductionBGM", Soundtype.BGM);
            //RegisterSound(mainThemeSong); 
            //PlayBGM(mainThemeSong); 
        }
        public void PlaySound(Sound sound)
        {
            switch (sound.soundtype)
            {
                case Soundtype.BGM:
                    PlayBGM(sound); break;
                case Soundtype.SFX:
                    PlayEffect(sound); break;
            }
        }
        public void PlayEffect(Sound sound)
        {
            if (!audioList.Contains(sound))
            {
                RegisterSound(sound);
            }
            Sound sfxSound;
            audioList.TryGetValue(sound, out sfxSound);
            sfxSource.PlayOneShot(sfxSound.audioClip);
        }
        private void RegisterSound(Sound sound)
        {
            string audioKey = $"Sound/{sound.soundName}";
            AudioClip registeringClip = Resources.Load<AudioClip>(audioKey);
            Sound registeringSound = new Sound(sound.soundName, sound.soundtype, registeringClip);
            audioList.Add(registeringSound);
        }
        public void PlayBGM(Sound sound)
        {
            if (!audioList.Contains(sound))
            {
                RegisterSound(sound);
            }
            Sound sfxSound;
            audioList.TryGetValue(sound, out sfxSound);
            bgmSource.clip = sfxSound.audioClip;

            bgmSource.Play();
            bgmSource.loop = true;
        }
        public void ResetAllMusic()
        {
            sfxSource.Stop();
            bgmSource.Stop();
        }
        public AudioClip GetAudio(string soundName)
        {
            AudioClip audioClip = null;
            return audioClip;
        }
        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("Master", volume);
        }
        public void SetSFXVolume(float volume)
        {
            audioMixer.SetFloat("SFX", volume);
        }
        public void SetBGMVolume(float volume)
        {
            audioMixer.SetFloat("BGM", volume);
        }
    }
}