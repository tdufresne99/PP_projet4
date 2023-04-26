using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerDamageDealer : MonoBehaviour
    {
        private PlayerHealingReceiver _playerHealingReceiver;

        void Awake()
        {
            _playerHealingReceiver = GetComponent<PlayerHealingReceiver>();
            if (_playerHealingReceiver == null) Debug.LogWarning("No 'PlayerHealingReceiver' found on " + gameObject.name + " (PlayerDamageDealer.cs)");
        }

        public void DealDamage(EnemyDamageReceiver enemyDamageReceiver, float damage, float damageMultiplier, float leech)
        {
            var calculatedDamage = damage * damageMultiplier;
            enemyDamageReceiver.OnDamageReceived(calculatedDamage);
            OnDamageDealt?.Invoke(calculatedDamage);
            if (leech > 0) _playerHealingReceiver.ReceiveHealing(calculatedDamage * leech);
        }

        public event Action<float> OnDamageDealt;
    }
}