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
            Debug.Log("player collided with Skill Up Orb");
            LevelManager.instance?.OnSkillUpOrbPickedUp();
            Destroy(gameObject);
        }
    }
}
