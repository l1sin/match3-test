using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    [SerializeField] private AbilityType _abilityType;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _costTMP;
    [SerializeField] private Image _abilityImage;
    [SerializeField] private int _abilityCost;
    private enum AbilityType
    {
        Reshuffle
    }

    private void Start()
    {
        _costTMP.text = $"{_abilityCost}";
        UpdateButton();
    }

    private void UseAbility()
    {
        switch (_abilityType)
        {
            case AbilityType.Reshuffle:
                EntitySpawner.Instance.AbilityReshuffle(_abilityCost);
                break;
            default: break;
        }
    }

    private void UpdateButton()
    {
        
        if (_abilityCost <= LevelController.Instance.TurnsLeft)
        {
            _button.interactable = true;
            _abilityImage.color = _button.colors.normalColor;

            Color textColor = _costTMP.color;
            textColor.a = 1;
            _costTMP.color = textColor;
        }
        else
        {
            _button.interactable = false;
            _abilityImage.color = _button.colors.disabledColor;

            Color textColor = _costTMP.color;
            textColor.a = 0.5f;
            _costTMP.color = textColor;
        } 
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(UseAbility);
        LevelController.Instance.TurnMade += UpdateButton;
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(UseAbility);
        LevelController.Instance.TurnMade -= UpdateButton;
    }
}
