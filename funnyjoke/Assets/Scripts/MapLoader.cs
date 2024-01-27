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
    public static Func<Transform> GetCurrentObjectiveTransform;

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
    
    void Start()
    {
        // assign Actions
        GetRandomLandmark += GiveLandmark;
        GetCurrentObjectiveTransform += GiveObjectivePosition;

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
        GetCurrentObjectiveTransform -= GiveObjectivePosition;
    }

    public Transform GiveLandmark()
    {
        System.Random rnd = new System.Random();
        return landmarks.OrderBy((_) => rnd.Next()).FirstOrDefault().transform;
    }
    
    public Transform GiveObjectivePosition()
    {
        var current = PlayerProgress.Current;
        if (activeObjectiveSpawns.ContainsKey(current)) return activeObjectiveSpawns[current].transform;
        else
        {
            Debug.LogError($"[Map] Cannot give location for {current}");
            return null;
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

[Flags]
public enum Direction
{
        North = 2,
        East = 4,
        South = 8,
        West = 16,
        Central = 32
}