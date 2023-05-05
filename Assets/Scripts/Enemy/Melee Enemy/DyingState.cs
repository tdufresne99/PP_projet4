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
            if(_manager.enrageActive) _manager.OnEnrageExit();

            // ---- Set state animations ------------------------------
            _manager.enemyAnimator.SetTrigger("die");
            _manager.meleeAudioSource.PlayOneShot(_manager.deathSound);
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