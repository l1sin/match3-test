using UnityEngine;
using UnityEngine.UI;

public class CanvasScaleHelper : MonoBehaviour
{
    public RectTransform rect;
    public CanvasScaler scaler;
    private const float defaultRatio = 16f / 9f;

    private void Update()
    {
        float ratio = rect.rect.width / rect.rect.height;
        if (ratio > defaultRatio)
        {
            scaler.matchWidthOrHeight = 1;
        }
        else if (ratio < defaultRatio)
        {
            scaler.matchWidthOrHeight = 0;
        }
    }
}
