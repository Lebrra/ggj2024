using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class EndScreen : MonoBehaviour
{
    private void Start()
    {
        GetComponent<UIDocument>().rootVisualElement.Q<Button>().clicked += () => SceneManager.LoadScene(0);
    }
}
