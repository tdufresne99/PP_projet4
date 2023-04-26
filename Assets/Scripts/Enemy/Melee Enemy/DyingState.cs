using System.Collections;
using UnityEngine;

namespace Enemy.Melee
{
    public class DyingState : MeleeEnemyState
    {
        private MeleeEnemyStateManager _manager;

        public DyingState(MeleeEnemyStateManager manager)
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