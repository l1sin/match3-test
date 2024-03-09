using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance;

    [SerializeField] private Transitor _transitor;
    [SerializeField] private LevelButton[] _levelButtons;
    public Sprite[] Stars;

    public void Start()
    {
        Singleton();
        LoadLevelButtons();
    }

    public void LoadLevel(int level)
    {
        _transitor.TransitOut(level);
    }

    private void LoadLevelButtons()
    {
        Progress progress = SaveManager.Instance.CurrentProgress;
        for (int i = 0; i < _levelButtons.Length; i++)
        {
            LevelInfo li = progress.LevelData[i];

            _levelButtons[i].UpdateData(li.Stars);
        }

        _levelButtons[0].SetButtonState(true);
        for (int i = 0; i < progress.LevelsComplete; i++)
        {
            if (i >= _levelButtons.Length) break;
            _levelButtons[i + 1].SetButtonState(true);
        }
    }

    private void Singleton()
    {
        if (Instance != null && Instance != this) Destroy(Instance.gameObject);
        else Instance = this;
    }
}
