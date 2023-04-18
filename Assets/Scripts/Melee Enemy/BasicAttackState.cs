using System.Collections;
using UnityEngine;

namespace MeleeEnemy
{
    public class BasicAttackState : MeleeEnemyState
    {
        private float _firstAttackWaitTime = 1f;
        private Coroutine coroutineBasicAttack;
        private int _nbOfAttackPerformed;
        private MeleeEnemyStateManager _manager;

        public BasicAttackState(MeleeEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            Debug.Log(_manager.gameObject.name + " is now attacking");
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.basicAttackMat;

            // ---- Start coroutine basic attack ----------------------
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
            _manager.successiveBasicAttacks++;
            Debug.Log(_manager.gameObject.name + " performs a basic attack (" + _manager.successiveBasicAttacks + ")");
        }
    }
}