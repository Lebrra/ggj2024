using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class PlayerProgress
{
    public static Action<int> ProgressUpdate = null;
    public static Action<Identifier> ObjectComplete = null;
    
    static List<Identifier> remaining;
    
    static Identifier current = Identifier.Hide;
    public static Identifier Current => current;

    public static void InitializeProgress()
    {
        remaining = new List<Identifier>();
        remaining.Add(Identifier.Balloon);
        remaining.Add(Identifier.Ball);
        remaining.Add(Identifier.Stuffie);
        remaining.Add(Identifier.Ticket);
        remaining.Add(Identifier.Cotton_Candy);
        
        SetNewActive();
    }
    
    static void SetNewActive()
    {
        System.Random rnd = new System.Random();
        current = remaining.OrderBy((_) => rnd.Next()).FirstOrDefault();
        remaining.Remove(current);
        
        ProgressUpdate?.Invoke(GetCurrentCompletion());
    }
    
    public static bool CheckWin()
    {
        return remaining.Count == 0;
    }
    
    public static void CompleteObjective()
    {
        ObjectComplete?.Invoke(current);
        if (CheckWin()) Debug.LogWarning("WIN");
        else SetNewActive();
    }
    
    public static int GetCurrentCompletion()
    {
        int progress = 5 - remaining?.Count ?? 0 + 1;
        Debug.Log($"Current difficulty level: {progress}");
        return progress;
    }
}