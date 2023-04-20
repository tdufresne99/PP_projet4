using System.Collections;
using UnityEngine;

namespace TankEnemy
{
    public class DyingState : TankEnemyState
    {
        private TankEnemyStateManager _manager;

        public DyingState(TankEnemyStateManager manager)
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