using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

namespace Player
{
    public class SpreadFireDebuff : MonoBehaviour
    {
        public PlayerStateManager playerStateManagerCS;
        public PlayerDamageDealer playerDamageDealerCS;
        private EnemyDamageReceiver _enemyDamageReceiverCS;
        private Coroutine _coroutineSpreadFireTick;
        private float _spreadFireDamage;
        private float _spreadFireDuration;
        private int _spreadFireTicks;
        public int abilityLevel = 1;
        public float _spreadFireDamageIncreaseDebuff = 0;
        public int _stacks;

        void Awake()
        {
            _enemyDamageReceiverCS = GetComponent<EnemyDamageReceiver>();
            if (_enemyDamageReceiverCS == null)
            {
                Debug.LogWarning(gameObject.name + " does not have the 'EnemyDamageReceiver' component. (SpreadFireDebuff.cs)");
                Destroy(this);
            }

            var activeSpreadFireDebuff = GetComponents<SpreadFireDebuff>();
            if (activeSpreadFireDebuff != null) 
            {
                foreach (var debuff in activeSpreadFireDebuff)
                {
                    if(debuff != this) Destroy(debuff);
                }

            }
        }
        void Start()
        {
            _spreadFireTicks = playerStateManagerCS.spreadFireTicks;
            _spreadFireDuration = playerStateManagerCS.spreadFireDuration;
            _spreadFireDamage = playerStateManagerCS.spreadFireDamage;
            _spreadFireDamageIncreaseDebuff = playerStateManagerCS.spreadFireDamageIncreaseDebuff;

            _coroutineSpreadFireTick = StartCoroutine(CoroutineSpreadFireTick());
        }

        private IEnumerator CoroutineSpreadFireTick()
        {
            _enemyDamageReceiverCS.damageMultiplier += _spreadFireDamageIncreaseDebuff;
            var damagePerTick = _spreadFireDamage / _spreadFireTicks;
            var timeBetweenTicks = _spreadFireDuration / _spreadFireTicks;
            for (int i = 0; i < _spreadFireTicks; i++)
            {
                playerDamageDealerCS.DealDamage(_enemyDamageReceiverCS, damagePerTick, playerStateManagerCS.currentLeech);
                yield return new WaitForSecondsRealtime(timeBetweenTicks);
            }
            Destroy(this);
        }

        void OnDestroy()
        {
            _enemyDamageReceiverCS.damageMultiplier -= _spreadFireDamageIncreaseDebuff;
            if (_coroutineSpreadFireTick != null) StopCoroutine(_coroutineSpreadFireTick);
        }
    }
}
