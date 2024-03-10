using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;

    public int TurnsLeft;
    [SerializeField] private int _targetScore;
    [SerializeField] private int _playerScore;
    [SerializeField] private ProgressBar _progressBar;
    [SerializeField] private TextMeshProUGUI _targetScoreTMP;
    [SerializeField] private TextMeshProUGUI _playerScoreTMP;
    [SerializeField] private TextMeshProUGUI _turnsLeftTMP;

    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;

    [SerializeField] private Image[] _starImages;
    [SerializeField] private Sprite[] _starSprites;
    [SerializeField] private TextMeshProUGUI _winTMP;
    [SerializeField] private string[] _winText;
    [SerializeField] private float[] _completionPercent;

    [SerializeField] private Transitor _transitor;

    [SerializeField] private float _endLevelDelay = 0.5f;
    private int _levelIndex;
    public event Action TurnMade;

    private void Start()
    {
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
        Save(stars);
    }

    private void Save(int stars)
    {
        Progress progress = SaveManager.Instance.CurrentProgress;
        if (_levelIndex > progress.LevelsComplete) progress.LevelsComplete = _levelIndex;

        LevelInfo info = progress.LevelData[_levelIndex - 1];

        if (stars > info.Stars) info.Stars = stars;
        if (_playerScore > info.HighScore) info.HighScore = _playerScore;

        progress.LevelData[_levelIndex - 1] = info;

        SaveManager.Instance.SaveData(progress);
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

    public void DoTurn(int cost)
    {
        TurnsLeft -= cost;
        UpdateTurns();
        TurnMade?.Invoke();
    }

    public void CheckIfLevelEnd()
    {
        if (TurnsLeft <= 0) StartCoroutine(EndLevel());
    }

    private IEnumerator EndLevel()
    {
        yield return new WaitForSeconds(_endLevelDelay);
        CalculateCompletion();
    }

    public void CalculateCompletion()
    {
        float completion = (float)_playerScore / (float)_targetScore;
        if (completion < _completionPercent[0]) TriggerLose();
        else if (completion < _completionPercent[1]) TriggerWin(1);
        else if (completion < _completionPercent[2]) TriggerWin(2);
        else TriggerWin(3);
    }

    private void Init()
    {
        UpdateTurns();
        UpdatePoints();
        _levelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void UpdateTurns()
    {
        _turnsLeftTMP.text = $"Turns left: {TurnsLeft}";
    }

    private void UpdatePoints()
    {
        _playerScoreTMP.text = $"Score: {_playerScore}";
        _targetScoreTMP.text = $"Target: {_targetScore}";
        _progressBar.UpdateProgressBar(_playerScore, _targetScore);
    }

    private void Singleton()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void LoadLevel(int level)
    {
        _transitor.TransitOut(level);
    }

    public void ReloadLevel()
    {
        _transitor.TransitOut(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnEnable()
    {
        Singleton();
    }
}
