using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SaveService 
{
    private static string FilePath => Application.persistentDataPath + "/save.json";
    public static SaveData SaveData { get; set; }

    public static void Save()
    {
        string json = JsonUtility.ToJson(SaveData, true);
        File.WriteAllText(FilePath, json);
        
        Debug.Log("Saved data!");
    }

    public static void Load()
    {
        if (!File.Exists(FilePath))
        {
            Debug.LogWarning("No save file");
            SaveData = new SaveData();
            return;
        }
        
        string json = File.ReadAllText(FilePath);
        if (!string.IsNullOrEmpty(json))
        {
            SaveData = JsonUtility.FromJson<SaveData>(json);
           
            Debug.Log("Loaded data!");
            Debug.Log($"Previous party: {string.Join(",", SaveData.PreviousPlayerParty.Select(p => p.Name))}");
        }
        else SaveData = new SaveData();
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Delete Save")]
    public static void DeleteSave()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
            Debug.Log($"Save deleted: {FilePath}");
        }
        else
        {
            Debug.LogWarning("Save file not found");
        }
    }
#endif
}

[Serializable]
public class SaveData
{
    public int ScreenSetting = 0;
    public float SoundsSetting = 0.7f;
    public float MusicSetting = 0.7f;

    public List<SerializeUnit> PreviousPlayerParty = new List<SerializeUnit>();
    public SerializeUnit PreviousPlayer;
}
