using System;
using ildoo; 
using UnityEngine;

namespace ildoo
{
    public enum Soundtype
    {
        Master,
        BGM,
        SFX,
        Size
    }
    public class AudioHelper
    {
    }
    [System.Serializable]
    public struct Sound : IEquatable<Sound>
    {
        public string soundName;
        public Soundtype soundtype;
        [HideInInspector]
        public AudioClip? audioClip;
        public Sound(string soundName, Soundtype soundtype)
        {
            this.soundName = soundName;
            this.soundtype = soundtype;
            this.audioClip = null;
        }
        public Sound(string soundName, Soundtype soundtype, AudioClip audioClip)
        {
            this.soundName = soundName;
            this.soundtype = soundtype;
            this.audioClip = audioClip;
        }
        //=======================================================================
        public bool Equals(Sound other)
        {
            return other.soundName == this.soundName;
        }
        public override int GetHashCode()
        {
            int hash = soundName != null ? soundName.GetHashCode() : 0;
            return hash;
        }
    }
}

