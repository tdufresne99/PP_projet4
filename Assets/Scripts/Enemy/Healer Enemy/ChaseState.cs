using System.Collections;
using UnityEngine;

namespace Enemy.Healer
{
    public class ChaseState : HealerEnemyState
    {
        private HealerEnemyStateManager _manager;

        public ChaseState(HealerEnemyStateManager manager)
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
            Vector3 direction = _manager.playerStateManagerCS.transform.position - _manager.transform.position;
            direction.y = 0f;
            _manager.transform.rotation = Quaternion.LookRotation(direction);
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