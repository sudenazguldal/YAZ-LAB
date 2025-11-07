// FileDeleter.cs

using UnityEngine;
using System.IO;
using Esper.ESave;

public static class FileDeleter
{
    public static void DeleteMainSave()
    {
        
        string path = Path.Combine(Application.persistentDataPath, "YAZ-LAB", "MainSave.json");

        if (File.Exists(path))
        {
            File.Delete(path);
           
        }

        
        if (SaveStorage.instance != null)
        {
            // SaveFile objesini isme göre çek
            var saveFileToRemove = SaveStorage.instance.GetSaveByFileName("MainSave");

            if (saveFileToRemove != null)
            {
             
                SaveStorage.instance.RemoveSave(saveFileToRemove);

               
                saveFileToRemove.EmptyFile();

             
            }
        }
    }
}