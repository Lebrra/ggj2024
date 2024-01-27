using System;
using System.Threading.Tasks;
using Kickstarter.Inputs;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Radio : MonoBehaviour, IInputReceiver, IRadio
{
    [SerializeField] private FloatInput rechargeInput;
    [SerializeField] private float rechargeRate;
    [SerializeField] private float drainRate;
    
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
        DisableRecharge();
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
    public void PlayClip(AudioClip clip)
    {
        if (!radioActive)
            return;
        if (BatteryCharge <= 0)
            return;
        audioSource.clip = clip;
        audioSource.Play();
    }
    #endregion

    private void EnableRecharge()
    {
        recharging = true;
        radioActive = false;
        reverbFilter.enabled = true;
        echoFilter.enabled = true;
        chorusFilter.enabled = true;
        locomotion.SetLocomotionStatus(false);
        RechargeBattery();
    }

    private void DisableRecharge()
    {
        recharging = false;
        radioActive = true;
        reverbFilter.enabled = false;
        echoFilter.enabled = false;
        chorusFilter.enabled = false;
        locomotion.SetLocomotionStatus(true);
        DrainBattery();
    }

    private async void RechargeBattery()
    {
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
            await Task.Delay(timeStep);
        }
    }
}

public interface IRadio
{
    public float BatteryCharge { get; }

    public void PlayClip(AudioClip clip);
}
