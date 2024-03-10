using UnityEngine;
using UnityEngine.UI;

public class CanvasScaleHelper : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasScaler _canvasScaler;
    [SerializeField] private Vector2 _ratio;
    private float _ratioF;

    private void Start()
    {
        _ratioF = _ratio.x / _ratio.y;
    }

    private void Update()
    {
        float ratio = _rectTransform.rect.width / _rectTransform.rect.height;
        if (ratio > _ratioF)
        {
            _canvasScaler.matchWidthOrHeight = 1;
        }
        else if (ratio < _ratioF)
        {
            _canvasScaler.matchWidthOrHeight = 0;
        }
    }
}
