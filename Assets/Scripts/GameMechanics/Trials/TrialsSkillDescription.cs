using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrialsSkillDescription : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _descriptionText;
    public void ToggleActivity(bool active, PlayerAbilityEnum orbAbility)
    {
        Debug.Log(orbAbility);
        gameObject.SetActive(active);
        if(orbAbility != PlayerAbilityEnum.None) 
        {
            if(_descriptionText.text == "") _descriptionText.text = SkillsDescription.instance.GetSkillDescription(orbAbility, 0);
        }
    }
}
