using System;
using System.Collections;
using System.Collections.Generic;
using BeauRoutine;
using UnityEngine;

public class RadioManager : MonoBehaviour
{
    // TODO: merge with Radio.cs 
    
    [SerializeField]
    bool DEBUG = false;
    
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
    
    bool lastPlayedHelpfulDirection;

    Routine radioRoutine;

    private void Start()
    {
        if (DEBUG) PlayerProgress.InitializeProgress();
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
        
        lastPlayedHelpfulDirection = UnityEngine.Random.Range(0, 10) % 2 == 0;
        
        currentPercent = 0;
        // TODO: intro before radio
        if (DEBUG) radioRoutine.Replace(StartRadio());
    }
    
    void RadioUpdate(int level)
    {
        currentPercent = level;
        lastPlayedHelpfulDirection = UnityEngine.Random.Range(0, 10) % 2 == 0;
        
        // TODO: maybe cut out radio and play a specific line or something
    }
    
    IEnumerator StartRadio()
    {
        PlayJoke();
        yield return maxInterval;
        radioRoutine.Replace(QueueRadio());
    }
    
    IEnumerator QueueRadio()
    {
        PlayRadio();
        yield return UnityEngine.Random.Range(minInterval, maxInterval);
        radioRoutine.Replace(QueueRadio());
    }
    
    void PlayRadio(int forceType = -1)
    {
        int selection = forceType;
        if (forceType is < 0 or > 100) selection = UnityEngine.Random.Range(1, 101);
        float cumulativeLogic = percentBreakdowns[currentPercent].helpfulHint;
        
        // play helpful hint
        if (selection <= cumulativeLogic)
        {
            Debug.Log($"Play the correct direction clip for the current objective spoken by the good guy");
            VOBlurb hint;
            if (lastPlayedHelpfulDirection) 
                hint = goodGuy.Directionals.GetBlurb(MapLoader.GetCurrentObjective.Invoke().directions.BreakToString());
            else 
                hint = goodGuy.Helpfuls.GetBlurb(PlayerProgress.Current.ToString().ToLower());
            lastPlayedHelpfulDirection = !lastPlayedHelpfulDirection;
            
            if (hint.clip)
                AudioManager.PlayClip(hint.clip);
            else 
                Debug.Log(goodGuy.name + "_" + hint.name);
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
                Debug.LogError("PLAY DIRECTION TO BIG BAD NOW");
            }
            else
            {
                var hint = badGuy.Directionals.GetBlurb(DirectionUtils.SelectRandomDirection().BreakToString());
                if (hint.clip)
                    AudioManager.PlayClip(hint.clip);
                else 
                    Debug.Log(badGuy.name + "_" + hint.name);
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
                Debug.Log(goodGuy.name + "_" + joke.name);
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
                Debug.Log(badGuy.name + "_" + joke.name);
            return;
        }
        
        Debug.LogError("[Radio] Error in percentage breakdown - we shouldn't be here!");
    }
    
    public void PlayJoke()
    {
        // assuming it could be from either guy
        PlayRadio(UnityEngine.Random.Range(100 - percentBreakdowns[currentPercent].jokeGood, 101));
    }
}

[Serializable]
public struct RadioPercent
{
    public int helpfulHint;
    public int hurtfulHint;
    public int jokeGood;
    public int jokeBad;
    [Space]
    public int bigBadPercent;
    // percent of other bad hint?
}