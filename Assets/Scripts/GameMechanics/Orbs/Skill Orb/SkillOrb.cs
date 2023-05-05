using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class SkillOrb : MonoBehaviour
{
    [SerializeField] private PlayerAbilityEnum orbAbility;
    [SerializeField] private AudioClip _skillUpSound;

    void OnTriggerEnter(Collider other)
    {
        var playerStateManagerCS = other.GetComponent<PlayerStateManager>();
        if(playerStateManagerCS != null)
        {
            GameManager.instance.PlaySoundOneShot(_skillUpSound);
            playerStateManagerCS.OnAbilityLearned(orbAbility);
            TrialsManager.instance?.OnSkillOrbPickUp(orbAbility);
            Destroy(gameObject);
        }
    }
}
