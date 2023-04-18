using System.Collections;
using UnityEngine;

namespace MeleeEnemy
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
            Debug.Log(_manager.gameObject.name + " is now chasing");

            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.chaseMat;
        }

        public override void Execute()
        {
            if (_manager.DetectObject(_manager.targetTransform, _manager.attackRange, _manager.targetLayerMask)) 
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