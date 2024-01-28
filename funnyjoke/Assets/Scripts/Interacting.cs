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

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
