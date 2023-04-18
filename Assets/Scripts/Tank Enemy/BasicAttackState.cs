using System.Collections;
using UnityEngine;

namespace TankEnemy
{
    public class BasicAttackState : TankEnemyState
    {
        private TankEnemyStateManager _manager;

        public BasicAttackState(TankEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            Debug.Log(_manager.gameObject.name + " is now using a basic attack");
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.basicAttackMat;
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}