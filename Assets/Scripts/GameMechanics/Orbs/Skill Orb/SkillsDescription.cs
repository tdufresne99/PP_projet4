using UnityEngine;

public class SkillsDescription : MonoBehaviour
{
    public string[] spreadFireDescriptions;
    public string[] lightningRainDescriptions;
    public string[] iceShieldDescriptions;
    public string[] naturesMelodyDescriptions;

    private static SkillsDescription _instance;
    public static SkillsDescription instance { get => _instance; set => _instance = value; }
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);
    }

    public string GetSkillDescription(PlayerAbilityEnum ability, int abilityLevel)
    {
        string skillDescription = "";
        switch (ability)
        {
            case PlayerAbilityEnum.None:
                break;

            case PlayerAbilityEnum.SpreadFire:
                skillDescription = spreadFireDescriptions[abilityLevel];
                break;

            case PlayerAbilityEnum.LightningRain:
                skillDescription = lightningRainDescriptions[abilityLevel];
                break;

            case PlayerAbilityEnum.IceShield:
                skillDescription = iceShieldDescriptions[abilityLevel];
                break;

            case PlayerAbilityEnum.NaturesMelody:
                skillDescription = naturesMelodyDescriptions[abilityLevel];
                break;

            default:
                break;
        }
        return skillDescription;
    }
}