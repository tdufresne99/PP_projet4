using System.Collections;
using UnityEngine;

namespace Player
{
    public class NaturesMelodyState : PlayerState
    {
        private PlayerStateManager _manager;
        private Coroutine _coroutineNaturesMelody;

        public NaturesMelodyState(PlayerStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.naturesMelodyMat;

            _manager.abilityLocked = true;
            _coroutineNaturesMelody = _manager.StartCoroutine(CoroutineNaturesMelody());
        }

        public override void Execute()
        {
            if(Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (_coroutineNaturesMelody != null) 
                {
                    _manager.StopCoroutine(_coroutineNaturesMelody);
                    Debug.Log("Stopped coroutine nature's melody");
                }
                else
                {
                    Debug.LogWarning("Could not stop coroutine nature's melody");
                }
                _manager.TransitionToState(_manager.idleState);
            }
        }

        public override void Exit()
        {
            if(_coroutineNaturesMelody != null) _manager.StopCoroutine(_coroutineNaturesMelody);
            _manager.abilityLocked = false;
            _manager.naturesMelodyOnCooldown = true;
        }

        private IEnumerator CoroutineNaturesMelody()
        {
            for (int i = 0; i < _manager.naturesMelodyMaxTicks; i++)
            {
                _manager.playerHealingDealerCS.DealHealing(_manager.playerHealingReceiverCS, _manager.healthManagerCS.maxHealthPoints / _manager.naturesMelodyMaxTicks);
                yield return new WaitForSecondsRealtime(_manager.naturesMelodyTickTime);
            }
            _manager.TransitionToState(_manager.idleState);
        }
    }
}