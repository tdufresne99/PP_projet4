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
            _manager.meshRenderer.material = _manager.chaseMat;

            _manager.currentMovementSpeed = _manager.baseMovementSpeed;
        }

        public override void Execute()
        {
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
            
        }
    }
}