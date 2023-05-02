using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    [RequireComponent(typeof(HealthManager))]
    public class PlayerDamageReceiver : MonoBehaviour
    {
        private HealthManager _healthManagerCS;
        [SerializeField] private float _damageMultiplier = 1;
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
            _healthManagerCS = GetComponent<HealthManager>();
        }

        public void ReceiveDamage(float damage)
        {
            if(_healthManagerCS.isDead) return;
            var calculatedDamageReceived = damage * _damageMultiplier;
            _healthManagerCS.ReceiveDamage(calculatedDamageReceived);
            OnDamageReceived?.Invoke(calculatedDamageReceived);
        }

        public event Action<float> OnDamageReceived;
    }
}