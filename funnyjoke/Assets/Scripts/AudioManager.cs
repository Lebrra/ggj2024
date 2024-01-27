using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static void PlaySound(string key)
    {
        Debug.Log($"[AudioManager] Play key: {key}");
    }
}
