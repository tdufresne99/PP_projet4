using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class HealthOrb : MonoBehaviour
{
    [Range(0, 1f)][SerializeField] private float _healingAmount = 0.25f;

    void OnTriggerEnter(Collider other)
    {
        var playerHealingReceiverCS = other.GetComponent<PlayerHealingReceiver>();
        if(playerHealingReceiverCS != null)
        {
            var maxHP = other.GetComponent<HealthManager>().maxHealthPoints;

            playerHealingReceiverCS.ReceiveHealing(maxHP * _healingAmount);

            Destroy(gameObject);
        }
    }
}