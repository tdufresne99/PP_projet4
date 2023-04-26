using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace TankEnemy
{
    public class CleaveAttack : MonoBehaviour
    {
        private TankEnemyStateManager _tankEnemyStateManagerCS;
        private EnemyDamageDealer _enemyDamageDealerCS;

        void Start()
        {
            _tankEnemyStateManagerCS = GetComponentInParent<TankEnemyStateManager>();
            if(_tankEnemyStateManagerCS == null) Debug.LogError("Could not get component 'TankEnemyStateManager' in parent of object " + gameObject.name + " (CleaveAttack.cs)");

            _enemyDamageDealerCS = GetComponentInParent<EnemyDamageDealer>();
            if(_enemyDamageDealerCS == null) Debug.LogError("Could not get component 'EnemyDamageDealer' in parent of object " + gameObject.name + " (CleaveAttack.cs)");
        }

        void OnTriggerEnter(Collider other)
        {
            var playerDamageReceiver = other.GetComponent<PlayerDamageReceiver>();
            if(playerDamageReceiver != null)
            {
                Debug.Log("cleave on player");
                _enemyDamageDealerCS.OnDamageDealt(_tankEnemyStateManagerCS.cleaveAttackDamage);
            }
        }
    }
}
