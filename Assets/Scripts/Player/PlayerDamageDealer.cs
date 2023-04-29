using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

namespace Player
{
    public class PlayerDamageDealer : MonoBehaviour
    {
        private PlayerHealingReceiver _playerHealingReceiverCS;
        private PlayerStateManager _playerStateManagerCS;

        private float _damageMultiplier => _playerStateManagerCS.currentDamageMultiplier;

        void Awake()
        {
            _playerHealingReceiverCS = GetComponent<PlayerHealingReceiver>();
            if (_playerHealingReceiverCS == null) Debug.LogWarning("No 'PlayerHealingReceiver' found on " + gameObject.name + " (PlayerDamageDealer.cs)");

            _playerStateManagerCS = GetComponent<PlayerStateManager>();
            if (_playerStateManagerCS == null) Debug.LogWarning("No 'PlayerStateManager' found on " + gameObject.name + " (PlayerDamageDealer.cs)");
        }

        public void DealDamage(EnemyDamageReceiver enemyDamageReceiver, float damage, float leech)
        {
            var calculatedDamage = damage * _damageMultiplier;
            enemyDamageReceiver.DamageReceived(calculatedDamage);
            OnDamageDealt?.Invoke(calculatedDamage);
            if (leech > 0) _playerHealingReceiverCS.ReceiveHealing(calculatedDamage * leech);
        }

        public event Action<float> OnDamageDealt;
    }
}