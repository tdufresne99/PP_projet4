using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerHealingReceiver : MonoBehaviour
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
            if (_healthManagerCS == null) Debug.LogWarning("No HealthManager found on " + gameObject.name + " (PlayerHealingReceiver.cs)");
        }

        public void ReceiveHealing(float healing)
        {
            _healthManagerCS.ReceiveHealing(healing * _healingMultiplier);
        }

        public event Action<float> OnHealingReceived;
    }
}