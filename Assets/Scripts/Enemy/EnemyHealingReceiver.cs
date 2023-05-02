using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyHealingReceiver : MonoBehaviour
    {
        private HealthManager _healthManagerCS;
        [SerializeField] private float _healingMultiplier = 1;
        public float healingMultiplier
        {
            get => _healingMultiplier;
            set
            {
                _healingMultiplier = value;
            }
        }
        void Awake()
        {
            _healthManagerCS = GetComponent<HealthManager>();
        }

        public void OnHealingReceived(float healing)
        {
            if(_healthManagerCS.isDead) return;
            _healthManagerCS.ReceiveHealing(healing * _healingMultiplier);
        }
    }
}