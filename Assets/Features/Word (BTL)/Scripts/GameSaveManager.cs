
using UnityEngine;
using System.IO;
using System;

[Serializable]
public class GameSaveData
{
    public string typedTextAll;
    public string generatedText;

    public int currentCharInPage;
    public int currentPage;

    public int totalChar;
    public int totalWords;

    public int readDataIndex;
    public int fileIndex;

    public bool isKeyA;

    public float stressValue;
    public float progressValue;
}

public class GameSaveManager : MonoBehaviour
{
    private string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            "game_save.json"
        );

    public void Save(GameSaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);

            Debug.Log("Game saved: " + SavePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Save failed: " + e.Message);
        }
    }

    public GameSaveData Load()
    {
        if (!File.Exists(SavePath))
            return null;

        try
        {
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<GameSaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError("Load failed: " + e.Message);
            return null;
        }
    }

    public bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }
}
