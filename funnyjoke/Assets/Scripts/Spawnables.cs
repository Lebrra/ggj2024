using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawnables", order = 100)]
public class Spawnables : ScriptableObject
{
    [SerializeField] 
    Spawnable[] spawnList;
    public Spawnable[] SpawnList => spawnList;
}

[Serializable]
public struct Spawnable
{
    public Objective key;
    public GameObject prefab;
}

public enum Objective
{
    Hide = 0,
    Balloon,
    Stuffie,
    Hat,
    Ticket,
    Cotton_Candy
}