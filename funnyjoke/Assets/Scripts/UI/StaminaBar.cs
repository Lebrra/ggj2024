using Kickstarter.Observer;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class StaminaBar : MonoBehaviour, IObserver<Locomotion.StaminaNotification>
{
    private ProgressBar staminaBar;
    
    #region UnityEvents
    private void Awake()
    {
        staminaBar = GetComponent<UIDocument>().rootVisualElement.Q<ProgressBar>();
        FindObjectOfType<Locomotion>().AddObserver(this);
    }
    #endregion
    
    public void OnNotify(Locomotion.StaminaNotification argument)
    {
        staminaBar.value = argument.Stamina / argument.MaxStamina * 100;
    }
}
