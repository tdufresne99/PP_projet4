using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class SkillOrb : MonoBehaviour
{
    [SerializeField] private PlayerAbilityEnum orbAbility;

    void OnTriggerEnter(Collider other)
    {
        var playerStateManagerCS = other.GetComponent<PlayerStateManager>();
        if(playerStateManagerCS != null)
        {
            playerStateManagerCS.OnAbilityLearned(orbAbility);
            Destroy(gameObject);
        }
    }
}
