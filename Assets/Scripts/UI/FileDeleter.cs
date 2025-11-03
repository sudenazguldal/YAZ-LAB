// FileDeleter.cs

using UnityEngine;
using System.IO;
using Esper.ESave;

public static class FileDeleter
{
    public static void DeleteMainSave()
    {
        // 1. Dosya yolunu oluştur ve diskten sil (YAZ-LAB klasörü dahil)
        string path = Path.Combine(Application.persistentDataPath, "YAZ-LAB", "MainSave.json");

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Kayıt dosyası başarıyla silindi: " + path);
        }

        // 2. 🎯 KRİTİK DÜZELTME: SaveFile objesini SaveStorage'dan kaldır
        if (SaveStorage.instance != null)
        {
            // SaveFile objesini isme göre çek
            var saveFileToRemove = SaveStorage.instance.GetSaveByFileName("MainSave");

            if (saveFileToRemove != null)
            {
                // API DÜZELTMESİ: Obje referansı ile silme.
                SaveStorage.instance.RemoveSave(saveFileToRemove);

                // Ayrıca, içindeki veriyi de temizleyebiliriz (opsiyonel ama güvenli)
                saveFileToRemove.EmptyFile();

                Debug.Log("MainSave SaveFile objesi hafızadan silindi ve temizlendi.");
            }
        }
    }
}