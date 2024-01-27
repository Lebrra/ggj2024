using System;
using System.Collections.Generic;
using Kickstarter.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Kickstarter.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class AudioController<TEnum> : MonoBehaviour, Observer.IObserver<TEnum> where TEnum : Enum
    {
        private readonly Dictionary<TEnum, AudioClip[]> stateClips = new Dictionary<TEnum, AudioClip[]>();
        protected AudioSource audioSource;

        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        protected void InitializeAudioController(params AudioClip[][] clips)
        {
            stateClips.LoadDictionary(clips);
        }

        public virtual void OnNotify(TEnum argument)
        {
            audioSource.Stop();
            audioSource.clip = stateClips[argument][Random.Range(0, stateClips[argument].Length)];
            audioSource.Play();
        }
    }
}