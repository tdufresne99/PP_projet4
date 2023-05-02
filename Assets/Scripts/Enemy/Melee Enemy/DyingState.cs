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