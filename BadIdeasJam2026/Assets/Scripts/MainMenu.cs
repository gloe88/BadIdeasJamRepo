using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static void StartCutscene()
    {
        SceneManager.LoadScene(1);
    }
    public static void StartGame()
    {
        SceneManager.LoadScene(2);
    }
}
