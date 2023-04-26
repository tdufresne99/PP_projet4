using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy
{
    [RequireComponent(typeof(HealthManager))]
    public class EnemyDamageReceiver : MonoBehaviour
    {
        private HealthManager _healthManagerCS;
        [SerializeField] private EnemyTypes _enemyType;
        public EnemyTypes enemyType => _enemyType;
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

        public void OnDamageReceived(float damage)
        {
            var accurateDamageReceived = damage * _damageMultiplier;
            _healthManagerCS.ReceiveDamage(accurateDamageReceived);
        }
    }
}