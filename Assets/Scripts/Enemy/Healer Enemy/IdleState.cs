using System.Collections;
using UnityEngine;

namespace Enemy.Healer
{
    public class IdleState : HealerEnemyState
    {
        private HealerEnemyStateManager _manager;

        public IdleState(HealerEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.idleMat;
        }

        public override void Execute()
        {
            if (_manager.DetectObject(_manager.targetTransform, _manager.detectionRange, _manager.targetLayerMask))
            {
                _manager.inCombat = true;
                _manager.TransitionToState(_manager.chaseState);
            }
        }

        public override void Exit()
        {
            
        }
    }
}