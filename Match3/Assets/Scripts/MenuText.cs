using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class MenuText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _TitleTMP;
    [SerializeField] private float _minFontSize;
    [SerializeField] private float _maxFontSize;
    [SerializeField] private float _fontChangeSpeed;
    [SerializeField] private float _colorChangeSpeed;
    [SerializeField] private Gradient _gradient;

    public void Start()
    {
        
    }

    private void Update()
    {
        ChangeFontSize();
        ChangeColor();
    }

    private void ChangeFontSize()
    {
        float t = Mathf.PingPong(Time.time * _fontChangeSpeed, 1);
        _TitleTMP.fontSize = Mathf.Lerp(_minFontSize, _maxFontSize, t);
    }

    private void ChangeColor()
    {
        float t = Mathf.PingPong(Time.time * _colorChangeSpeed, 1);
        Color c = _gradient.Evaluate(t);
        _TitleTMP.color = c;
    }
}
