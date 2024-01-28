using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    private GameObject pauseMenu;
    
    #region UnityEvents
    private void Awake()
    {
        pauseMenu = transform.childCount switch
        {
            0 => null,
            _ => transform.GetChild(0).gameObject,
        };
    }
    #endregion
    
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        pauseMenu?.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        pauseMenu?.SetActive(false);
        Time.timeScale = 1;
    }
}
