using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuSlideshow : MonoBehaviour
{
    [Header("Images (In Order)")]
    public Image[] images;

    [Header("Timing")]
    public float fadeDuration = 1.5f;   // Geçiþ süresi
    public float displayTime = 3f;      // Görüntüde kalma süresi

    private void Start()
    {
        // Baþta tüm görselleri görünmez yap
        foreach (var img in images)
        {
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }

        // Ýlk resmi görünür yap
        if (images.Length > 0)
        {
            StartCoroutine(PlaySlideshow());
        }
    }

    IEnumerator PlaySlideshow()
    {
        int index = 0;

        while (true)
        {
            Image current = images[index];
            yield return StartCoroutine(FadeImage(current, 0f, 1f));  // Fade In
            yield return new WaitForSeconds(displayTime);
            yield return StartCoroutine(FadeImage(current, 1f, 0f));  // Fade Out

            index = (index + 1) % images.Length;
        }
    }

    IEnumerator FadeImage(Image img, float start, float end)
    {
        float elapsed = 0f;
        Color c = img.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(start, end, elapsed / fadeDuration);
            img.color = c;
            yield return null;
        }

        c.a = end;
        img.color = c;
    }
}
