using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;       

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
       

        // Oyun durdurulsun
        Time.timeScale = 0f;

        // Cursor aktif olsun
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


   

    private IEnumerator EndGameSequence(GameObject panelToActivate)
    {
        isGameOver = true;

        

        if (panelToActivate != null)
        {
            panelToActivate.SetActive(true);
        }


        UnityEngine.EventSystems.EventSystem.current.enabled = true;

        yield break;
    }

}