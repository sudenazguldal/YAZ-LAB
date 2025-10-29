using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ContinueGame()
    {
        Debug.Log("Continue pressed");
        SceneManager.LoadScene("GameScene"); // Gerçek oyun sahnenin adýný yaz
    }

    public void NewGame()
    {
        Debug.Log("New Game pressed");
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings opened");
        // Buraya ayrý bir Settings paneli açabilirsin
    }

    public void ExitGame()
    {
        Debug.Log("Exit pressed");
        Application.Quit();
    }
}
