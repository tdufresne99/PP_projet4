using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

namespace Player
{
    public class IceShieldState : PlayerState
    {
        private int _stacks = 0;
        private PlayerStateManager _manager;
        private Coroutine _coroutineIceShield;

        public IceShieldState(PlayerStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.playerAnimator.SetTrigger("iceShield");


            _manager.abilityLocked = true;
            _stacks = 0;
            _manager.iceShieldStacks = _stacks;
            _coroutineIceShield = _manager.StartCoroutine(CoroutineIceShield());
        }

        public override void Execute()
        {

        }

        public override void Exit()
        {
            if (_coroutineIceShield != null) _manager.StopCoroutine(_coroutineIceShield);
            _manager.abilityLocked = false;
            _manager.iceShieldOnCooldown = true;
        }

        public IEnumerator CoroutineIceShield()
        {
            var moveSpeed = _manager.currentMovementSpeed;
            _manager.currentMovementSpeed *= 0.01f;
            
            Collider[] colliders = Physics.OverlapSphere(_manager.transform.position, _manager.iceShieldRange);
            foreach (Collider collider in colliders)
            {
                var detectedEnemy = collider.GetComponent<EnemyDamageReceiver>();

                if (detectedEnemy != null)
                {
                    _stacks++;
                    if (_stacks > _manager.iceShieldMaxStacks)
                    {
                        _stacks = _manager.iceShieldMaxStacks;
                    }

                    if(_manager.iceShieldLevel > 2) 
                    {
                        var debuff = detectedEnemy.gameObject.AddComponent<IceShieldDamageReductionDebuff>();
                        debuff.GetDebuffValues(_manager.iceShieldDebuffDuration, _manager.iceShieldDebuffDamageReduction);
                    }

                    Debug.DrawLine(_manager.transform.position, collider.transform.position, Color.green, 1f);
                }
                else
                {
                    Debug.DrawLine(_manager.transform.position, collider.transform.position, Color.red, 1f);
                }
            }
            if(_stacks > _manager.iceShieldMaxStacks) _stacks = _manager.iceShieldMaxStacks;
            if(_manager.iceShieldLevel > 1) _manager.iceShieldStacks = _stacks;
            if(_stacks == 0) _stacks++;
            _manager.shieldManagerCS.ReceiveShield(_manager.iceShieldHealthPerStack * _stacks);

            yield return new WaitForSecondsRealtime(1.2f);
            _manager.currentMovementSpeed /= 0.01f;
            _manager.TransitionToState(_manager.idleState);
        }
    }
}