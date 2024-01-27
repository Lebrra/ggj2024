using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "AudioBlurb", order = 100)]
public class AudioBlurbs : ScriptableObject
{
    [SerializeField]
    List<VOBlurb> blurbs;
    
    int lastSelected = -1;
    
    /// <summary>
    /// Ensures the same blurb will not be picked back to back as long as more than one exists
    /// </summary>
    /// <returns></returns>
    public VOBlurb GetRandomBlurb()
    {
        if (blurbs == null || blurbs.Count == 0) return new VOBlurb();
        else if (blurbs.Count == 1) return blurbs.FirstOrDefault();
        
        int selected = lastSelected;
        while (selected == lastSelected) 
            selected = UnityEngine.Random.Range(0, blurbs.Count);
        
        lastSelected = selected;
        return blurbs[selected];
    }
    
    /// <summary>
    /// Key matching (sorry for strings)
    /// </summary>
    public VOBlurb GetBlurb(string key)
    {
        if (blurbs == null) return new VOBlurb();
        foreach (var blurb in blurbs)
        {
            if (key == blurb.name) return blurb;
        }
        
        return new VOBlurb();
    }
}

[Serializable]
public struct VOBlurb
{
    public string name;
    public AudioClip clip;
}