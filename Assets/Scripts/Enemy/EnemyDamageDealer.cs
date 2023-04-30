using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Enemy
{
    public class EnemyDamageDealer : MonoBehaviour
    {
        [SerializeField] private PlayerDamageReceiver _playerDamageReceiver;
        public PlayerDamageReceiver playerDamageReceiver { get => _playerDamageReceiver; set => _playerDamageReceiver = value; }
        private EnemyHealingReceiver _enemyHealingReceiver;
        [SerializeField] private float _damageMultiplier = 1;
        [SerializeField] private float _leech = 0;
        public float leech
        {
            get => _leech;
            set => _leech = value;
        }

        public float damageMultiplier
        {
            get => _damageMultiplier;
            set
            {
                _damageMultiplier = value;
            }
        }

        void Awake()
        {
            _enemyHealingReceiver = GetComponent<EnemyHealingReceiver>();
            if (_enemyHealingReceiver == null) Debug.LogWarning("No 'EnemyHealingReceiver' found on " + gameObject.name + " (EnemyDamageDealer.cs)");
        }

        public void OnDamageDealt(float damage)
        {
            var calculatedDamage = damage * _damageMultiplier;
            _playerDamageReceiver.ReceiveDamage(calculatedDamage);
            _enemyHealingReceiver.OnHealingReceived(calculatedDamage * leech);
        }
    }
}