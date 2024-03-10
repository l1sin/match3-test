using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SubMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] _UiElements;
    [SerializeField] private float _transitionTime;

    public void OpenSubMenu()
    {
        for (int i = 0; i < _UiElements.Length; i++)
        {
            _UiElements[i].transform.localScale = Vector3.zero;
            _UiElements[i].transform.DOScale(1, _transitionTime);
        }
    }

    public void CloseSubMenu()
    {
        for (int i = 0; i < _UiElements.Length; i++)
        {
            _UiElements[i].transform.localScale = Vector3.one;
            _UiElements[i].transform.DOScale(0, _transitionTime);
        }
    }

    public void Transit(SubMenu sm)
    {
        CloseSubMenu();
        StartCoroutine(Wait(sm));
    }

    private IEnumerator Wait(SubMenu sm)
    {
        yield return new WaitForSeconds(_transitionTime);
        gameObject.SetActive(false);
        sm.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        OpenSubMenu();
    }
}
