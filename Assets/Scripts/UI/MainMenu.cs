using Esper.ESave;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject CanvasMainMenu;    
    public GameObject CanvasSettings;    
    public GameObject SettingsPanel;     


    void Start()
    {
        // Oyun her zaman ana menüden başlasın
        if (CanvasMainMenu != null) CanvasMainMenu.SetActive(true);
        if (CanvasSettings != null) CanvasSettings.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void ContinueGame()
    {
        if (SaveStorage.instance == null)
        {
            SceneManager.LoadScene("MainScene");
            return;
        }

        var saveFile = SaveStorage.instance.GetSaveByFileName("MainSave");
        string savePath = Path.Combine(Application.persistentDataPath, "YAZ-LAB", "MainSave.json");

        if (saveFile == null || !File.Exists(savePath))
        {
            
            SceneManager.LoadScene("MainScene");
            return;
        }

        if (saveFile.HasData("SceneName"))
        {
            saveFile.Load();
            string lastScene = saveFile.GetData<string>("SceneName");
            
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            
            SceneManager.LoadScene("MainScene");
        }
    }

    public void NewGame()
    {
        FileDeleter.DeleteMainSave();

        if (SaveStorage.instance != null)
        {
            var residual = SaveStorage.instance.GetSaveByFileName("MainSave");
            if (residual != null)
                residual.EmptyFile();
        }

        SceneManager.LoadScene("MainScene");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings opened");

        //  Ana menüyü gizler
        if (CanvasMainMenu != null)
            CanvasMainMenu.SetActive(false);

        //  Settings menüsünü göster
        if (CanvasSettings != null)
            CanvasSettings.SetActive(true);

        if (SettingsPanel != null)
            SettingsPanel.SetActive(true);
    }

    public void ExitGame()
    {
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
