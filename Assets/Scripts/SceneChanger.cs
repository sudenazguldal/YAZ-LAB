using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public void ExitToMainMenu()
    {
        Debug.Log("Ana menüye dönülüyor...");
        StartCoroutine(LoadMainMenuAsync());
    }

    private IEnumerator LoadMainMenuAsync()
    {
        // 1️⃣ Oyunu normal hızına döndür
        Time.timeScale = 1f;

        // 2️⃣ Cursor aktif
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3️⃣ Asenkron sahne yükleme başlat
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMenuScene");

        // 4️⃣ Sahne yüklenene kadar bekle (Realtime!)
        while (!asyncLoad.isDone)
        {
            yield return null; // bir frame bekle
        }
    }
}

