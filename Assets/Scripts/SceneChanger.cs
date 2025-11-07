using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public void ExitToMainMenu()
    {
       
        StartCoroutine(LoadMainMenuAsync());
    }

    private IEnumerator LoadMainMenuAsync()
    {
        //  Oyunu normal hızına döndür
        Time.timeScale = 1f;

        // 2 Cursor aktif
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3 Asenkron sahne yükleme başlat
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMenuScene");

        //  Sahne yüklenene kadar bekle 
        while (!asyncLoad.isDone)
        {
            yield return null; 
        }
    }
}

