using Esper.ESave;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ContinueGame()
    {
        if (SaveStorage.instance == null)
        {
            SceneManager.LoadScene("MainScene");
            return;
        }

        var saveFile = SaveStorage.instance.GetSaveByFileName("MainSave");
        string savePath = Path.Combine(Application.persistentDataPath, "YAZ-LAB", "MainSave.json");

        // 1. SaveFile objesi hafızada yoksa veya diskte dosya yoksa New Game akışını başlat
        if (saveFile == null || !File.Exists(savePath))
        {
            Debug.Log("MainMenu: Kayıt dosyası bulunamadı veya SaveFile objesi yok. Yeni oyun başlatılıyor.");
            SceneManager.LoadScene("MainScene");
            return;
        }

        // 2. Eğer SaveFile varsa, verinin olup olmadığını kontrol et
        if (saveFile.HasData("SceneName"))
        {
            // Önce SaveFile'ı diskten yükle (içindeki verileri tazele)
            saveFile.Load();

            string lastScene = saveFile.GetData<string>("SceneName");
            Debug.Log("MainMenu: Kayıtlı sahneye yükleniyor: " + lastScene);
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            // Kayıt dosyası var ama SceneName verisi yok (bozuk veya eski kayıttır)
            Debug.Log("MainMenu: Kayıt dosyası var ancak SceneName verisi eksik. Yeni oyun başlatılıyor.");
            SceneManager.LoadScene("MainScene");
        }
    }


    public void NewGame()
    {
        // 1. Dosyayı diskten ve SaveStorage'dan silme girişimi
        // Bu, FileDeleter.cs içindeki güncellediğimiz metot olmalı.
        FileDeleter.DeleteMainSave();

        // 2. 🎯 KRİTİK DÜZELTME: Garanti Temizlik
        // FileDeleter, SaveStorage'dan objeyi kaldıramadıysa diye, in-memory SaveFile objesinin içini boşalt.
        if (SaveStorage.instance != null)
        {
            var residualSaveFile = SaveStorage.instance.GetSaveByFileName("MainSave");
            if (residualSaveFile != null)
            {
                // SaveFile.cs içinde tanımlı olan EmptyFile() metodu, 
                // iç veriyi (saveData.Clear()) temizler ve diske boş bir dosya yazar.
                // Bu, LoadGame'in eski veriyi görmesini imkansız hale getirir.
                residualSaveFile.EmptyFile();
                Debug.Log("New Game: SaveFile objesinin verisi hafızadan temizlendi ve diske boş dosya yazıldı.");
            }
        }

        // 3. Ana Sahneyi Yükle
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }
    public void OpenSettings()
    {
        Debug.Log("Settings opened");
        // Buraya ayrı bir Settings paneli açabilirsin
    }

    public void ExitGame()
    {
        Debug.Log("The game is closing...");
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

}
