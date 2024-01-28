using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kickstarter.Inputs;
using BeauRoutine;

public class Interacting : MonoBehaviour, IInputReceiver
{

    [SerializeField]
    FloatInput m_interact;

    [SerializeField]
    float interactionDistance = 4F;
    
    [SerializeField]
    Animator inventoryAnim;
    [SerializeField]
    Animator interactAnim;
    
    bool isEnabled = true;
    bool canInteract = false;
    Routine distanceRoutine;
    
    public void DeregisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        m_interact.DeregisterInput(takeInput, playerIdentifier);
        PlayerProgress.ObjectComplete -= UpdateInventoryUI;
        PlayerProgress.ProgressUpdate -= Enable;
    }

    public void RegisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        m_interact.RegisterInput(takeInput, playerIdentifier);
        PlayerProgress.ObjectComplete += UpdateInventoryUI;
        PlayerProgress.ProgressUpdate += Enable;
    }

    private void Start()
    {
        Enable(-1);
    }

    public void Enable(int _)
    {
        isEnabled = true;
        distanceRoutine.Replace(CheckForDistance());
    }

    IEnumerator CheckForDistance()
    {
        while (isEnabled)
        {
            if (canInteract != DistanceToObjective())
            {
                canInteract = DistanceToObjective();
                interactAnim?.SetBool("status", canInteract);
            }
            
            yield return Time.deltaTime;
        }
    }
    
    void UpdateInventoryUI(Identifier item)
    {
        if (!inventoryAnim) return;
        switch (item)
        {
            case Identifier.Hide | Identifier.Landmark:
                Debug.LogError("Invaild ui code");
                break;
            case Identifier.Cotton_Candy:
                inventoryAnim.SetBool("cottoncandy", true);
                break;
            default:
                inventoryAnim.SetBool(item.ToString().Trim(new char[] {'_'}).ToLower(), true);
                break;
        }
    }

    bool DistanceToObjective()
    {
        var objective = MapLoader.GetCurrentObjective?.Invoke();
        if (objective is SpawnPoint valid && valid.loadedSpawnable != null)
        {
            return Vector3.Distance(transform.position, valid.loadedSpawnable.position) <= interactionDistance;
        }
        return false;
    }

    private void takeInput(float v)
    {
        if (v == 0)
        {
            return;
        }
        
        if (canInteract && isEnabled)
        {
            isEnabled = false;
            distanceRoutine.Stop();
            
            PlayerProgress.CompleteObjective();
            // trigger next objective 
        }
    }
}
