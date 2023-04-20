using System.Collections;
using UnityEngine;

namespace RangeEnemy
{
    public class DyingState : RangeEnemyState
    {
        private RangeEnemyStateManager _manager;

        public DyingState(RangeEnemyStateManager manager)
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