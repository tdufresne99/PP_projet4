using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HealerEnemy
{
    public class HealingAbilityState : HealerEnemyState
    {
        private HealerEnemyStateManager _manager;
        private Coroutine _coroutineHealing;

        public HealingAbilityState(HealerEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            if (_manager.healOnCooldown)
            {
                _manager.TransitionToState(_manager.chaseState);
                return;
            }

            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.idleMat;

            _coroutineHealing = _manager.StartCoroutine(CoroutineHealing());
        }

        public override void Execute()
        {

        }

        public override void Exit()
        {
            if (_coroutineHealing != null) _manager.StopCoroutine(_coroutineHealing);
            _manager.teleportOnCooldown = true;
        }

        public IEnumerator CoroutineHealing()
        {
            Debug.DrawLine(_manager.transform.position, _manager.transform.position + Vector3.forward * _manager.healRange, Color.magenta, 3.5f);
            Debug.DrawLine(_manager.transform.position, _manager.transform.position + Vector3.back * _manager.healRange, Color.magenta, 3.5f);
            Debug.DrawLine(_manager.transform.position, _manager.transform.position + Vector3.right * _manager.healRange, Color.magenta, 3.5f);
            Debug.DrawLine(_manager.transform.position, _manager.transform.position + Vector3.left * _manager.healRange, Color.magenta, 3.5f);
            yield return new WaitForSecondsRealtime(_manager.healCastTime);

            var detectedEnemyHealingReceivers = new List<EnemyHealingReceiver>();
            Collider[] colliders = Physics.OverlapSphere(_manager.transform.position, _manager.healRange);
            foreach (Collider collider in colliders)
            {
                var detectedEnemyHealingReceiver = collider.GetComponent<EnemyHealingReceiver>();

                if (detectedEnemyHealingReceiver != null)
                {
                    detectedEnemyHealingReceivers.Add(detectedEnemyHealingReceiver);

                    detectedEnemyHealingReceiver.OnHealingReceived(_manager.healValue);

                    Debug.DrawLine(_manager.transform.position, collider.transform.position, Color.green, 1f);
                }
                else
                {
                    Debug.DrawLine(_manager.transform.position, collider.transform.position, Color.red, 1f);
                }
            }

            _manager.meshRenderer.material = _manager.idleMat;
            yield return new WaitForSecondsRealtime(0.5f);
            _manager.TransitionToState(_manager.chaseState);
        }
    }
}