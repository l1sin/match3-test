using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public Progress CurrentProgress;
    public int MainMenuSceneIndex;
    public KeyCode SaveKey;
    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void LoadDataLocal()
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
            SaveData(progress);
            Debug.Log("File do not exists. Creating save file");
        }
        SceneManager.LoadScene(MainMenuSceneIndex);
    }

    public void LoadDataCloud(string json)
    {
        Progress progress;
        if (json != null)
        {
            progress = JsonUtility.FromJson<Progress>(json);
            CurrentProgress = progress;
            Debug.Log($"Loaded from Cloud\n{json}");
        }
        else
        {
            progress = new Progress();
            SaveData(progress);
            Debug.Log("File do not exists. Creating save file");
        }
        SceneManager.LoadScene(MainMenuSceneIndex);
    }

    public void SaveData(Progress progress)
    {
        SaveDataLocal(progress);
#if UNITY_EDITOR || UNITY_STANDALONE
        Debug.Log("Fake CloudSave");
#elif UNITY_WEBGL
        SaveDataCloud(progress);
#endif
    }

    public void SaveDataLocal(Progress progress)
    {
        CurrentProgress = progress;
        string json = JsonUtility.ToJson(progress);
        PlayerPrefs.SetString("save", json);
        PlayerPrefs.Save();
        Debug.Log($"Local save to PlayerPrefs");
    }

    public void SaveDataCloud(Progress progress)
    {
        CurrentProgress = progress;
        string json = JsonUtility.ToJson(progress);
        Yandex.SaveExtern(json);
        Debug.Log($"Cloud save");
    }
}