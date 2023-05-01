using System.Collections;
using UnityEngine;

namespace Enemy.Tank
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
            _manager.enemyAnimator.SetTrigger("die");
            _manager.Invoke("SelfDestruct", 2f);
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}