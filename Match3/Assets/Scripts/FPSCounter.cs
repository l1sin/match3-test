using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private float time;
    private float frames;
    void Update()
    {
        CountFPS();
    }

    private void CountFPS()
    {
        time += Time.deltaTime;
        frames++;
        if (time >= 1f)
        {
            _text.text = (1 / Time.deltaTime).ToString();
            frames = 0;
            time = 0;
        }
    }
}
