using Kickstarter.StateControllers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        Gameplay,
        Paused,
        GameOver,
        Win,
    }

    public StateMachine<GameState> StateMachine { get; private set; }
    
    #region UnityEvents
    private void Awake()
    {
        InitializeStateMachine();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion
    
    #region StateListeners
    private void EnterGameplay()
    {
        
    }

    private void LoseGame()
    {
        
    }

    private void CompleteGame()
    {
        
    }
    #endregion
    
    private void InitializeStateMachine()
    {
        StateMachine = new StateMachine<GameState>.Builder()
            .WithInitialState(GameState.MainMenu)
            .WithTransition(GameState.MainMenu, GameState.Gameplay)
            .WithTransition(GameState.Gameplay, GameState.Paused)
            .WithTransition(GameState.Gameplay, GameState.GameOver)
            .WithTransition(GameState.Gameplay, GameState.Win)
            .WithTransition(GameState.Paused, GameState.Gameplay)
            .WithTransition(GameState.Paused, GameState.GameOver)
            .WithTransitionListener(GameState.Gameplay, EnterGameplay, StateMachine<GameState>.StateChange.Entry)
            .WithTransitionListener(GameState.GameOver, LoseGame, StateMachine<GameState>.StateChange.Entry)
            .WithTransitionListener(GameState.Win, CompleteGame, StateMachine<GameState>.StateChange.Entry)
            .Build();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneLoadedFactory.GetSceneFunction(scene)();
    }
}
