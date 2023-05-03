using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class SkillsUpgradesInformations : MonoBehaviour
{
    string[] fireSkillNames = new string[]
    {
        "Feu de propagation II",
        "Feu de propagation III",
    };
    string[] fireSkillDescriptions= new string[]
    {
        "Ajoute un effet supplémentaire à Feu de propagation [E] qui fait que les cibles affectée par Feu de propagation [E] reçoivent des dégâts amplifié de 30% ",
        "Feu de propagation peut maintenant accumuler jusqu'à 3 charges sur les cibles si l'effet est utiliser à plusieurs reprise sur la même cible avant que la durée de l'effet ne se soit achevée",
    };

    string[] lightningSkillNames = new string[]
    {
        "Pluie de foudre II",
        "Pluie de foudre III",
    };
    string[] lightningSkillDescriptions = new string[]
    {
        "Pluie de foudre II description",
        "Pluie de foudre III description",
    };

    string[] iceSkillNames = new string[]
    {
        "Bouclier de glace II",
        "Bouclier de glace III",
    };
    string[] iceSkillDescriptions = new string[]
    {
        "Bouclier de glace II description",
        "Bouclier de glace III description",
    };

    string[] natureSkillNames = new string[]
    {
        "Mélodie de la nature II",
        "Mélodie de la nature III",
    };
    string[] natureSkillDescriptions = new string[]
    {
        "Mélodie de la nature II description",
        "Mélodie de la nature III description",
    };

    public string[] GetSkillsInformations(PlayerAbilityEnum ability, int levelIndex)
    {
        Debug.Log(levelIndex + "index");
        string[] skillInfos = new string[2];
        switch (ability)
        {
            case PlayerAbilityEnum.SpreadFire :
            skillInfos[0] = fireSkillNames[levelIndex];
            skillInfos[1] = fireSkillDescriptions[levelIndex];
            break;

            case PlayerAbilityEnum.LightningRain :
            skillInfos[0] = lightningSkillNames[levelIndex];
            skillInfos[1] = lightningSkillDescriptions[levelIndex];
            break;

            case PlayerAbilityEnum.IceShield :
            skillInfos[0] = iceSkillNames[levelIndex];
            skillInfos[1] = iceSkillDescriptions[levelIndex];
            break;

            case PlayerAbilityEnum.NaturesMelody :
            skillInfos[0] = natureSkillNames[levelIndex];
            skillInfos[1] = natureSkillDescriptions[levelIndex];
            break;

            default:
            break;
        }

        return skillInfos;
    }
}
