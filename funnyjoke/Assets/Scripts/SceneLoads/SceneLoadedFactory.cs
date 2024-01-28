using System;

public static class SceneLoadedFactory
{
    public static Action GetSceneFunction(int sceneIndex)
    {
        return sceneIndex switch
        {
            0 => new MenuLoaded().OnSceneLoad,
            1 => new GameplayLoaded().OnSceneLoad,
            2 => new CompleteLoaded().OnSceneLoad,
            _ => default,
        };
    }
}