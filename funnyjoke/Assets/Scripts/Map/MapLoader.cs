using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Spawn objects using spawnpoints
/// </summary>
public class MapLoader : MonoBehaviour
{
    public static Func<Transform> GetRandomLandmark;
    public static Func<SpawnPoint> GetCurrentObjective;

    [SerializeField] 
    SpawnPoint[] objectiveSpawns;
    [SerializeField]
    Transform[] landmarks;
    
    Dictionary<Identifier, SpawnPoint> activeObjectiveSpawns;

    [SerializeField]
    Spawnables objectives;

    int lastLandmark = -1;
    
    void Start()
    {
        // assign Actions
        GetRandomLandmark += GiveLandmark;
        GetCurrentObjective += GiveObjective;
        PlayerProgress.ObjectComplete += ObjectiveComplete;
        PlayerProgress.ProgressUpdate += UpdateActiveObjective;

        System.Random rnd = new System.Random();
        
        // spawn objectives:
        List<SpawnPoint> tempObjectiveSpawns = objectiveSpawns.OrderBy((_) => rnd.Next()).ToList();
        activeObjectiveSpawns = new Dictionary<Identifier, SpawnPoint>();
        foreach (var objective in objectives.SpawnList)
        {
            SpawnPoint spawnPoint = tempObjectiveSpawns.FirstOrDefault();
            tempObjectiveSpawns.Remove(spawnPoint);
            
            if (activeObjectiveSpawns.ContainsKey(objective.key))
                Debug.LogError("Tried to load objective that has already spawned!");
            else
            { 
                spawnPoint.loadedSpawnable = Instantiate(objective.prefab, spawnPoint.transform).transform;
                activeObjectiveSpawns.Add(objective.key, spawnPoint);
            }
        }
        
        // animate first one up
        UpdateActiveObjective(-1);
    }

    private void OnDestroy()
    {
        GetRandomLandmark -= GiveLandmark;
        GetCurrentObjective -= GiveObjective;
        PlayerProgress.ObjectComplete -= ObjectiveComplete;
        PlayerProgress.ProgressUpdate -= UpdateActiveObjective;
    }

    public Transform GiveLandmark()
    {
        if (landmarks == null || landmarks.Length == 0) return null;
        else if (landmarks.Length == 1) return landmarks.FirstOrDefault();
        
        int selected = lastLandmark;
        while (selected == lastLandmark)
            selected = UnityEngine.Random.Range(0, landmarks.Length);
        
        lastLandmark = selected;
        return landmarks[selected].transform;
    }
    
    public SpawnPoint GiveObjective()
    {
        var current = PlayerProgress.Current;
        if (activeObjectiveSpawns.ContainsKey(current)) return activeObjectiveSpawns[current];
        else
        {
            Debug.LogError($"[Map] Cannot give location for {current}");
            return new SpawnPoint();
        }
    }
    
    void ObjectiveComplete(Identifier objective)
    {
        if (activeObjectiveSpawns.ContainsKey(objective)) 
        {
           var anim = activeObjectiveSpawns[objective].loadedSpawnable.GetComponent<Animator>();
           anim?.SetBool("elevator", false);
        }
        
    }
    
    void UpdateActiveObjective(int _)
    {
        if (activeObjectiveSpawns.ContainsKey(PlayerProgress.Current)) 
            activeObjectiveSpawns[PlayerProgress.Current].loadedSpawnable?.
                GetComponent<Animator>()?.SetBool("elevator", true);
        else Debug.LogError($"[Map] Error setting anim for {PlayerProgress.Current}");
    }
}

[Serializable]
public struct SpawnPoint
{
    public Transform transform;
    public Direction directions;
    public Transform loadedSpawnable;
}

