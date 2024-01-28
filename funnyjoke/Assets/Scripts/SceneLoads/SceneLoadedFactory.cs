using System;
using UnityEngine.SceneManagement;

public static class SceneLoadedFactory
{
    public static Action GetSceneFunction(Scene scene)
    {
        int sceneIndex = 0;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i) != scene)
                continue;
            sceneIndex = i;
            break;
        }
        
        return sceneIndex switch
        {
            0 => new MenuLoaded().OnSceneLoad,
            1 => new GameplayLoaded().OnSceneLoad,
            2 => new CompleteLoaded().OnSceneLoad,
            _ => default,
        };
    }
}