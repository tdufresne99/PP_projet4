using System.Collections;
using UnityEngine;

namespace HealerEnemy
{
    public class DyingState : HealerEnemyState
    {
        private HealerEnemyStateManager _manager;

        public DyingState(HealerEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.dyingMat;
            _manager.SelfDestruct();
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}