using System.Collections;
using UnityEngine;

namespace Player
{
    public class BasicAttackState : PlayerState
    {
        private PlayerStateManager _manager;
        private Coroutine _coroutineMeleeAttack;

        public BasicAttackState(PlayerStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.playerAnimator.SetBool("isAttacking", true);
            _coroutineMeleeAttack = _manager.StartCoroutine(CoroutineMelee());
            _manager.currentMovementSpeed = 0;
        }

        public override void Execute()
        {
            if(Input.GetButtonUp("Fire1") || !Input.GetButton("Fire1")) _manager.TransitionToState(_manager.idleState);
        }

        public override void Exit()
        {
            if(_coroutineMeleeAttack != null) _manager.StopCoroutine(_coroutineMeleeAttack);
            _manager.playerAnimator.SetBool("isAttacking", false);
            _manager.meleeHitboxGO.SetActive(false);
            _manager.currentMovementSpeed = _manager.baseMovementSpeed;

        }

        public IEnumerator CoroutineMelee()
        {
            _manager.meleeHitboxGO.SetActive(false);
            while(true)
            {
                yield return new WaitForSecondsRealtime(0.2f);
                _manager.meleeHitboxGO.SetActive(true);
                _manager.playerAudioSource.PlayOneShot(_manager.meleeAttacksSound);
                yield return new WaitForSecondsRealtime(0.6f);
                _manager.meleeHitboxGO.SetActive(false);
            }
        } 
    }
}