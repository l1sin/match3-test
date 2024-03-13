using System.Runtime.InteropServices;
using UnityEngine;

public class Yandex : MonoBehaviour
{
    public static Yandex Instance;
    public const string Path = "idbfs/MagicShooterSaveDirectory";
    public Localization DefaultLanguage;
    public string Domen;
    public enum Localization
    {
        en,
        ru,
    }

    [DllImport("__Internal")]
    public static extern string GetLanguage();

    [DllImport("__Internal")]
    public static extern void WatchAdCoins();

    [DllImport("__Internal")]
    public static extern void WatchAdExp();

    [DllImport("__Internal")]
    public static extern void SaveExtern(string data);

    [DllImport("__Internal")]
    public static extern void LoadExtern();

    [DllImport("__Internal")]
    public static extern void FullScreenAd();

    [DllImport("__Internal")]
    public static extern string GetPrice(int index);

    [DllImport("__Internal")]
    public static extern void GameReady();

    [DllImport("__Internal")]
    public static extern void ReachGoal(string goal);

    [DllImport("__Internal")]
    public static extern string GetDomen();

    [DllImport("__Internal")]
    public static extern void ConsumePurchase(string token);

    [DllImport("__Internal")]
    public static extern void FindAllPurchases();

    [DllImport("__Internal")]
    public static extern void BuyPurchase(string purchaseId, int purchaseIndex);

    [DllImport("__Internal")]
    public static extern void CheckPurchase(string purchaseId, int purchaseIndex);

    private void Awake()
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
#if UNITY_EDITOR || UNITY_STANDALONE
        EditorInit();
#endif
    }

    public void StartInit()
    {
        string lang = GetLanguage();
        Domen = GetDomen();
        DataController.Instance.LoadAllData(lang);
        LoadExtern();
    }

    public void EditorInit()
    {
        DataController.Instance.LoadAllData(DefaultLanguage.ToString());
        SaveManager.Instance.LoadDataLocal();
    }
}
