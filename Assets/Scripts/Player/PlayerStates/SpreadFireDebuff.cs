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

        void Awake()
        {
            _enemyDamageReceiverCS = GetComponent<EnemyDamageReceiver>();
            if (_enemyDamageReceiverCS == null)
            {
                Debug.LogWarning(gameObject.name + " does not have the 'EnemyDamageReceiver' component. (SpreadFireDebuff.cs)");
                Destroy(this);
            }

            var activeSpreadFireDebuff = GetComponent<SpreadFireDebuff>();
            if (activeSpreadFireDebuff != null && activeSpreadFireDebuff != this) Destroy(activeSpreadFireDebuff);
        }
        void Start()
        {
            _spreadFireTicks = playerStateManagerCS.spreadFireTicks;
            _spreadFireDuration = playerStateManagerCS.spreadFireDuration;
            _spreadFireDamage = playerStateManagerCS.spreadFireDamage;

            _coroutineSpreadFireTick = StartCoroutine(CoroutineSpreadFireTick());
        }

        private IEnumerator CoroutineSpreadFireTick()
        {
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
            if (_coroutineSpreadFireTick != null) StopCoroutine(_coroutineSpreadFireTick);
        }
    }
}
