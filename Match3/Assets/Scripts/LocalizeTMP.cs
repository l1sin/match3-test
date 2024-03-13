using TMPro;
using UnityEngine;

public class LocalizeTMP : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public int LineID;
    public string AdditionalText;
    public bool LineBreak;

    public void Start()
    {
        Text.text = DataController.Instance.Dictionary[LineID] + AdditionalText;
        if (LineBreak) Text.text = Text.text.Replace(" ", "\n");
    }
}
