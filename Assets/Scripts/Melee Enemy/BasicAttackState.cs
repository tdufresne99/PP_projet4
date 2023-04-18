using System.Collections;
using UnityEngine;

namespace MeleeEnemy
{
    public class BasicAttackState : MeleeEnemyState
    {
        private Coroutine coroutineBasicAttack;
        private MeleeEnemyStateManager _manager;

        public BasicAttackState(MeleeEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            Debug.Log(_manager.gameObject.name + " is now using a basic attack");
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.basicAttackMat;

            // ---- Start coroutine basic attack ----------------------
            coroutineBasicAttack = _manager.StartCoroutine(CoroutineBasicAttack());
        }

        public override void Execute()
        {
            if (!_manager.DetectObject(_manager.targetTransform, _manager.attackRange, _manager.targetLayerMask)) _manager.TransitionToState(_manager.chaseState);
        }

        public override void Exit()
        {
            _manager.StopCoroutine(coroutineBasicAttack);
        }

        private IEnumerator CoroutineBasicAttack()
        {
            while (true)
            {
                PerformBasicAttack();
                yield return new WaitForSecondsRealtime(_manager.globalCooldown);
            }
        }

        private void PerformBasicAttack()
        {
            Debug.Log(_manager.gameObject.name + " performs a basic attack");
        }
    }
}