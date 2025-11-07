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
    public float buttonStaggerDelay = 0.05f; // butonlar arası gecikme

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

        //  İmleci gizle ve ortada kilitle
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Restart()
    {// Kayıt dosyasını sil (FileDeleter.cs görüntüsüne göre bu metot doğru yolu siliyor)
        FileDeleter.DeleteMainSave();

        Time.timeScale = 1f;
        // Mevcut sahneyi yeniden yükle
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

        // 🔹 İmleci görünür ve serbest yap
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    IEnumerator FadeCanvas(bool fadeIn)
    {
        float start = fadeIn ? 0 : 1;
        float end = fadeIn ? 1 : 0;
        float elapsed = 0f;

        // Eğer açılıyorsa alpha 0'dan başla ama aktif kal
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

        // Eğer kapanıyorsa input'u kapat
        if (!fadeIn)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else{}
    }

}
