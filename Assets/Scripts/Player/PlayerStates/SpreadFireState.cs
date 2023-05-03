using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

// test

namespace Player
{
    public class SpreadFireState : PlayerState
    {
        private PlayerStateManager _manager;
        private Coroutine _coroutineSpreadFire;

        public SpreadFireState(PlayerStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.playerAnimator.SetTrigger("spreadFire");

            _manager.abilityLocked = true;
            _coroutineSpreadFire = _manager.StartCoroutine(CoroutineSpreadFire());
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            if(_coroutineSpreadFire != null) _manager.StopCoroutine(_coroutineSpreadFire);
            _manager.abilityLocked = false;
        }

        public IEnumerator CoroutineSpreadFire()
        {
            var moveSpeed = _manager.currentMovementSpeed;
            _manager.currentMovementSpeed = 0;
            yield return new WaitForSecondsRealtime(0.5f);
            _manager.spreadFireOnCooldown = true;
            Collider[] colliders = Physics.OverlapSphere(_manager.transform.position, _manager.spreadFireRange);
            foreach (Collider collider in colliders)
            {
                var detectedEnemy = collider.GetComponent<EnemyDamageReceiver>();

                if (detectedEnemy != null)
                {
                    var spreadFireDebuff = detectedEnemy.gameObject.AddComponent<SpreadFireDebuff>();
                    spreadFireDebuff.playerStateManagerCS = _manager;
                    spreadFireDebuff.playerDamageDealerCS = _manager.playerDamageDealerCS;
                    spreadFireDebuff.spreadFireDebuffIconGO = _manager.spreadFireDebuffIconGO;
                    spreadFireDebuff.InstnciateDebuffIcon();

                    Debug.DrawLine(_manager.transform.position, collider.transform.position, Color.green, 1f);
                }
                else
                {
                    Debug.DrawLine(_manager.transform.position, collider.transform.position, Color.red, 1f);
                }
            }

            yield return new WaitForSecondsRealtime(1.2f);
            _manager.currentMovementSpeed = moveSpeed;
            _manager.TransitionToState(_manager.idleState);
        }
    }
}