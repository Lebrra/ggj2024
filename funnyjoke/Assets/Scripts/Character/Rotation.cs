using Kickstarter.Inputs;
using UnityEngine;

public class Rotation : MonoBehaviour, IInputReceiver
{
    [SerializeField] private Vector2Input lookRotationInput;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float verticalClamp = 89;
    
    #region InputHandler
    public void RegisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        lookRotationInput.RegisterInput(OnLookRotationInputChange, playerIdentifier);
    }

    public void DeregisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        lookRotationInput.DeregisterInput(OnLookRotationInputChange, playerIdentifier);
    }

    public void OnLookRotationInputChange(Vector2 input)
    {
        var rotation = transform.root.rotation.eulerAngles;
        rotation.y += rotationSpeed * input.x;
        transform.root.rotation = Quaternion.Euler(rotation);

        rotation = transform.rotation.eulerAngles;
        rotation.x -= rotationSpeed * input.y;
        if (rotation.x > verticalClamp && rotation.x < 180) rotation.x = verticalClamp;
        if (rotation.x < 360 - verticalClamp && rotation.x > 180) rotation.x = -verticalClamp;
        transform.rotation = Quaternion.Euler(rotation);
    }
    #endregion
}
