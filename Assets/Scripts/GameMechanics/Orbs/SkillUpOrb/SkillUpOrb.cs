using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class SkillUpOrb : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        var playerStateManagerCS = other.GetComponent<PlayerStateManager>();
        if(playerStateManagerCS != null)
        {
            LevelManager.instance?.OnSkillUpOrbPickedUp();
            Destroy(gameObject);
        }
    }
}
