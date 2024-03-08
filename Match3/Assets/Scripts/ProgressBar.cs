using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image _filledImage;

    public void UpdateProgressBar(float current, float max)
    {
        _filledImage.fillAmount = current / max;
    }
}
