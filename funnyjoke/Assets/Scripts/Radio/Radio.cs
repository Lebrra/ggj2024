using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kickstarter.Inputs;
using UnityEngine;
using BeauRoutine;

[RequireComponent(typeof(AudioSource))]
public class Radio : MonoBehaviour, IInputReceiver, IRadio
{
    [SerializeField]
    bool DEBUG = false;
    
    [Header("Data"), SerializeField]
    AudioClip intro1;
    [SerializeField]
    AudioClip intro2;
    [SerializeField]
    float betweenIntroClipsTime = 1F;
    
    [SerializeField]
    List<AudioVoicePack> voicePacks;
    
    AudioVoicePack goodGuy = null;
    AudioVoicePack badGuy = null;
    
    [Header("Playing"), SerializeField] 
    float minInterval = 30F;
    [SerializeField]
    float maxInterval = 60F;

    public static bool PickedUpRadio = false;
    
    [SerializeField]
    List<RadioPercent> percentBreakdowns;
    int currentPercent = -1;
    
    bool lastPlayedHelpfulDirection;

    Routine radioRoutine;
    
    [Header("Recharge")]
    [SerializeField] private FloatInput rechargeInput;
    [SerializeField] private float rechargeRate;
    [SerializeField] private float drainRate;
    [SerializeField] float batteryMax = 50F;
    
    bool volumeOn = true;
    Routine volumeRoutine;
    
    public float BatteryCharge { get; private set; }

    private AudioSource audioSource;
    private AudioReverbFilter reverbFilter;
    private AudioEchoFilter echoFilter;
    private AudioChorusFilter chorusFilter;

    private ILocomotion locomotion;

    private bool recharging;
    private bool radioActive;
    
    #region UnityEvents
    private void Awake()
    {
        TryGetComponent(out audioSource);
        TryGetComponent(out reverbFilter);
        TryGetComponent(out echoFilter);
        TryGetComponent(out chorusFilter);
        locomotion = transform.root.GetComponentInChildren<ILocomotion>();
    }

    private void Start()
    {
        BatteryCharge = batteryMax;
        volumeOn = true;
        DisableRecharge();
        
        if (DEBUG) PlayerProgress.InitializeProgress();
        PlayerProgress.ProgressUpdate += RadioUpdate;
        //Initialize();
    }

    private void OnDestroy()
    {
        recharging = false;
        radioActive = false;
    }
    #endregion
    
    #region InputHandler
    public void RegisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        rechargeInput.RegisterInput(OnRechargeInputChange, playerIdentifier);
    }

    public void DeregisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        rechargeInput.DeregisterInput(OnRechargeInputChange, playerIdentifier);
    }
    
    private void OnRechargeInputChange(float input)
    {
        Action action = input == 1 ? EnableRecharge : DisableRecharge;
        action();
    }
    #endregion
    
    #region Radio
    
    public void Initialize()
    {
        PickedUpRadio = false;
        
        int good = UnityEngine.Random.Range(0, voicePacks.Count);
        int bad = good;
        
        while (bad == good) 
            bad = UnityEngine.Random.Range(0, voicePacks.Count);
        
        goodGuy = voicePacks[good];
        badGuy = voicePacks[bad];
        
        lastPlayedHelpfulDirection = UnityEngine.Random.Range(0, 10) % 2 == 0;
        
        currentPercent = 0;
        if (DEBUG) radioRoutine.Replace(StartRadio());
        else radioRoutine.Replace(StartIntro());
    }
    
    public void PlayClip(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("No clip to play!");
            return;
        }
        audioSource.clip = clip;
        audioSource.Play();
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
        radioRoutine.Replace(LoopRadio());
    }
    
    IEnumerator LoopRadio()
    {
        PlayRadio();
        yield return UnityEngine.Random.Range(minInterval, maxInterval);
        radioRoutine.Replace(LoopRadio());
    }
    
    // TODO: radio off when battery is dead & on when not
    
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
                PlayClip(hint.clip);
            else 
                Debug.LogWarning(goodGuy.name + "_" + hint.name);
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
                    PlayClip(hint.clip);
                else 
                    Debug.LogWarning(badGuy.name + "_" + hint.name);
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
                PlayClip(joke.clip);
            else 
                Debug.LogWarning(goodGuy.name + "_" + joke.name);
            return;
        }
        cumulativeLogic += percentBreakdowns[currentPercent].jokeBad;
        // play bad joke
        if (selection <= cumulativeLogic)
        {
            Debug.Log($"Play a joke by the bad guy");
            var joke = badGuy.Jokes.GetRandomBlurb();
            if (joke.clip)
                PlayClip(joke.clip);
            else 
                Debug.LogWarning(badGuy.name + "_" + joke.name);
            return;
        }
        
        Debug.LogError("[Radio] Error in percentage breakdown - we shouldn't be here!");
    }
    
    public void PlayJoke()
    {
        // assuming it could be from either guy
        PlayRadio(UnityEngine.Random.Range(100 - percentBreakdowns[currentPercent].jokeGood, 101));
    }
    
    #endregion

    #region Intro

    IEnumerator StartIntro()
    {
        PlayClip(intro1);
        yield return intro1 ? intro1.length + betweenIntroClipsTime : betweenIntroClipsTime;
        yield return FinishIntro();
    }

    IEnumerator FinishIntro()
    {
        // TODO: on radio picked up -> Radio.PickedUpRadio = true;
        yield return new WaitUntil(() => PickedUpRadio);
        
        PlayClip(intro2);
        yield return intro2 ? intro2.length + betweenIntroClipsTime : betweenIntroClipsTime;
        
        var clips = new AudioClip[2];
        if (UnityEngine.Random.Range(0, 10) % 2 == 0)
        {
            clips[0] = goodGuy.StartIntro;
            clips[1] = badGuy.LastIntro;
        }
        else
        {
            clips[0] = badGuy.StartIntro;
            clips[1] = goodGuy.LastIntro;
        }
        foreach (var clip in clips)
        {
            PlayClip(clip);
            yield return clip ? clip.length + betweenIntroClipsTime : betweenIntroClipsTime;
        }
        
        yield return minInterval;
        radioRoutine.Replace(StartRadio());
    }

    #endregion

    #region Battery
    
    private void EnableRecharge()
    {
        recharging = true;
        radioActive = false;
        reverbFilter.enabled = true;
        echoFilter.enabled = true;
        chorusFilter.enabled = true;
        locomotion?.SetLocomotionStatus(false);
        RechargeBattery();
    }

    private void DisableRecharge()
    {
        recharging = false;
        radioActive = true;
        reverbFilter.enabled = false;
        echoFilter.enabled = false;
        chorusFilter.enabled = false;
        locomotion?.SetLocomotionStatus(true);
        DrainBattery();
    }

    private async void RechargeBattery()
    {
        if (!volumeOn) 
            volumeRoutine.Replace(LerpVolume(true));
        while (recharging)
        {
            int timeStep = (int)(Time.deltaTime * 1000);
            BatteryCharge += rechargeRate * Time.deltaTime;
            await Task.Delay(timeStep);
        }
    }

    private async void DrainBattery()
    {
        while (radioActive)
        {
            int timeStep = (int)(Time.deltaTime * 1000);
            BatteryCharge -= drainRate * Time.deltaTime;
            if (BatteryCharge <= 0F && volumeOn)
                volumeRoutine.Replace(LerpVolume(false));
            await Task.Delay(timeStep);
        }
    }
    
    IEnumerator LerpVolume(bool toOn)
    {
        volumeOn = toOn;
        yield return audioSource.VolumeTo(toOn ? 1F : 0F, 1F);
    }
    
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) PickedUpRadio = true;
    }
}

public interface IRadio
{
    public float BatteryCharge { get; }

    public void PlayClip(AudioClip clip);
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