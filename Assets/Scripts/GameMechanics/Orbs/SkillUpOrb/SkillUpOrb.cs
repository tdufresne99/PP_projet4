using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class SkillUpOrb : MonoBehaviour
{
    [SerializeField] private AudioClip _skillUpSound;

    void OnTriggerEnter(Collider other)
    {
        var playerStateManagerCS = other.GetComponent<PlayerStateManager>();
        if(playerStateManagerCS != null)
        {
            if(GameManager.instance != null) GameManager.instance.PlaySoundOneShot(_skillUpSound);
            LevelManager.instance?.OnSkillUpOrbPickedUp();
            Destroy(gameObject);
        }
    }
}
