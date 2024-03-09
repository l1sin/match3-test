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
        for (int i = 0; i < _levelButtons.Length; i++)
        {
            _levelButtons[i].UpdateData(0);
        }
    }

    private void Singleton()
    {
        if (Instance != null && Instance != this) Destroy(Instance.gameObject);
        else Instance = this;
    }
}
