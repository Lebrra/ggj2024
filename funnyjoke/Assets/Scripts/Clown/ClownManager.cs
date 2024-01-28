using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using BeauRoutine;
using System.Linq;
public class ClownManager : MonoBehaviour
{
    
    
    public GameObject player;

    private NavMeshAgent agent;
    [SerializeField] private float default_speed;
    private NavMeshPath currentPath;

    [SerializeField] private List<Clown_Level> m_Levels = new List<Clown_Level>();
    
    private void Awake() {
        

        currentPath = new NavMeshPath();

        agent = this.GetComponent<NavMeshAgent>();
        

        
    }

    private void Start() {

        

    }

    public void ClownInitialize(Clown_Level level) {

        agent.speed = default_speed;

    }

    public void SetDestination(Vector3 location) {
        agent.SetDestination(location);
    }
    public IEnumerator Stance_Chase(float speed, int maxAmount, int currentChase = 0) {
        Vector3 pos = player.transform.position;
        agent.speed = speed;

        yield return null; //extra frame to allow calculations
        //Debug.Log(CalculateDistance(this.transform.position, pos));
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

        Vector3 pos = MapLoader.GetRandomLandmark().position;

        agent.SetDestination(pos);

        yield return new WaitForSeconds(CalculateDistance(this.transform.position, pos) / default_speed);
    }

    private void Idle() {
        agent.Stop(); 

        StartCoroutine(Stance_Idle());
    }

    public IEnumerator Stance_Idle(float idleTime = 20) {
        Debug.Log("Test");
        Quaternion newRotation;
        Quaternion currentRotation = transform.rotation;

        int currentIterations = 0;

        do {
            newRotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0));
            yield return transform.RotateQuaternionTo(newRotation, 4);
            currentIterations++;

        } while (currentIterations != 5);

        yield return null;
    }

    
    private void Patrol() {

    }

    public IEnumerator Stance_Patrol(Landmark landmark = null) {
        int currentIterations = 0;
        

        do {
            Vector3 pos;
            NavMeshHit hit;
            if(NavMesh.SamplePosition(landmark.GetPointFromLandmark(), out hit, landmark.collider.radius, NavMesh.AllAreas)){
                pos = hit.position;
                agent.SetDestination(pos);
                yield return new WaitForSeconds(CalculateDistance(this.transform.position, pos) / default_speed);
            }

            
            currentIterations++;

        } while (currentIterations != 5);


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


