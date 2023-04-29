using System.Collections;
using UnityEngine;
using Enemy;
using Enemy.Tank;
using Enemy.Range;
using Enemy.Melee;
using Enemy.Healer;

namespace Player
{
    public class LightningRainState : PlayerState
    {
        private PlayerStateManager _manager;
        private Coroutine _coroutineLightningRain;
        private int _charges = 0;
        private bool _lightRainPerformed = false;

        public LightningRainState(PlayerStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.lightningRainMat;

            _lightRainPerformed = false;
            _charges = 0;

            _manager.currentMovementSpeed = _manager.lightningRainMoveSpeed;
            _manager.abilityLocked = true;
            _coroutineLightningRain = _manager.StartCoroutine(CoroutineChargeLightningRain());
        }

        public override void Execute()
        {
            if (Input.GetKeyUp(KeyCode.Q))
            {
                if (_coroutineLightningRain != null) 
                {
                    _manager.StopCoroutine(_coroutineLightningRain);
                    Debug.Log("Stopped coroutine lightning rain");
                }
                else
                {
                    Debug.LogWarning("Could not stop coroutine lightning rain");
                }
                PerformLightningRain();
            }
        }

        public override void Exit()
        {
            if(_coroutineLightningRain != null) _manager.StopCoroutine(_coroutineLightningRain);
            _manager.currentMovementSpeed = _manager.baseMovementSpeed;
            _manager.abilityLocked = false;
            _manager.lightningRainOnCooldown = true;
        }

        private IEnumerator CoroutineChargeLightningRain()
        {
            var activationDelayPerCharge = _manager.lightningRainActivationDelay / _manager.lightningRainMaxCharges;

            for (int i = 0; i < _manager.lightningRainMaxCharges; i++)
            {
                _charges++;
                Debug.Log("Charging... (" + _charges + ")");
                yield return new WaitForSecondsRealtime(activationDelayPerCharge);
            }
            PerformLightningRain();
        }

        private void PerformLightningRain()
        {
            if (_lightRainPerformed) 
            {
                Debug.LogWarning("lightning rain already performed...");
                return;
            }
            Debug.Log("Performing lightning rain ability");
            _lightRainPerformed = true;

            Collider[] colliders = Physics.OverlapSphere(_manager.transform.position, _manager.spreadFireRange);
            foreach (Collider collider in colliders)
            {
                var detectedEnemy = collider.GetComponent<EnemyDamageReceiver>();

                if (detectedEnemy != null)
                {
                    switch (detectedEnemy.enemyType)
                    {
                        case EnemyTypes.Melee:
                            var meleeManager = detectedEnemy.GetComponent<MeleeEnemyStateManager>();
                            meleeManager.stunDuration = _manager.lightningRainStunDuration;
                            meleeManager.TransitionToState(meleeManager.stunState);
                            break;

                        case EnemyTypes.Range:
                            var rangeManager = detectedEnemy.GetComponent<RangeEnemyStateManager>();
                            rangeManager.stunDuration = _manager.lightningRainStunDuration;
                            rangeManager.TransitionToState(rangeManager.stunState);
                            break;

                        case EnemyTypes.Tank:
                            var tankManager = detectedEnemy.GetComponent<TankEnemyStateManager>();
                            tankManager.currentStunDuration = _manager.lightningRainStunDuration;
                            tankManager.TransitionToState(tankManager.stunState);
                            break;

                        case EnemyTypes.Healer:
                            var healerManager = detectedEnemy.GetComponent<HealerEnemyStateManager>();
                            healerManager.stunDuration = _manager.lightningRainStunDuration;
                            healerManager.TransitionToState(healerManager.stunState);
                            break;

                        default:
                            break;
                    }

                    _manager.playerDamageDealerCS.DealDamage(detectedEnemy, _manager.lightningRainDamagePerCharge * _charges, _manager.currentLeech);

                    Debug.DrawLine(_manager.transform.position, collider.transform.position, Color.green, 1f);
                }
                else
                {
                    Debug.DrawLine(_manager.transform.position, collider.transform.position, Color.red, 1f);
                }
            }
            _manager.TransitionToState(_manager.idleState);
        }
    }
}