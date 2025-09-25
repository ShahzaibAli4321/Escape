using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string folder = Application.persistentDataPath + "/saves/";

    public static void Save(GameData data, string fileName)
    {
        Debug.Log("Save path: " + Application.persistentDataPath);

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        string path = folder + fileName + ".json";
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static GameData Load(string fileName)
    {
        string path = folder + fileName + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<GameData>(json);
        }
        return null;
    }

    public static void DeleteAllSaves()
    {
        if (Directory.Exists(folder))
        {
            Directory.Delete(folder, true); // true deletes all files/subdirectories
        }
    }
}
