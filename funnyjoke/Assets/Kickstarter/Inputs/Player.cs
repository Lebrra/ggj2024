using UnityEngine;

namespace Kickstarter.Inputs
{
    /// <summary>
    /// Represents a player in the game, facilitating input registration and deregistration.
    /// </summary>
    [SelectionBase]
    public class Player : MonoBehaviour
    {
        [field: SerializeField]
        public PlayerIdentifier Identifier { get; private set; }

        /// <summary>
        /// Enumeration representing different player identifications.
        /// </summary>
        public enum PlayerIdentifier
        {
            KeyboardAndMouse,
            Player1,
            Player2,
            Player3,
            Player4,
        }
        
        private IInputReceiver[] inputReceivers;

        private void Awake()
        {
            inputReceivers = GetComponentsInChildren<IInputReceiver>();
        }
        
        private void Start()
        {
            foreach (var inputReceiver in inputReceivers)
                inputReceiver.RegisterInputs(Identifier);
        }

        private void OnEnable()
        {
            foreach (var inputReceiver in inputReceivers)
                inputReceiver.RegisterInputs(Identifier);
        }

        private void OnDisable()
        {
            foreach (var inputReceiver in inputReceivers)
                inputReceiver.DeregisterInputs(Identifier);
        }
    }
}
