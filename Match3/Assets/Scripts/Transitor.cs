using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Transitor : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private float _transitionTime;

    public void Start()
    {
        TransitIn();
    }

    public void TransitIn()
    {
        _image.color = Color.black;
        _image.DOColor(Color.clear, _transitionTime).OnComplete(() => _image.enabled = false);
    }

    public void TransitOut(int level)
    {
        _image.enabled = true;
        _image.DOColor(Color.black, _transitionTime).OnComplete(() => LoadLevel(level));
    }

    public static void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
}
