using System.Threading.Tasks;
using Kickstarter.Inputs;
using UnityEngine;

public class DayneMode : MonoBehaviour, IInputReceiver
{
    [SerializeField] private FloatInput dayneModeInput;
    [SerializeField] private float comboTime;
    [SerializeField] private int countRequired;

    private static bool active;
    private bool timerStatus;

    private int time;
    private int count;
    
    #region InputHandler
    public void RegisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        dayneModeInput.RegisterInput(OnDayneModeInputChange, playerIdentifier);
    }

    public void DeregisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        dayneModeInput.DeregisterInput(OnDayneModeInputChange, playerIdentifier);
    }

    private void OnDayneModeInputChange(float input)
    {
        if (!timerStatus)
            InputComboTimer();
        if (input == 1)
            count++;
    }
    #endregion
    
    #region UnityEvents
    private void Start()
    {
        if (active)
        {
            Debug.Log("DAYNE MODE AAAHHHHHHH");
        }
        active = false;
    }

    private void OnDestroy()
    {
        time = (int)(comboTime * 1000) + 1;
    }
    #endregion

    private async void InputComboTimer()
    {
        timerStatus = true;
        time = 0;
        int comboTimer = (int)(comboTime * 1000);
        while (time < comboTimer)
        {
            int timeStep = (int)(Time.deltaTime * 1000);
            await Task.Delay(timeStep);
            time += timeStep;
        }
        timerStatus = false;
        if (count >= countRequired)
            active = true;
        count = 0;
    }
}
