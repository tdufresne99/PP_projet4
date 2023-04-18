using System.Collections;
using UnityEngine;

namespace TankEnemy
{
    public class ChaseState : TankEnemyState
    {
        private TankEnemyStateManager _manager;

        public ChaseState(TankEnemyStateManager manager)
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
            
        }

        public override void Exit()
        {
            
        }
    }
}