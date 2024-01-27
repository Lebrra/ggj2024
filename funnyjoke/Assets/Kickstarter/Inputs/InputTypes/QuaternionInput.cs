using UnityEngine;

namespace Kickstarter.Inputs
{
    /// <summary>
    /// Represents an input asset for handling Quaternion-based input actions in Unity.
    /// Inherits the functionality from the InputAsset class for Quaternion input handling.
    /// </summary>
    [CreateAssetMenu(fileName = "Quaternion Input", menuName = "Kickstarter/Input Assets/Quaternion")]
    public sealed class QuaternionInput : InputAsset<Quaternion> { }
}
