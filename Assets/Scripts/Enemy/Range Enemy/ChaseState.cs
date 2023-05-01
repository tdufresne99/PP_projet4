using System.Collections;
using UnityEngine;

namespace Enemy.Range
{
    public class ChaseState : RangeEnemyState
    {
        private RangeEnemyStateManager _manager;

        public ChaseState(RangeEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.enemyAnimator.SetBool("isRunning", true);

            _manager.currentMovementSpeed = _manager.baseMovementSpeed;
        }

        public override void Execute()
        {
            _manager.transform.LookAt(_manager.playerStateManagerCS.transform);
            if (_manager.DetectObject(_manager.targetTransform, _manager.currentAttackRange, _manager.targetLayerMask)) 
            {
                _manager.TransitionToState(_manager.basicAttackState);
                return;
            }

            // ---- Set NavMeshDestination ----------------------------
            _manager.navMeshAgentManagerCS.ChangeDestination(_manager.targetTransform.position);
        }

        public override void Exit()
        {
            _manager.enemyAnimator.SetBool("isRunning", false);
        }
    }
}