using System;
using System.Collections;
using UnityEngine;

namespace Enemy.Range
{
    public class BasicAttackState : RangeEnemyState
    {
        private float _bonusRangeWhileCasting = 5f;
        private float _firstAttackWaitTime = 1f;
        private Coroutine _coroutineBasicAttack;
        private RangeEnemyStateManager _manager;

        public BasicAttackState(RangeEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.enemyAnimator.SetBool("isAttacking", true);

            _manager.currentMovementSpeed = 0;

            _coroutineBasicAttack = _manager.StartCoroutine(CoroutineBasicAttack());
        }

        public override void Execute()
        {
            _manager.transform.LookAt(_manager.playerStateManagerCS.transform);
            if (!_manager.DetectObject(_manager.targetTransform, _manager.currentAttackRange, _manager.targetLayerMask)) _manager.TransitionToState(_manager.chaseState);
        }

        public override void Exit()
        {
            _manager.enemyAnimator.SetBool("isAttacking", false);
            _manager.StopCoroutine(_coroutineBasicAttack);
        }

        private IEnumerator CoroutineBasicAttack()
        {
            yield return new WaitForSecondsRealtime(_firstAttackWaitTime);
            while (true)
            {
                _manager.currentAttackRange = _manager.baseAttackRange + _bonusRangeWhileCasting;
                yield return new WaitForSecondsRealtime(_manager.currentAttackSpeed);
                _manager.currentAttackRange = _manager.baseAttackRange;
                PerformBasicAttack();
            }
        }

        private void PerformBasicAttack()
        {
            InstantiateProjectile();
        }

        public void InstantiateProjectile()
        {
            var instanciatedProjectile = GameObject.Instantiate(_manager.projectileGO, _manager.projectileSpawnTransform.position, Quaternion.identity);
            var rangeEnemyProjectileCS = instanciatedProjectile.GetComponent<RangeEnemyProjectile>();
            rangeEnemyProjectileCS.targetTransform = _manager.targetTransform;
            rangeEnemyProjectileCS.damage = _manager.currentAttackDamage;
        }
    }
}