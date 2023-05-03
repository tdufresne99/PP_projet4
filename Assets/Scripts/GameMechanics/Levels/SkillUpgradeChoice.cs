using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillUpgradeChoice : MonoBehaviour
{
    [SerializeField] private Color _spreadFireColor;
    [SerializeField] private Color _lightningRainColor;
    [SerializeField] private Color _iceShieldColor;
    [SerializeField] private Color _naturesMelodyColor;
    [SerializeField] private GameObject[] _icons;
    [SerializeField] private Image _button;
    [SerializeField] private GameObject _iconHolder;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    private PlayerAbilityEnum choiceAbility;

    public void DisplaySkillUpgradeInformation(PlayerAbilityEnum ability, string title, string description)
    {
        choiceAbility = ability;

        GameObject icon;
        Color backgroundColor;
        switch (ability)
        {
            case PlayerAbilityEnum.SpreadFire :
                icon = _icons[0];
                backgroundColor = _spreadFireColor;
                break;

            case PlayerAbilityEnum.LightningRain :
                icon = _icons[1];
                backgroundColor = _lightningRainColor;
                break;

            case PlayerAbilityEnum.IceShield :
                icon = _icons[2];
                backgroundColor = _iceShieldColor;
                break;

            case PlayerAbilityEnum.NaturesMelody :
                icon = _icons[3];
                backgroundColor = _naturesMelodyColor;
                break;

            default:
                icon = _icons[0];
                backgroundColor = Color.black;
                break;
        }

        var iconGO = Instantiate(icon, _iconHolder.transform.position, Quaternion.identity, _iconHolder.transform);

        _button.color = backgroundColor;

        _title.text = title;
        _title.color = backgroundColor;

        _description.text = description;
    }

    public void OnSkillUpChosen()
    {
        LevelManager.instance.OnSkillUpChosen(choiceAbility);
    }

}
