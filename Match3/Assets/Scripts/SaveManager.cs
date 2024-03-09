using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public Progress CurrentProgress;

    private void Start()
    {
        Singleton();
        LoadData();
    }

    public void LoadData()
    {
        Progress progress;
        if (PlayerPrefs.HasKey("save"))
        {
            string json = PlayerPrefs.GetString("save");
            progress = JsonUtility.FromJson<Progress>(json);
            CurrentProgress = progress;
            Debug.Log($"Loaded from PlayerPrefs:\n{json}");
        }
        else
        {
            progress = new Progress();
            Debug.Log("File does not exist. Creating save file");
        }
        SaveData(progress);
    }

    public void SaveData(Progress progress)
    {
        CurrentProgress = progress;
        string json = JsonUtility.ToJson(progress);
        PlayerPrefs.SetString("save", json);
        PlayerPrefs.Save();
    }

    private void Singleton()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}