using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Player/Settings")]
public class PlayerSettings : ScriptableObject
{
    [System.Serializable]
    public struct Setting
    {
        public float sensitivity, fov, volume;
    }
    public Setting settings;
    public bool serialize;

    // void OnValidate()
    // {
    //     Save();
    // }

    [ContextMenu("Save")]
    public void Save()
    {
        if(!serialize)
            return;

        string setting = JsonUtility.ToJson(settings);
        //Debug.Log(Application.persistentDataPath);
        string path = Application.persistentDataPath + "/" + name + ".txt";
        File.WriteAllText(path, setting);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if(!serialize)
            return;

        string path = Application.persistentDataPath + "/" + name + ".txt";
        if(!File.Exists(path))
            return;

        string setting = File.ReadAllText(path);
        Setting newSettings = JsonUtility.FromJson<Setting>(setting);
        settings = newSettings;
    }
}
