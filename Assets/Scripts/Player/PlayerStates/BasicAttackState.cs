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
            Debug.Log("melee attack");
            _coroutineMeleeAttack = _manager.StartCoroutine(CoroutineMelee());
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
        }

        public IEnumerator CoroutineMelee()
        {
            _manager.meleeHitboxGO.SetActive(false);
            while(true)
            {
                yield return new WaitForSecondsRealtime(0.2f);
                Debug.Log("active");
                _manager.meleeHitboxGO.SetActive(true);
                yield return new WaitForSecondsRealtime(0.6f);
                Debug.Log("inactive");
                _manager.meleeHitboxGO.SetActive(false);
            }
        } 
    }
}