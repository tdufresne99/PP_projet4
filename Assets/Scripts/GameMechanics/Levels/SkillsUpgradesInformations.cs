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
        "Les dégâts reçus de toutes sources par les ennemis affectés par Feu de propagation [E] sont amplifiés de 30%.",
        "Feu de propagation peut désormais accumuler jusqu'à 3 charges si l'effet est utilisé plusieurs fois sur la même cible avant que la durée de l'effet ne se termine.",
    };

    string[] lightningSkillNames = new string[]
    {
        "Pluie de foudre II",
        "Pluie de foudre III",
    };
    string[] lightningSkillDescriptions = new string[]
    {
        "Pour chaque ennemi touché par Pluie de foudre [Q], vous obtenez un bonus de dégâts de 10% pendant 10 secondes.",
        "Augmente la durée de l'effet d'étourdissement sur les cibles touchées par Pluie de foudre [Q] de 3 secondes à 5 secondes.",
    };

    string[] iceSkillNames = new string[]
    {
        "Bouclier de glace II",
        "Bouclier de glace III",
    };
    string[] iceSkillDescriptions = new string[]
    {
        "Le temps de chargement de Bouclier de glace [Shift] est réduit de 10% par ennemi à proximité lorsque l'habileté est utilisée.",
        "Les ennemis à proximités lorsque Bouclier de glace [Shift] est utilisée font 50% moins de dégâts pour les 10 prochaines secondes.",
    };

    string[] natureSkillNames = new string[]
    {
        "Mélodie de la nature II",
        "Mélodie de la nature III",
    };
    string[] natureSkillDescriptions = new string[]
    {
        "Lorsque Mélodie de la nature [R] commence son temps de recharge, le joueur reçoit un effet de soins qui le soigne périodiquement sur 10 secondes.",
        "Lorsque Mélodie de la nature [R] commence son temps de recharge, tous les habiletés du joueurs se rechargent 2x plus rapidement qu'en temps normal pendant 10 secondes.",
    };

    public string[] GetSkillsInformations(PlayerAbilityEnum ability, int levelIndex)
    {
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
