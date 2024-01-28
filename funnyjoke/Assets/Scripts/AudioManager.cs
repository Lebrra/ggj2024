using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static void PlayClip(AudioClip audioClip)
    {
        Debug.Log($"[AudioMan] Play {audioClip.name}");
    }
}
