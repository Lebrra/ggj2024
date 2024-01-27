using System;
using System.Collections.Generic;
using Kickstarter.Extensions;
using UnityEngine;

namespace Kickstarter.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class AudioController<TEnum> : MonoBehaviour, Observer.IObserver<TEnum> where TEnum : Enum
    {
        private readonly Dictionary<TEnum, AudioClip> stateClips = new Dictionary<TEnum, AudioClip>();
        private AudioSource audioSource;

        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        protected void InitializeAudioController(AudioClip[] clips)
        {
            stateClips.LoadDictionary(clips);
        }

        public void OnNotify(TEnum argument)
        {
            audioSource.Stop();
            audioSource.clip = stateClips[argument];
            audioSource.Play();
        }
    }
}