using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private int _level;
    [SerializeField] private TextMeshProUGUI _levelNumberTMP;
    [SerializeField] private Image[] _starImages;
    [SerializeField] private Button _button;

    public void UpdateData(int levelRating)
    {
        _levelNumberTMP.text = $"{_level}";
        for (int i = 0; i < levelRating; i++)
        {
            _starImages[i].sprite = MenuController.Instance.Stars[1];
        }
    }

    private void LoadLevel()
    {
        MenuController.Instance.LoadLevel(_level);
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(LoadLevel);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }
}
