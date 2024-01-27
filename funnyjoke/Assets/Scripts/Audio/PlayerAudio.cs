using System.Collections.Generic;
using Kickstarter.Audio;
using Kickstarter.Extensions;
using UnityEngine;

public class PlayerAudio : AudioController<PlayerAudio.SoundEffects>
{
    [SerializeField] private AudioClip[] footsteps;
    [SerializeField] private AudioClip[] idles;

    [SerializeField, EnumData(typeof(SoundEffects)), Range(-3, 3),]
    private float[] pitches;

    private readonly Dictionary<SoundEffects, float> audioPitches = new Dictionary<SoundEffects, float>();
    
    private void Start()
    {
        audioPitches.LoadDictionary(pitches);
        transform.root.GetComponentInChildren<Locomotion>().AddObserver(this);
        InitializeAudioController(footsteps, idles);
    }

    public override void OnNotify(SoundEffects argument)
    {
        audioSource.loop = argument == SoundEffects.Walking;
        audioSource.pitch = audioPitches[argument];
        base.OnNotify(argument);
    }

    public enum SoundEffects
    {
        Walking,
        Idle,
    }
}
