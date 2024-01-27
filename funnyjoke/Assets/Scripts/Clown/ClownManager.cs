using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
public class ClownManager : MonoBehaviour
{
    public static ClownManager instance;

    public static Action Clown_Chase;
    public static Action Clown_Roam;
    public static Action Clown_Idle;
    public static Action Clown_Patrol;
    public GameObject player;

    private NavMeshAgent agent;
    private float speed;


    
    private NavMeshPath currentPath;

    [SerializeField] private Clown_Level currentStance;
    [SerializeField] private List<Clown_Level> m_Levels = new List<Clown_Level>();
    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }

        currentPath = new NavMeshPath();

        agent = this.GetComponent<NavMeshAgent>();
        agent.speed = currentStance.chase_speed;

        Clown_Chase += Chase;
        Clown_Roam += Roam;
        Clown_Idle += Idle;
        Clown_Patrol += Patrol;
    }

    private void Start() {

        

    }


    public void SetDestination(Vector3 location) {
        agent.SetDestination(location);
    }

    private void Chase() {
        agent.speed = currentStance.chase_speed;
        StartCoroutine(Stance_Chase());

        
    }
    private IEnumerator Stance_Chase(int chaseAmount = 1) {
        Vector3 pos = player.transform.position;
        

        yield return null; //extra frame to allow calculations
        Debug.Log(CalculateDistance(this.transform.position, pos));
        SetDestination(pos);
        yield return new WaitForSeconds(CalculateDistance(this.transform.position, pos) / currentStance.chase_speed);

        Debug.Log("Done Chasing");

        if(chaseAmount < currentStance.chaseAmount) {
            StartCoroutine(Stance_Chase(++chaseAmount));
        }

    }

    private void Roam() {

    }

    private void Idle() {

    }

    private void Patrol() {

    }

    /// <summary>
    /// Used for Navmesh distance calculations
    /// </summary>
    /// <param name="startLocation"></param>
    /// <param name="endLocation"></param>
    private float CalculateDistance(Vector3 startLocation, Vector3 endLocation) {
        NavMesh.CalculatePath(startLocation, endLocation, 0, currentPath);
        float dist = 0;
        if (currentPath.status != NavMeshPathStatus.PathInvalid) {
            //good
            dist = agent.remainingDistance;
        }
        else {
            //bad
            dist = Vector3.Distance(startLocation, endLocation);
        }

        return dist;
    }
}

/// <summary>
/// Clown Levels are used to hold onto current Clown values. Level increases when objectives are obtained.
/// </summary>

[Serializable]
public class Clown_Level {
    public int id;
    public float chase_speed;
    public int chaseAmount; //amount of times the Clown will chase before returning to Roam
    public Clown_Level(int _id) {
        id = _id;
    }
}
