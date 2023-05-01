using System.Collections;
using UnityEngine;

namespace Enemy.Melee
{
    public class ChaseState : MeleeEnemyState
    {
        private MeleeEnemyStateManager _manager;

        public ChaseState(MeleeEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.enemyAnimator.SetBool("isRunning", true);
        }

        public override void Execute()
        {
            Vector3 direction = _manager.playerStateManagerCS.transform.position - _manager.transform.position;
            direction.y = 0f;
            _manager.transform.rotation = Quaternion.LookRotation(direction);
            if (_manager.DetectObject(_manager.targetTransform, _manager.baseAttackRange, _manager.targetLayerMask)) 
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