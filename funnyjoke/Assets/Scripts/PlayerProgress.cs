using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class PlayerProgress
{
    static Dictionary<Identifier, bool> progress;
    
    public static void InitializeProgress()
    {
        progress = new Dictionary<Identifier, bool>();
        progress.Add(Identifier.Balloon, false);
        progress.Add(Identifier.Hat, false);
        progress.Add(Identifier.Stuffie, false);
        progress.Add(Identifier.Ticket, false);
        progress.Add(Identifier.Cotton_Candy, false);
    }
    
    public static bool CheckWin()
    {
        foreach (var pair in progress) 
            if (!pair.Value) return false;
        return true;
    }
    
    public static void UpdateProgress(Identifier key, bool status)
    {
        if (progress.ContainsKey(key))
        { 
            progress[key] = status;
            var keyword = status ? "collected" : "dropped";
            Debug.Log($"[Progression] Updated {key} to {keyword}");
        }
        if (CheckWin()) Debug.Log("WIN");
    }
    
    public static List<Identifier> GetRemainingObjectives()
    {
        var remaining = new List<Identifier>();
        foreach (var pair in progress) 
            if (!pair.Value) remaining.Add(pair.Key);
        return remaining.Count > 0 ? remaining : null;
    }
    
    public static Identifier GetRandomRemainingObjective()
    {
        System.Random rnd = new System.Random();
        var remainings = GetRemainingObjectives();
        return remainings != null ? remainings.OrderBy((_) => rnd.Next()).FirstOrDefault() : Identifier.Hide;
    }
    
    public static int GetRemainingObjectiveCount()
    {
        var remainings = GetRemainingObjectives();
        return remainings?.Count ?? 0;
    }
}
