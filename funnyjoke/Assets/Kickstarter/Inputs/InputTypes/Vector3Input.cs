using UnityEngine;

namespace Kickstarter.Inputs
{
    /// <summary>
    /// Represents an input asset for handling Vector3-based input actions in Unity.
    /// Inherits the functionality from the InputAsset class for Vector3 input handling.
    /// </summary>
    [CreateAssetMenu(fileName = "Vector3 Input", menuName = "Kickstarter/Input Assets/Vector3")]
    public sealed class Vector3Input : InputAsset<Vector3> { }
}
