using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kickstarter.Inputs;

public class Interacting : MonoBehaviour, IInputReceiver
{

    [SerializeField]
    FloatInput m_interact;


    public void DeregisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        m_interact.DeregisterInput(takeInput, playerIdentifier);
    }

    public void RegisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        m_interact.RegisterInput(takeInput, playerIdentifier);
    }

    private void takeInput(float v)
    {
        if (v == 0)
        {
            return;
        }
    }
}
