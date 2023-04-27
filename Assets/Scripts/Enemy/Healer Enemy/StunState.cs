using System.Collections;
using UnityEngine;

namespace Enemy.Healer
{
    public class StunState : HealerEnemyState
    {
        private HealerEnemyStateManager _manager;
        private Coroutine _coroutineStun;

        public StunState(HealerEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.stunMat;
            _manager.stunned = true;
            _manager.currentMovementSpeed = 0;
            _coroutineStun = _manager.StartCoroutine(CoroutineStun());
        }

        public override void Execute()
        {

        }

        public override void Exit()
        {
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