using System.Threading.Tasks;
using Kickstarter.Inputs;
using Kickstarter.Observer;
using UnityEngine;

public class Locomotion : Observable, IInputReceiver, ILocomotion
{
    [SerializeField] private Vector2Input movementInput;
    [SerializeField] private float movementSpeed;
    [Space]
    [SerializeField] private FloatInput sprintInput;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float staminaDuration;
    [SerializeField] private float staminaRechargeRate;

    private float stamina;
    public float Stamina
    {
        get => stamina;
        set
        {
            stamina = value;
            NotifyObservers(new StaminaNotification(Stamina, staminaDuration));
        }
    }

    private Rigidbody body;
    private float activeSpeedLimit;
    
    private Vector3 rawInput;
    private bool locomotionActive = true;
    private bool sprinting;
    private bool staminaRecharging;
    
    #region InputHandler
    public void RegisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        movementInput.RegisterInput(OnMovementInputChange, playerIdentifier);
        sprintInput.RegisterInput(OnSprintInputChange, playerIdentifier);
    }

    public void DeregisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        movementInput.DeregisterInput(OnMovementInputChange, playerIdentifier);
        sprintInput.DeregisterInput(OnSprintInputChange, playerIdentifier);
    }

    private void OnMovementInputChange(Vector2 input)
    {
        rawInput = new Vector3(input.x, 0, input.y);
        NotifyObservers(rawInput == Vector3.zero ? PlayerAudio.SoundEffects.Idle : PlayerAudio.SoundEffects.Walking);
    }

    private void OnSprintInputChange(float input)
    {
        if (input == 0)
        {
            activeSpeedLimit = movementSpeed;
            if (!staminaRecharging)
                RefillStamina();
            return;
        }
        if (Stamina > 0)
            DrainStamina();
    }
    #endregion
    
    #region UnityEvents
    private void Awake()
    {
        transform.root.TryGetComponent(out body);
    }

    private void Start()
    {
        stamina = staminaDuration;
        sprinting = false;
        staminaRecharging = true;
        activeSpeedLimit = movementSpeed;
    }

    private void FixedUpdate()
    {
        if (!locomotionActive)
        {
            body.velocity = Vector3.zero;
            return;
        }
        body.velocity = transform.TransformDirection(rawInput) * activeSpeedLimit;
    }

    private void OnDestroy()
    {
        sprinting = false;
        staminaRecharging = false;
    }
    #endregion
    
    #region Locomotion
    public void SetLocomotionStatus(bool toggle)
    {
        locomotionActive = toggle;
    }
    #endregion

    private async void DrainStamina()
    {
        activeSpeedLimit = sprintingSpeed;
        staminaRecharging = false;
        sprinting = true;
        while (sprinting)
        {
            if (stamina <= 0)
            {
                sprinting = false;
                activeSpeedLimit = movementSpeed;
                break;
            }
            int timeStep = (int)(Time.deltaTime * 1000);
            await Task.Delay(timeStep);
            Stamina -= timeStep / 1000.0f;
        }
    }

    private async void RefillStamina()
    {
        sprinting = false;
        staminaRecharging = true;
        while (staminaRecharging)
        {
            if (stamina >= staminaDuration)
            {
                staminaRecharging = false;
                break;
            }
            int timeStep = (int)(Time.deltaTime * 1000);
            await Task.Delay(timeStep);
            Stamina += timeStep / 1000.0f;
        }
    }
    
    #region Notifications
    public struct StaminaNotification : INotification
    {
        public float Stamina { get; }
        public float MaxStamina { get; }
        
        public StaminaNotification(float stamina, float maxStamina)
        {
            Stamina = stamina;
            MaxStamina = maxStamina;
        }
    }
    #endregion
}

public interface ILocomotion
{
    public void SetLocomotionStatus(bool toggle);
}