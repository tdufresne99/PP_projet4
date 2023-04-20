using System.Collections;
using UnityEngine;

namespace TankEnemy
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

            _manager.navMeshAgentManagerCS.ChangeDestination(_manager.resetTransform.position);
        }

        public override void Execute()
        {
            if(Vector3.Distance(_manager.transform.position, _manager.resetTransform.position) < 2f) _manager.TransitionToState(_manager.idleState);
        }

        public override void Exit()
        {
            
        }
    }
}