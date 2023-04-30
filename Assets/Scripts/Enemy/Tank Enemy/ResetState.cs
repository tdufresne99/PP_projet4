using System.Collections;
using UnityEngine;

namespace Enemy.Tank
{
    public class ResetState : TankEnemyState
    {
        private TankEnemyStateManager _manager;

        public ResetState(TankEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.resetMat;

            // _manager.navMeshAgentManagerCS.ChangeDestination(_manager.resetTransform.position);
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}