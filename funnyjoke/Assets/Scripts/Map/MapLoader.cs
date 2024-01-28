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
    SpawnPoint[] hideSpawns;
    [SerializeField]
    SpawnPoint[] landmarks;
    
    Dictionary<Identifier, SpawnPoint> activeObjectiveSpawns;
    //List<SpawnPoint> activeHideSpawns;
    
    [SerializeField]
    Spawnables objectives;
    [SerializeField]
    Spawnables hideables;
    
    int lastLandmark = -1;
    
    void Start()
    {
        // assign Actions
        GetRandomLandmark += GiveLandmark;
        GetCurrentObjective += GiveObjective;

        System.Random rnd = new System.Random();
        
        // spawn objectives:
        List<SpawnPoint> tempObjectiveSpawns = objectiveSpawns.OrderBy((_) => rnd.Next()).ToList();
        activeObjectiveSpawns = new Dictionary<Identifier, SpawnPoint>();
        foreach (var objective in objectives.SpawnList)
        {
            SpawnPoint spawnPoint = tempObjectiveSpawns.FirstOrDefault();
            tempObjectiveSpawns.Remove(spawnPoint);
            spawnPoint.loadedSpawnable = objective;
            if (activeObjectiveSpawns.ContainsKey(objective.key))
                Debug.LogError("Tried to load objective that has already spawned!");
            else activeObjectiveSpawns.Add(objective.key, spawnPoint);
            
            if (spawnPoint.transform && objective.prefab) 
                Instantiate(objective.prefab, spawnPoint.transform);
        }

        // spawn hiding spots:
        List<SpawnPoint> tempHideSpawns = hideSpawns.OrderBy((_) => rnd.Next()).ToList();
        //activeHideSpawns = new List<SpawnPoint>();
        foreach (var hide in hideables.SpawnList)
        {
            SpawnPoint spawnPoint = tempHideSpawns.FirstOrDefault();
            tempHideSpawns.Remove(spawnPoint);
            spawnPoint.loadedSpawnable = hide;
            //activeHideSpawns.Add(spawnPoint);
            
            if (spawnPoint.transform && hide.prefab) 
                Instantiate(hide.prefab, spawnPoint.transform);
        }
    }

    private void OnDestroy()
    {
        GetRandomLandmark -= GiveLandmark;
        GetCurrentObjective -= GiveObjective;
    }

    public Transform GiveLandmark()
    {
        if (landmarks == null || landmarks.Length == 0) return null;
        else if (landmarks.Length == 1) return landmarks.FirstOrDefault().transform;
        
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
}

[Serializable]
public struct SpawnPoint
{
    public Transform transform;
    public Direction directions;
    public Spawnable loadedSpawnable;
}

