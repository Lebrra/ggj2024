using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class PlayerProgress
{
    //static Dictionary<Identifier, ObjectiveState> progress;
    
    static List<Identifier> progress;
    
    static Identifier current = Identifier.Hide;
    public static Identifier Current => current;

    public static void InitializeProgress()
    {
        progress = new List<Identifier>();
        progress.Add(Identifier.Balloon);
        progress.Add(Identifier.Hat);
        progress.Add(Identifier.Stuffie);
        progress.Add(Identifier.Ticket);
        progress.Add(Identifier.Cotton_Candy);
    }
    
    static void SetNewActive()
    {
        System.Random rnd = new System.Random();
        current = progress.OrderBy((_) => rnd.Next()).FirstOrDefault();
        progress.Remove(current);
        
    }
    
    public static bool CheckWin()
    {
        return progress.Count == 0;
    }
    
    public static void CompleteObjective()
    {
        if (CheckWin()) Debug.LogWarning("WIN");
        else SetNewActive();
    }
    
    public static int GetRemainingObjectiveCount()
    {
        return progress?.Count ?? 0;
    }
}