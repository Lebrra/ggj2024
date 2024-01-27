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
    public static Func<Transform> GetRandomObjectiveTransform;
    public static Func<Objective, Transform> GetObjectiveTransform;
    
    [SerializeField] 
    SpawnPoint[] objectiveSpawns;
    [SerializeField] 
    SpawnPoint[] hideSpawns;
    
    Dictionary<Objective, SpawnPoint> activeObjectiveSpawns;
    List<SpawnPoint> activeHideSpawns;
    
    [SerializeField]
    Spawnables objectives;
    [SerializeField]
    Spawnables hideables;
    
    void Start()
    {
        // assign Actions
        GetRandomObjectiveTransform += () => GiveObjectivePosition();
        GetObjectiveTransform += GiveObjectivePosition;

        System.Random rnd = new System.Random();
        
        // spawn objectives:
        List<SpawnPoint> tempObjectiveSpawns = objectiveSpawns.OrderBy((_) => rnd.Next()).ToList();
        activeObjectiveSpawns = new Dictionary<Objective, SpawnPoint>();
        foreach (var objective in objectives.SpawnList)
        {
            SpawnPoint spawnPoint = tempObjectiveSpawns.FirstOrDefault();
            tempObjectiveSpawns.Remove(spawnPoint);
            spawnPoint.loadedSpawnable = objective;
            if (activeObjectiveSpawns.ContainsKey(objective.key))
                Debug.LogError("Tried to load objective that has already spawned!");
            else activeObjectiveSpawns.Add(objective.key, spawnPoint);
            
            // TODO: spawn
        }

        // spawn hiding spots:
        List<SpawnPoint> tempHideSpawns = hideSpawns.OrderBy((_) => rnd.Next()).ToList();
        activeHideSpawns = new List<SpawnPoint>();
        foreach (var hide in hideables.SpawnList)
        {
            SpawnPoint spawnPoint = tempHideSpawns.FirstOrDefault();
            tempHideSpawns.Remove(spawnPoint);
            spawnPoint.loadedSpawnable = hide;
            activeHideSpawns.Add(spawnPoint);
            
            // TODO: spawn
        }
        
        TESTING();
    }

    void TESTING()
    {
        foreach (var item in activeObjectiveSpawns)
        {
            Debug.Log($"{item.Key} spawning in {item.Value.directions}");
        }
        
        var test = GetRandomObjectiveTransform?.Invoke();
    }
    
    private void OnDestroy()
    {
        GetRandomObjectiveTransform -= () => GiveObjectivePosition();
        GetObjectiveTransform -= GiveObjectivePosition;
    }

    public Transform GiveObjectivePosition(Objective objective = Objective.Hide)
    {
        if (objective == Objective.Hide)
        {
            // randomly return one
            int rand = UnityEngine.Random.Range(0, activeObjectiveSpawns.Count);
            var selected = activeObjectiveSpawns.Values.ToList()[rand];
            Debug.Log($"Randomly selected: {selected.loadedSpawnable.key}");
            return selected.transform;
        }
        else if (activeObjectiveSpawns.ContainsKey(objective)) return activeObjectiveSpawns[objective].transform;
        
        Debug.LogError("Error returning objective transform!");
        return null;
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