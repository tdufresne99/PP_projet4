using System.Collections;
using UnityEngine;

namespace Enemy.Healer
{
    public class DyingState : HealerEnemyState
    {
        private HealerEnemyStateManager _manager;

        public DyingState(HealerEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.enemyAnimator.SetTrigger("die");
            _manager.healerAudioSource.PlayOneShot(_manager.deathSound);
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