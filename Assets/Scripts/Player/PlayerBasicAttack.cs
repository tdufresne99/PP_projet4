using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerBasicAttack : MonoBehaviour
    {
        private PlayerStateManager _playerStateManagerCS;
        private PlayerDamageDealer _playerDamageDealer;

        void Awake()
        {
            _playerStateManagerCS = GetComponentInParent<PlayerStateManager>();
            if(_playerStateManagerCS == null) Debug.LogWarning("No 'PlayerStateManager' found on " + gameObject.name + " (PlayerBasicAttack.cs)");
            
            _playerDamageDealer = GetComponentInParent<PlayerDamageDealer>();
            if(_playerDamageDealer == null) Debug.LogWarning("No 'PlayerDamageDealer' found on " + gameObject.name + " (PlayerBasicAttack.cs)");
        }

        void OnTriggerEnter(Collider other)
        {
            var enemyDamageReceiver = other.GetComponent<EnemyDamageReceiver>();
            if (enemyDamageReceiver != null)
            {
                _playerDamageDealer.DealDamage(enemyDamageReceiver, _playerStateManagerCS.currentAttackDamage, 1, _playerStateManagerCS.currentLeech);
            }
        }
    }
}
