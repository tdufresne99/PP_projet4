using System.Collections;
using UnityEngine;

namespace TankEnemy
{
    public class IdleState : TankEnemyState
    {
        private TankEnemyStateManager _manager;

        public IdleState(TankEnemyStateManager manager)
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
            if (_manager.DetectObject(_manager.targetTransform, _manager.detectionRange, _manager.targetLayerMask)) _manager.TransitionToState(_manager.chaseState);           
        }

        public override void Exit()
        {
            
        }
    }
}