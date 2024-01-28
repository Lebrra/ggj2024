using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeauRoutine;
using System;

public class AI_Manager : MonoBehaviour
{
    public static AI_Manager instance;

    public Landmark[] landmarks;
    public ClownManager clown;
    public Routine AI_CurrentStance;

    [SerializeField] private Clown_Level[] m_Levels;
    [SerializeField] private int currentClownLevel = 0;
    [SerializeField] private int tempLevel;
    [SerializeField] private ClownStances stance = ClownStances.none;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }

        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Start() {

        clown.ClownInitialize(m_Levels[currentClownLevel]);

        GetNewStance();
        //AI_CurrentStance.Replace(clown.Stance_Idle());
        
    }

    

    private void GetNewStance() {
        int rand = UnityEngine.Random.Range(1, 100);
        Debug.Log(rand);
        switch (rand) {
            case int n when (n > 0 && n < m_Levels[currentClownLevel].m_ChasePerc):
                AI_CurrentStance.Replace(clown.Stance_Chase(m_Levels[currentClownLevel].chase_speed, m_Levels[currentClownLevel].chaseAmount));
                stance = ClownStances.chase;
                AI_CurrentStance.OnComplete(GetNewStance);
                break;
            case int n when (n > m_Levels[currentClownLevel].m_ChasePerc && n < m_Levels[currentClownLevel].m_ChasePerc + m_Levels[currentClownLevel].m_RoamPerc):
                AI_CurrentStance.Replace(clown.Stance_Roam());
                stance = ClownStances.roam;
                AI_CurrentStance.OnComplete(GetNewStance);
                break;
            case int n when (n > m_Levels[currentClownLevel].m_RoamPerc && n < m_Levels[currentClownLevel].m_ChasePerc + m_Levels[currentClownLevel].m_RoamPerc + m_Levels[currentClownLevel].m_PatrolPerc):
                AI_CurrentStance.Replace(clown.Stance_Patrol(GetPatrolLandmark()));
                stance = ClownStances.patrol;
                AI_CurrentStance.OnComplete(GetNewStance);
                break;
            case int n when (n > m_Levels[currentClownLevel].m_PatrolPerc && n < m_Levels[currentClownLevel].m_ChasePerc + m_Levels[currentClownLevel].m_RoamPerc + m_Levels[currentClownLevel].m_PatrolPerc + m_Levels[currentClownLevel].m_IdlePerc):
                AI_CurrentStance.Replace(clown.Stance_Idle());
                stance = ClownStances.idle;
                AI_CurrentStance.OnComplete(GetNewStance);
                break;
            default: Debug.Log("wtf");
                AI_CurrentStance.Replace(clown.Stance_Chase(m_Levels[currentClownLevel].chase_speed, m_Levels[currentClownLevel].chaseAmount));
                stance = ClownStances.chase;
                AI_CurrentStance.OnComplete(GetNewStance);
                break;
        }

    }
    
    private Landmark GetPatrolLandmark() {
        float dist = Vector3.Distance(clown.transform.position, landmarks[0].transform.position);
        int landmarkIndex = 0;
        for(int i = 0; i < landmarks.Length; i++) {
            float val = Vector3.Distance(clown.transform.position, landmarks[i].transform.position);
            if (val <= dist) {
                landmarkIndex = i;
                dist = val;
            }
        }

        return landmarks[landmarkIndex];

    }



}



/// <summary>
/// Clown Levels are used to hold onto current Clown values. Level increases when objectives are obtained.
/// </summary>

[Serializable]
public class Clown_Level {
    
    public float chase_speed;
    public int chaseAmount; //amount of times the Clown will chase before returning to Roam

    public int m_ChasePerc;
    public int m_RoamPerc;
    public int m_PatrolPerc;
    public int m_IdlePerc;

}

public enum ClownStances {
    chase,
    roam,
    patrol,
    idle,
    none
}