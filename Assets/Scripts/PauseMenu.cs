using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;
    public CanvasGroup canvasGroup;

    [Header("Fade Settings")]
    public float fadeDuration = 0.4f;
    public float buttonStaggerDelay = 0.05f; // butonlar arasý gecikme

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
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Save()
    {
        Debug.Log("Oyun kaydedildi!");
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        StartCoroutine(FadeCanvas(true));
        Time.timeScale = 0f;
        GameIsPaused = true;

        animator.Play("FadeIn");
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    IEnumerator FadeCanvas(bool fadeIn)
    {
        float start = fadeIn ? 0 : 1;
        float end = fadeIn ? 1 : 0;
        float elapsed = 0f;

        // Eðer açýlýyorsa alpha 0'dan baþla ama aktif kal
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

        // Eðer kapanýyorsa input'u kapat
        if (!fadeIn)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            // fade bittikten sonra buton animasyonlarýný sýrayla baþlat
            //StartCoroutine(AnimateButtons());
        }
    }


    /*IEnumerator AnimateButtons()
    {
        yield return null;
        // butonlarý sýrayla hafif büyüterek içeri getir
        foreach (Transform child in pauseMenuUI.transform)
        {
            if (!child.gameObject.activeSelf) continue;
            Vector3 originalScale = child.localScale;
            child.localScale = Vector3.zero;

            float elapsed = 0f;
            while (elapsed < 0.25f)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / 0.25f;
                child.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
                yield return null;
            }

            child.localScale = originalScale;
            yield return new WaitForSecondsRealtime(buttonStaggerDelay);
        }
    }*/
}
