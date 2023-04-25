using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class IceShieldState : PlayerState
    {
        private PlayerStateManager _manager;
        private Coroutine _coroutineIceShield;

        public IceShieldState(PlayerStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            _manager.abilityLocked = true;

            _coroutineIceShield = _manager.StartCoroutine(CoroutineIceShield());
        }

        public override void Execute()
        {

        }

        public override void Exit()
        {
            if (_coroutineIceShield != null) _manager.StopCoroutine(_coroutineIceShield);
            _manager.abilityLocked = false;
        }

        public IEnumerator CoroutineIceShield()
        {
            int stacks = 0;
            Collider[] colliders = Physics.OverlapSphere(_manager.transform.position, _manager.spreadFireRange);
            foreach (Collider collider in colliders)
            {
                var detectedEnemy = collider.GetComponent<EnemyDamageReceiver>();

                if (detectedEnemy != null)
                {
                    stacks++;
                    if (stacks > _manager.iceShieldMaxStacks)
                    {
                        stacks = _manager.iceShieldMaxStacks;
                    }
                    Debug.DrawLine(_manager.transform.position, collider.transform.position, Color.green, 1f);
                }
                else
                {
                    Debug.DrawLine(_manager.transform.position, collider.transform.position, Color.red, 1f);
                }
            }
            if(stacks == 0) stacks = 1;
            _manager.shieldManagerCS.ReceiveShield(_manager.iceShieldHealthPerStack * stacks);

            yield return new WaitForSecondsRealtime(0.5f);
            _manager.TransitionToState(_manager.idleState);
        }
    }
}