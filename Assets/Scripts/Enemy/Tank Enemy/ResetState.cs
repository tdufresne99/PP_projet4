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
            Debug.Log(_manager.gameObject.name + " is now resetting");
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.resetMat;
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}