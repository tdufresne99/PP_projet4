using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            _manager.meshRenderer.material = _manager.spreadFireMat;

            _manager.abilityLocked = true;
            _coroutineSpreadFire = _manager.StartCoroutine(CoroutineSpreadFire());
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            _manager.StopCoroutine(_coroutineSpreadFire);
            _manager.abilityLocked = false;
            _manager.spreadFireOnCooldown = true;
        }

        public IEnumerator CoroutineSpreadFire()
        {
            var detectedEnemies = new List<EnemyDamageReceiver>();
            Collider[] colliders = Physics.OverlapSphere(_manager.transform.position, _manager.spreadFireRange);
            foreach (Collider collider in colliders)
            {
                var detectedEnemy = collider.GetComponent<EnemyDamageReceiver>();

                if (detectedEnemy != null)
                {
                    detectedEnemies.Add(detectedEnemy);
                    var spreadFireDebuff = detectedEnemy.gameObject.AddComponent<SpreadFireDebuff>();
                    spreadFireDebuff.playerStateManagerCS = _manager;

                    Debug.DrawLine(_manager.transform.position, collider.transform.position, Color.green, 1f);
                }
                else
                {
                    Debug.DrawLine(_manager.transform.position, collider.transform.position, Color.red, 1f);
                }
            }

            yield return new WaitForSecondsRealtime(0.5f);
            _manager.TransitionToState(_manager.idleState);
        }
    }
}