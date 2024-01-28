using System;
using System.Collections;
using System.Collections.Generic;
using BeauRoutine;
using UnityEngine;

public class RadioManager : MonoBehaviour
{
    [SerializeField] 
    float minInterval = 30F;
    [SerializeField]
    float maxInterval = 60F;

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

    Routine radioRoutine;

    private void Start()
    {
        //PlayerProgress.InitializeProgress();
        PlayerProgress.ProgressUpdate += RadioUpdate;
        Initialize();
    }

    public void Initialize()
    {
        int good = UnityEngine.Random.Range(0, voicePacks.Count);
        int bad = good;
        
        while (bad == good) 
            bad = UnityEngine.Random.Range(0, voicePacks.Count);
        
        goodGuy = voicePacks[good];
        badGuy = voicePacks[bad];
        
        currentPercent = 0;
        radioRoutine.Replace(QueueRadio());
    }
    
    void RadioUpdate(int level)
    {
        currentPercent = level;
    }
    
    IEnumerator QueueRadio()
    {
        PlayRadio();
        
        yield return UnityEngine.Random.Range(minInterval, maxInterval);
        
        radioRoutine.Replace(QueueRadio());
    }
    
    void PlayRadio()
    {
        int selection = UnityEngine.Random.Range(1, 101);
        float cumulativeLogic = percentBreakdowns[currentPercent].helpfulHint;
        
        // play helpful hint
        if (selection <= cumulativeLogic)
        {
            Debug.Log($"Play the correct direction clip for the current objective spoken by the good guy");
            var hint = goodGuy.Directionals.GetBlurb(MapLoader.GetCurrentObjective.Invoke().directions.BreakToString());
            
            if (hint.clip)
                AudioManager.PlayClip(hint.clip);
            else 
                Debug.Log(goodGuy + "_" + hint.name);
            return;
        }
        cumulativeLogic += percentBreakdowns[currentPercent].hurtfulHint;
        // play hurtful hint
        if (selection <= cumulativeLogic)
        {
            Debug.Log($"Play a direction clip for an already collected objective or the direction of the big bad by the bad guy");
            bool findBigBad = UnityEngine.Random.Range(1, 101) <= percentBreakdowns[currentPercent].bigBadPercent;
            if (findBigBad)
            {
                // TODO: find big bad and play matching audio
            }
            else
            {
                var hint = badGuy.Directionals.GetBlurb(DirectionUtils.SelectRandomDirection().BreakToString());
                if (hint.clip)
                    AudioManager.PlayClip(hint.clip);
                else 
                    Debug.Log(goodGuy + "_" + hint.name);
            }
            return;
        }
        cumulativeLogic += percentBreakdowns[currentPercent].jokeGood;
        // play good joke
        if (selection <= cumulativeLogic)
        {
            Debug.Log($"Play a joke by the good guy");
            var joke = goodGuy.Jokes.GetRandomBlurb();
            if (joke.clip)
                AudioManager.PlayClip(joke.clip);
            else 
                Debug.Log(goodGuy + "_" + joke.name);
            return;
        }
        cumulativeLogic += percentBreakdowns[currentPercent].jokeBad;
        // play bad joke
        if (selection <= cumulativeLogic)
        {
            Debug.Log($"Play a joke by the bad guy");
            var joke = badGuy.Jokes.GetRandomBlurb();
            if (joke.clip)
                AudioManager.PlayClip(joke.clip);
            else 
                Debug.Log(goodGuy + "_" + joke.name);
            return;
        }
        
        Debug.LogError("[Radio] Error in percentage breakdown - we shouldn't be here!");
    }
}

[Serializable]
public struct RadioPercent
{
    public float helpfulHint;
    public float hurtfulHint;
    public float jokeGood;
    public float jokeBad;
    
    public float bigBadPercent;
    // percent of other bad hint?
}