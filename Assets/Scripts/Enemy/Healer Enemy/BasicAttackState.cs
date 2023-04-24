using System;
using System.Collections;
using UnityEngine;

namespace HealerEnemy
{
    public class BasicAttackState : HealerEnemyState
    {
        private float _bonusRangeWhileCasting = 5f;
        private float _firstAttackWaitTime = 1f;
        private Coroutine _coroutineBasicAttack;
        private HealerEnemyStateManager _manager;

        public BasicAttackState(HealerEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.basicAttackMat;

            _manager.currentMovementSpeed = 0;

            _coroutineBasicAttack = _manager.StartCoroutine(CoroutineBasicAttack());
        }

        public override void Execute()
        {
            if (!_manager.DetectObject(_manager.targetTransform, _manager.currentAttackRange, _manager.targetLayerMask)) _manager.TransitionToState(_manager.chaseState);
        }

        public override void Exit()
        {
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
            _manager.InstantiateProjectile();
        }
    }
}