using UnityEngine;

public class SkillsDescription : MonoBehaviour
{
    public string[] spreadFireDescriptions = {
        "Description : Enflamme les ennemis dans une zone autour de vous, leur infligeant des dégâts au fil du temps.  Comment l'utiliser : Appuyez sur [E] pour activer le Feu de propagation. Pendant l'activation, approchez-vous des ennemis pour les enflammer et leur infliger des dégâts au fil du temps. Cette capacité a une période de récupération avant de pouvoir être utilisée à nouveau.",
    };
    public string[] lightningRainDescriptions = {
        "Description : Entourez votre personnage d'une tempête de foudre qui se charge au fil du temps. Une fois que toutes les charges sont accumulées, relâchez la capacité pour que des éclairs frappent le sol autour de votre personnage, infligeant des dégâts aux ennemis et les étourdissant pendant un court laps de temps. Comment l'utiliser : Appuyez la touche de chargement de la capacité pour activer la tempête de foudre et maintenez la le plus longtemps possible pour accumuler les dégâts. Relâchez la touche pour lancer la Pluie de Foudre. Cette capacité a une période de récupération avant de pouvoir être utilisée à nouveau.",
    };
    public string[] iceShieldDescriptions = {
        "Description : À l'activation, gêle l'air autour de vous. Pour chaque ennemi à proximité, gagne une pile de bouclier. Chaque pile de bouclier donne des points de vies supplémentaires au joueur. Comment l'utiliser : Appuyez sur [Shift] pour activer le Bouclier de Glace. Cette capacité a une période de récupération avant de pouvoir être utilisée à nouveau.",
    };
    public string[] naturesMelodyDescriptions = {
        "Description : Vous invoquez la mélodie de la nature. Cette dernière vous soigne progressivement jusqu'à ce que vous ayez récupéré l'équivalent de l'entièreté de vos points de vies. Comment l'utiliser : Appuyer sur [R] pour déclancher Mélodie de la Nature et appuyer de nouveau pour l'arrêter. Cette capacité a une période de récupération avant de pouvoir être utilisée à nouveau.",
    };

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