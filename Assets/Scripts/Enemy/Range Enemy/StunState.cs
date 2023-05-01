using System.Collections;
using UnityEngine;

namespace Enemy.Range
{
    public class StunState : RangeEnemyState
    {
        private RangeEnemyStateManager _manager;
        private Coroutine _coroutineStun;

        public StunState(RangeEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.enemyAnimator.SetBool("isStunned", true);
            
            _manager.stunned = true;
            _manager.currentMovementSpeed = 0;
            _coroutineStun = _manager.StartCoroutine(CoroutineStun());
        }

        public override void Execute()
        {

        }

        public override void Exit()
        {
            _manager.enemyAnimator.SetBool("isStunned", false);
            if (_coroutineStun != null) _manager.StopCoroutine(_coroutineStun);
        }

        private IEnumerator CoroutineStun()
        {
            yield return new WaitForSecondsRealtime(_manager.stunDuration);

            _manager.stunned = false;
            _manager.TransitionToState(_manager.chaseState);
        }
    }
}