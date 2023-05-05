using System.Collections;
using UnityEngine;
using Enemy;
using Enemy.Tank;
using Enemy.Range;
using Enemy.Melee;
using Enemy.Healer;
using System.Collections.Generic;

namespace Player
{
    public class LightningRainState : PlayerState
    {
        private PlayerStateManager _manager;
        private Coroutine _coroutineLightningRain;
        private Coroutine _coroutinePerformLightningRain;
        private int _charges = 0;
        private bool _lightRainPerformed = false;

        public LightningRainState(PlayerStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.playerAnimator.SetTrigger("lightningRainCast");

            _lightRainPerformed = false;
            _charges = 0;

            _manager.currentMovementSpeed *= _manager._lightningRainMoveSpeedMultiplier;
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
                }
                else
                {
                    Debug.LogWarning("Could not stop coroutine lightning rain");
                }
                _coroutinePerformLightningRain = _manager.StartCoroutine(CoroutinePerformLightningRain());
            }
        }

        public override void Exit()
        {
            if (_coroutineLightningRain != null) _manager.StopCoroutine(_coroutineLightningRain);
            if (_coroutinePerformLightningRain != null) _manager.StopCoroutine(_coroutinePerformLightningRain);
            _manager.currentMovementSpeed /= _manager._lightningRainMoveSpeedMultiplier;
            _manager.abilityLocked = false;
            _manager.lightningRainOnCooldown = true;
        }

        private IEnumerator CoroutineChargeLightningRain()
        {
            _manager.playerAudioSource.PlayOneShot(_manager.lightningRainCastSound);
            var activationDelayPerCharge = _manager.lightningRainActivationDelay / _manager.lightningRainMaxCharges;

            for (int i = 0; i < _manager.lightningRainMaxCharges; i++)
            {
                _charges++;
                yield return new WaitForSecondsRealtime(activationDelayPerCharge);
            }
            _coroutinePerformLightningRain = _manager.StartCoroutine(CoroutinePerformLightningRain());
        }

        private IEnumerator CoroutinePerformLightningRain()
        {
            if (_lightRainPerformed)
            {
                Debug.LogWarning("lightning rain already performed...");
                _manager.TransitionToState(_manager.idleState);
            }
            _lightRainPerformed = true;
            _manager.playerAnimator.SetTrigger("lightningRain");
            _manager.playerAudioSource.PlayOneShot(_manager.lightningRainSound);

            Collider[] colliders = Physics.OverlapSphere(_manager.transform.position, _manager.spreadFireRange);
            List<EnemyDamageReceiver> enemiesHit = new List<EnemyDamageReceiver>(colliders.Length);

            foreach (Collider collider in colliders)
            {
                var detectedEnemy = collider.GetComponent<EnemyDamageReceiver>();
                if (detectedEnemy != null)
                {
                    enemiesHit.Add(detectedEnemy);
                }
            }

            if(_manager.lightningRainLevel > 1 && enemiesHit.Count > 0) 
            {
                var dmgBuff = _manager.gameObject.AddComponent<LightningRainDamageBuff>();
                dmgBuff.GetBuffValues(_manager, enemiesHit.Count, _manager.lightningRainDamageBuffPerStacks, _manager.lightningRainDamageBuffDuration);
            }

            foreach (EnemyDamageReceiver enemyHit in enemiesHit)
            {
                switch (enemyHit.enemyType)
                {
                    case EnemyTypes.Melee:
                        var meleeManager = enemyHit.GetComponent<MeleeEnemyStateManager>();
                        meleeManager.stunDuration = _manager.lightningRainStunDuration;
                        meleeManager.TransitionToState(meleeManager.stunState);
                        break;

                    case EnemyTypes.Range:
                        var rangeManager = enemyHit.GetComponent<RangeEnemyStateManager>();
                        rangeManager.stunDuration = _manager.lightningRainStunDuration;
                        rangeManager.TransitionToState(rangeManager.stunState);
                        break;

                    case EnemyTypes.Tank:
                        var tankManager = enemyHit.GetComponent<TankEnemyStateManager>();
                        tankManager.currentStunDuration = _manager.lightningRainStunDuration;
                        tankManager.TransitionToState(tankManager.stunState);
                        break;

                    case EnemyTypes.Healer:
                        var healerManager = enemyHit.GetComponent<HealerEnemyStateManager>();
                        healerManager.stunDuration = _manager.lightningRainStunDuration;
                        healerManager.TransitionToState(healerManager.stunState);
                        break;

                    default:
                        break;
                }

                _manager.playerDamageDealerCS.DealDamage(enemyHit, _manager.lightningRainDamagePerCharge * _charges, _manager.currentLeech);
            }
            yield return new WaitForSecondsRealtime(1f);
            _manager.TransitionToState(_manager.idleState);
        }
    }
}