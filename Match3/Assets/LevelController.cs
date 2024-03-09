using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;

    [SerializeField] private int _turnsLeft;
    [SerializeField] private int _targetScore;
    [SerializeField] private int _playerScore;
    [SerializeField] private ProgressBar _progressBar;
    [SerializeField] private TextMeshProUGUI _targetScoreTMP;
    [SerializeField] private TextMeshProUGUI _leftToScoreTMP;
    [SerializeField] private TextMeshProUGUI _turnsLeftTMP;

    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;

    [SerializeField] private Image[] _starImages;
    [SerializeField] private Sprite[] _starSprites;
    [SerializeField] private TextMeshProUGUI _winTMP;
    [SerializeField] private string[] _winText;
    [SerializeField] private float[] _completionPercent;
    
    

    private void Start()
    {
        Singleton();
        Init();
    }

    public void TriggerWin(int stars)
    {
        _winScreen.SetActive(true);
        for (int i = 0; i < stars; i++)
        {
            _starImages[i].sprite = _starSprites[1];
        }
        _winTMP.text = _winText[stars - 1];
    }

    public void TriggerLose()
    {
        _loseScreen.SetActive(true);
    }

    public void GainScore()
    {
        _playerScore++;
        UpdatePoints();
    }

    public void DoTurn()
    {
        _turnsLeft--;
        UpdateTurns();
        if (_turnsLeft <= 0)
        {
            CalculateCompletion();
        }
    }

    public void CalculateCompletion()
    {
        float completion = _playerScore / _targetScore;
        if (completion < _completionPercent[0]) TriggerLose();
        else if (completion < _completionPercent[1]) TriggerWin(1);
        else if (completion < _completionPercent[2]) TriggerWin(2);
        else TriggerWin(3);
    }

    private void Init()
    {
        UpdateTurns();
        UpdatePoints();
    }

    private void UpdateTurns()
    {
        _turnsLeftTMP.text = $"Turns left: {_turnsLeft}";
    }

    private void UpdatePoints()
    {
        int scoreLeft = Mathf.Max(_targetScore - _playerScore, 0);
        _leftToScoreTMP.text = $"Left to score: {scoreLeft}";
        _targetScoreTMP.text = $"Target: {_targetScore}";
        _progressBar.UpdateProgressBar(_playerScore, _targetScore);
        if (scoreLeft == 0) TriggerWin(3);
    }

    private void Singleton()
    {
        if (Instance != null && Instance != this) Destroy(Instance.gameObject);
        else Instance = this;
    }
}
