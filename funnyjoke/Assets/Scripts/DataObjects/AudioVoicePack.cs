using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "VoicePack", order = 100)]
public class AudioVoicePack : ScriptableObject
{
    [FormerlySerializedAs("goodIntro")] [SerializeField]
    AudioClip startIntro;
    [FormerlySerializedAs("badIntro")] [SerializeField]
    AudioClip endIntro;
    
    [Space, SerializeField]
    AudioBlurbs directionalBlurbs;
    [SerializeField]
    AudioBlurbs jokeBlurbs;
    [SerializeField, Tooltip("'Hide!' 'Look out!' 'Run!'")]
    AudioBlurbs helpfulBlurbs;
    [SerializeField, Tooltip("Additional random lines for the big bad to say that aren't helpful")]
    AudioBlurbs hurtfulBlurbs;
    
    public AudioClip StartIntro => startIntro;
    public AudioClip LastIntro => endIntro;
    
    public AudioBlurbs Directionals => directionalBlurbs;
    public AudioBlurbs Jokes => jokeBlurbs;
    public AudioBlurbs Helpfuls => helpfulBlurbs;
    public AudioBlurbs Hurtfuls => hurtfulBlurbs;
}
