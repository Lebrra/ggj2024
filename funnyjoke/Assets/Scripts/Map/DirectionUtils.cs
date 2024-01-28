using System.Collections;
using System.Linq;
using System;
using UnityEngine;

public static class DirectionUtils
{
    public static Direction SelectRandomDirection()
    {
        var options = new[] {Direction.North, Direction.South, Direction.East, Direction.West, Direction.Central};
        System.Random rnd = new System.Random();
        return options.OrderBy(_ => rnd.Next()).FirstOrDefault();
    }
    
    public static string BreakToString(this Direction self)
    {
        switch (self.FlagCount())
        {
            case 1: return self.ToString().ToLower();
            case 2:
                var split = self.ToString().Split(',');
                return split[0].ToLower() + split[1].ToLower().Trim();
            default: return "none";
        }
    }
    
    public static Direction AddDirection(this Direction self, Direction other)
    {
        return self | other;
    }

    public static Direction RemoveDirection(this Direction self, Direction flag) 
    {
        return self & ~flag;
    }
        
    public static bool HasFlag(this Direction self, Direction flag)
    {
        return (self & flag) == flag;
    }
    
    static int FlagCount(this Direction self)
    {
        return new BitArray(new[] { (int)self }).OfType<bool>().Count(x => x);
    }
}

[Flags]
public enum Direction
{
    Central = 2,
    North = 4,
    South = 8,
    East = 16,
    West = 32,
}