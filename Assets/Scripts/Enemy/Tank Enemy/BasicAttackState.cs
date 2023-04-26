using System.Collections;
using UnityEngine;

namespace TankEnemy
{
    public class BasicAttackState : TankEnemyState
    {
        private float _firstAttackWaitTime = 1f;
        private Coroutine coroutineBasicAttack;
        private TankEnemyStateManager _manager;

        public BasicAttackState(TankEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.basicAttackMat;

            coroutineBasicAttack = _manager.StartCoroutine(CoroutineBasicAttack());
        }

        public override void Execute()
        {
            if (!_manager.DetectObject(_manager.targetTransform, _manager.baseAttackRange, _manager.targetLayerMask)) _manager.TransitionToState(_manager.chaseState);
        }

        public override void Exit()
        {
            _manager.StopCoroutine(coroutineBasicAttack);
        }
        private IEnumerator CoroutineBasicAttack()
        {
            yield return new WaitForSecondsRealtime(_firstAttackWaitTime);
            while (true)
            {
                PerformBasicAttack();
                yield return new WaitForSecondsRealtime(_manager.currentAttackSpeed);
            }
        }

        private void PerformBasicAttack()
        {
            _manager.enemyDamageDealerCS.OnDamageDealt(_manager.currentAttackDamage);

            if (!_manager.cleaveOnCooldown)
            {
                if (Random.Range(0, 1f) <= _manager.cleaveActivationChance)
                {
                    _manager.TransitionToState(_manager.cleaveAbilityState);
                }
            }
        }
    }
}