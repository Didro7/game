
using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public static string FilePath =>
        Path.Combine(
            Application.persistentDataPath,
            "save.json"
        );

    public static void Save(SaveData data)
    {
        try
        {
            string json =
                JsonUtility.ToJson(data, true);

            File.WriteAllText(FilePath, json);

            Debug.Log("Game saved: " + FilePath);
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Save failed: " + e.Message
            );
        }
    }

    public static SaveData Load()
    {
        if (!File.Exists(FilePath))
        {
            Debug.Log("No save found. Using defaults.");
            return new SaveData();
        }

        try
        {
            string json =
                File.ReadAllText(FilePath);

            if (string.IsNullOrWhiteSpace(json) ||
                !json.TrimStart().StartsWith("{") ||
                !json.TrimEnd().EndsWith("}") ||
                !json.Contains("\"version\"") ||
                !json.Contains("\"unlockedLevels\""))
            {
                throw new Exception("Invalid save format");
            }

            SaveData data =
                JsonUtility.FromJson<SaveData>(json);

            if (data == null ||
                data.version != 1 ||
                data.unlockedLevels == null ||
                data.unlockedLevels.Count == 0)
            {
                throw new Exception("Invalid save data");
            }

            if (!data.unlockedLevels.Contains(1))
                data.unlockedLevels.Add(1);

            Debug.Log("Game loaded successfully!");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogWarning(
                "Save corrupted. Loading defaults: " +
                e.Message
            );

            return new SaveData();
        }
    }

    public static bool Delete()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }

            Debug.Log("Save file deleted!");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Failed to delete save: " + e.Message
            );

            return false;
        }
    }

}
