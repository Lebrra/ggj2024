using Kickstarter.Inputs;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class Compass : MonoBehaviour
{
    private Transform playerTransform;
    private RawImage rawImage;
    
    #region UnityEvents
    private void Awake()
    {
        playerTransform = FindObjectOfType<Player>().transform;
        TryGetComponent(out rawImage);
    }

    private void Update()
    {
        var rect = rawImage.uvRect;
        rect.x = playerTransform.rotation.eulerAngles.y / 360;
        rawImage.uvRect = rect;
    }
    #endregion
}
