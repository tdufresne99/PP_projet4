using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerHealingDealer : MonoBehaviour
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

        public void DealHealing(PlayerHealingReceiver playerHealingReceiver, float healing)
        {
            var calculatedHealing = healing * _healingMultiplier;
            playerHealingReceiver.ReceiveHealing(calculatedHealing);
            OnHealingDealt?.Invoke(calculatedHealing);
        }

        public event Action<float> OnHealingDealt;
    }
}
