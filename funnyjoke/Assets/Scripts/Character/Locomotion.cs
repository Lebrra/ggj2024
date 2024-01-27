using Kickstarter.Inputs;
using Kickstarter.Observer;
using UnityEngine;

public class Locomotion : Observable, IInputReceiver, ILocomotion
{
    [SerializeField] private Vector2Input movementInput;
    [SerializeField] private float movementSpeed;

    private Rigidbody body;
    
    private Vector3 rawInput;
    private bool locomotionActive = true;
    
    #region InputHandler
    public void RegisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        movementInput.RegisterInput(OnMovementInputChange, playerIdentifier);
    }

    public void DeregisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        movementInput.DeregisterInput(OnMovementInputChange, playerIdentifier);
    }

    private void OnMovementInputChange(Vector2 input)
    {
        rawInput = new Vector3(input.x, 0, input.y);
        NotifyObservers(rawInput == Vector3.zero ? PlayerAudio.SoundEffects.Idle : PlayerAudio.SoundEffects.Walking);
    }
    #endregion
    
    #region UnityEvents
    private void Awake()
    {
        transform.root.TryGetComponent(out body);
    }
    
    private void FixedUpdate()
    {
        if (!locomotionActive)
        {
            body.velocity = Vector3.zero;
            return;
        }
        body.velocity = transform.TransformDirection(rawInput) * movementSpeed;
    }
    #endregion
    
    #region Locomotion
    public void SetLocomotionStatus(bool toggle)
    {
        locomotionActive = toggle;
    }
    #endregion
}

public interface ILocomotion
{
    public void SetLocomotionStatus(bool toggle);
}