using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance;

    [SerializeField] private Transitor _transitor;
    [SerializeField] private LevelButton[] _levelButtons;
    [SerializeField] private SliderController[] _sliders;
    public Sprite[] Stars;

    public void Start()
    {
        LoadSliders();
        LoadLevelButtons();
    }

    private void LoadSliders()
    {
        for (int i = 0; i < _sliders.Length; i++)
        {
            _sliders[i].LoadSlider();
        }
    }

    public void LoadLevel(int level)
    {
        _transitor.TransitOut(level);
    }

    public void SaveCurrentState()
    {
        SaveManager.Instance.SaveData(SaveManager.Instance.CurrentProgress);
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
            if (i >= progress.LevelsComplete - 1) break;
            _levelButtons[i + 1].SetButtonState(true);
        }
    }

    private void Singleton()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void OnEnable()
    {
        Singleton();
    }
}
