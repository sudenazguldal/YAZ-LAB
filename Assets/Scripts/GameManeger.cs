using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yüklemek için
using System.Collections;         //IEnumerator için

public class GameManager : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;

    public float panelDisplayTime = 5f;

    private bool isGameOver = false;

    void Start()
    {
        Time.timeScale = 1f;
        if (winPanel != null && losePanel != null)
        {
            winPanel.SetActive(false);
            losePanel.SetActive(false);
        }
        isGameOver = false;
    }

    public void PlayerWin()
    {
        if (isGameOver)
            return;

        StartCoroutine(EndGameSequence(winPanel));
        Debug.Log("Oyun bitti! Kazanma sırası başlatıldı.");
        // Oyun durdurulsun
        Time.timeScale = 0f;

        // Cursor aktif olsun
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void PlayerLose()
    {
        if (isGameOver)
            return;

        StartCoroutine(EndGameSequence(losePanel));
        Debug.Log("Oyun bitti! Kaybetme sırası başlatıldı.");

        // Oyun durdurulsun
        Time.timeScale = 0f;

        // Cursor aktif olsun
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    /*private IEnumerator EndGameSequence(GameObject panelToActivate)
    {
        isGameOver = true;
        Time.timeScale = 0f;

        // 2. Paneli Aktif Et
        if (panelToActivate != null)
        {
            panelToActivate.SetActive(true);

            yield return new WaitForSecondsRealtime(panelDisplayTime);

            Time.timeScale = 1f; //oyunu çöz.

            SceneManager.LoadScene(MainMenuSceneName);
        }
    }*/

    private IEnumerator EndGameSequence(GameObject panelToActivate)
    {
        isGameOver = true;

        // 🎯 Sadece player ve düşmanları durdur
        

        if (panelToActivate != null)
        {
            panelToActivate.SetActive(true);
        }

        // ✅ EventSystem çalışmaya devam etsin
        UnityEngine.EventSystems.EventSystem.current.enabled = true;

        yield break;
    }

}