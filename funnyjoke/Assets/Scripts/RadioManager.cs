using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioManager : MonoBehaviour
{
    [SerializeField] 
    float interval = 30F;

    [SerializeField] 
    bool radioOn = false;
    public bool RadioOn
    {
        get => radioOn;
        set => radioOn = value;
    }
    
    //[SerializeField]
    
    
}
