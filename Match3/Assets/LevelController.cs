using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;

    [SerializeField] private int _turnsLeft;
    [SerializeField] private int _targetScore;
    [SerializeField] private int _playerScore;
    [SerializeField] private ProgressBar _progressBar;
    [SerializeField] private TextMeshProUGUI _targetScoreText;
    [SerializeField] private TextMeshProUGUI _leftToScoreText;
    [SerializeField] private TextMeshProUGUI _turnsLeftText;

    private void Start()
    {
        Singleton();
        Init();
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
    }

    private void Init()
    {
        UpdateTurns();
        UpdatePoints();
    }

    private void UpdateTurns()
    {
        _turnsLeftText.text = $"Turns left: {_turnsLeft}";
    }

    private void UpdatePoints()
    {
        int scoreLeft = Mathf.Max(_targetScore - _playerScore, 0);
        _leftToScoreText.text = $"Left to score: {scoreLeft}";
        _targetScoreText.text = $"Target: {_targetScore}";
        _progressBar.UpdateProgressBar(_playerScore, _targetScore);
    }

    private void Singleton()
    {
        if (Instance != null && Instance != this) Destroy(Instance.gameObject);
        else Instance = this;
    }
}
