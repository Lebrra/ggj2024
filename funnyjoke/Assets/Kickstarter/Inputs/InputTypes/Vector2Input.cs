using UnityEngine;

namespace Kickstarter.Inputs
{
    /// <summary>
    /// Represents an input asset for handling Vector2-based input actions in Unity.
    /// Inherits the functionality from the InputAsset class for Vector2 input handling.
    /// </summary>
    [CreateAssetMenu(fileName = "Vector2 Input", menuName = "Kickstarter/Input Assets/Vector2")]
    public sealed class Vector2Input : InputAsset<Vector2> { }
}
