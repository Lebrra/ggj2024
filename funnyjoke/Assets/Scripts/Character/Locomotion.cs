using Kickstarter.Inputs;
using UnityEngine;

public class Locomotion : MonoBehaviour, IInputReceiver
{
    [SerializeField] private Vector2Input movementInput;
    [SerializeField] private float movementSpeed;

    private Rigidbody body;
    
    private Vector3 rawInput;
    
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
    }
    #endregion
    
    #region UnityEvents
    private void Awake()
    {
        transform.root.TryGetComponent(out body);
    }
    
    private void FixedUpdate()
    {
        body.velocity = transform.TransformDirection(rawInput) * movementSpeed;
    }
    #endregion
}
