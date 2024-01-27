using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioManager : MonoBehaviour
{
    [SerializeField] 
    float interval = 30F;

    [SerializeField] 
    bool radioOn = false;
    public bool RadioOn
    {
        get => radioOn;
        set => radioOn = value;
    }
    
    [SerializeField]
    List<AudioVoicePack> voicePacks;
    
    AudioVoicePack goodGuy = null;
    AudioVoicePack badGuy = null;
    
    [SerializeField]
    List<RadioPercent> percentBreakdowns;
    int currentPercent = -1;
    
    void temp()
    {
        Debug.Log(goodGuy.ToString() + badGuy + currentPercent + interval + radioOn);
    }
}

public struct RadioPercent
{
    public float helpfulHint;
    public float hurtfulHint;
    public float jokeGood;
    public float jokeBad;
}