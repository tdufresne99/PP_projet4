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
            Debug.Log(_manager.gameObject.name + " is now idle");
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.idleMat;
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}