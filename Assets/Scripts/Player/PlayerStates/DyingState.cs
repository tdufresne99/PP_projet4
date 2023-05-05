using System.Collections;
using UnityEngine;

namespace Player
{
    public class DyingState : PlayerState
    {
        private PlayerStateManager _manager;

        public DyingState(PlayerStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.playerAnimator.SetBool("isDead", true);
            if(GameManager.instance != null) GameManager.instance.PlaySoundOneShot(_manager.deathSound);
            _manager.currentMovementSpeed = 0;
            _manager.currentRotateSpeed = 0;

            _manager.isDead = true;
            _manager.abilityLocked = true;
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            _manager.playerAnimator.SetBool("isDead", false);
            _manager.healthManagerCS.isDead = false;
            _manager.isDead = false;
            _manager.currentMovementSpeed = _manager.baseMovementSpeed;
            _manager.currentRotateSpeed = _manager.rotateSpeed;
        }

        private void OnDeath()
        {
            _manager.currentMovementSpeed = 0;
        }
    }
}