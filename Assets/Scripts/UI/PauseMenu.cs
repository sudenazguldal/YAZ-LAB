using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Esper.ESave;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;
    public CanvasGroup canvasGroup;

    [Header("Fade Settings")]
    public float fadeDuration = 0.4f;
    public float buttonStaggerDelay = 0.05f; // butonlar arası gecikme icin

    public static bool GameIsPaused = false;
    public Animator animator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        StartCoroutine(FadeCanvas(false));
        Time.timeScale = 1f;
        GameIsPaused = false;
        animator.Play("FadeOut");

        //  İmleci gizler ve ortada kilitler
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Restart()
    {
        FileDeleter.DeleteMainSave();

        Time.timeScale = 1f;
        // Mevcut sahneyi yeniden yükler
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }


    public void Save()
    {
        Debug.Log("Oyun kaydedildi!");
        var player = GameObject.FindWithTag("Player");
        if (player) player.GetComponent<PlayerSaveData>()?.SaveGame();
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        StartCoroutine(FadeCanvas(true));
        Time.timeScale = 0f;
        GameIsPaused = true;
        animator.Play("FadeIn");

        // İmleci görünür yapar ve serbest bırakır
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    IEnumerator FadeCanvas(bool fadeIn)
    {
        float start = fadeIn ? 0 : 1;
        float end = fadeIn ? 1 : 0;
        float elapsed = 0f;

      
        canvasGroup.alpha = start;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        canvasGroup.alpha = end;

       
        if (!fadeIn)
        {
            //inputu kapar
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else { }
    }

}
