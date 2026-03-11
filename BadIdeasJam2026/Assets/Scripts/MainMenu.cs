using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static void Menu()
    {
        SceneManager.LoadScene(0);
    }
    public static void StartCutscene()
    {
        SceneManager.LoadScene(1);
    }
    public static void StartGame()
    {
        SceneManager.LoadScene(2);
    }
    public static void Credits()
    {
        SceneManager.LoadScene(3);
    }
    private void OnConfirm() //working with controller input; kinda messy rn tbh...
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                StartCutscene();
                break;
            case 1:
                StartGame();
                break;
            //case 2: the game scene
            //    break;
            case 3:
                Menu();
                break;
            default:
                break;

        }
    }
}
