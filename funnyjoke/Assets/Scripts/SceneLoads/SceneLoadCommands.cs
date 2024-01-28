public interface ISceneLoadCommand
{
    public void OnSceneLoad();
}

public class MenuLoaded : ISceneLoadCommand
{
    public void OnSceneLoad()
    {
        
    }
}

public class GameplayLoaded : ISceneLoadCommand
{
    public void OnSceneLoad()
    {
        PlayerProgress.InitializeProgress();
    }
}

public class CompleteLoaded : ISceneLoadCommand
{
    public void OnSceneLoad()
    {
        
    }
}
