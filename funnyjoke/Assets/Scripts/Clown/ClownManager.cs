using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using BeauRoutine;
public class ClownManager : MonoBehaviour
{
    
    
    public GameObject player;

    private NavMeshAgent agent;
    private float default_speed;
    private NavMeshPath currentPath;

    [SerializeField] private List<Clown_Level> m_Levels = new List<Clown_Level>();

    private void Awake() {
        

        currentPath = new NavMeshPath();

        agent = this.GetComponent<NavMeshAgent>();
        

        
    }

    private void Start() {

        

    }

    public void ClownInitialize(Clown_Level level) {

        agent.speed = level.chase_speed;

    }

    public void SetDestination(Vector3 location) {
        agent.SetDestination(location);
    }
    public IEnumerator Stance_Chase(float speed, int maxAmount, int currentChase = 0) {
        Vector3 pos = player.transform.position;
        agent.speed = speed;

        yield return null; //extra frame to allow calculations
        Debug.Log(CalculateDistance(this.transform.position, pos));
        SetDestination(pos);
        yield return new WaitForSeconds(CalculateDistance(this.transform.position, pos) / speed);

        Debug.Log("Done Chasing");

        if(currentChase < maxAmount) {
            StartCoroutine(Stance_Chase(speed, ++currentChase, maxAmount));
        }

    }

    private void Roam() {
        agent.speed = default_speed;
        StartCoroutine(Stance_Roam());
    }

    public IEnumerator Stance_Roam() {

        agent.SetDestination(MapLoader.GetRandomLandmark().position);

        yield return null;
    }

    private void Idle() {
        agent.Stop(); 

        StartCoroutine(Stance_Idle());
    }

    public IEnumerator Stance_Idle(float idleTime = 20) {
        float i = 0;
        float duration = UnityEngine.Random.Range(1, 3);
        float t = idleTime;
        Quaternion newRotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0));
        Quaternion currentRotation = transform.rotation;
        do {
            t -= Time.deltaTime;

            transform.rotation = Quaternion.Slerp(currentRotation, newRotation, i / duration);
            i += Time.deltaTime;
            yield return null;
        } while (i <= duration);

        if(t > 0) {
            StartCoroutine(Stance_Idle(t));
        }
        yield return null;
    }

    
    private void Patrol() {

    }

    public IEnumerator Stance_Patrol() {
        yield return null;
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


    private void OnCollisionEnter(Collision collision) {
        if(collision.transform.tag == "Player") {
            //player dead
        }

    }
}


