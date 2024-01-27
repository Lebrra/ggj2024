using UnityEngine;
using UnityEngine.InputSystem;

namespace Kickstarter.Inputs
{
    /// <summary>
    /// Manages the initialization and enabling of input assets within the game.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputAsset[] inputObjects;

        /// <summary>
        /// Initializes the input assets using available devices and players.
        /// </summary>
        private void Awake()
        {
            InitializeInputs();
        }

        /// <summary>
        /// Initializes all input assets with connected devices and players.
        /// </summary>
        public void InitializeInputs()
        {
            var devices = InputSystem.devices.ToArray();
            foreach (var inputObject in inputObjects)
                inputObject.Initialize(devices);
            EnableAll();
        }

        /// <summary>
        /// Enables all initialized input assets.
        /// </summary>
        public void EnableAll()
        {
            foreach (var inputObject in inputObjects)
                inputObject.EnableInput();
        }
    }
}
