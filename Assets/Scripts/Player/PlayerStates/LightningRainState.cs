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

            _manager.currentMovementSpeed *= _manager.lightningRainMoveSpeed;
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
                PerformLightningRain();
            }
        }

        public override void Exit()
        {
            if (_coroutineLightningRain != null) _manager.StopCoroutine(_coroutineLightningRain);
            _manager.currentMovementSpeed /= _manager.lightningRainMoveSpeed;
            _manager.abilityLocked = false;
            _manager.lightningRainOnCooldown = true;
        }

        private IEnumerator CoroutineChargeLightningRain()
        {
            var activationDelayPerCharge = _manager.lightningRainActivationDelay / _manager.lightningRainMaxCharges;

            for (int i = 0; i < _manager.lightningRainMaxCharges; i++)
            {
                _charges++;
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
            _lightRainPerformed = true;
            _manager.playerAnimator.SetTrigger("lightningRain");

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

            if(_manager.lightningRainLevel > 1) 
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
            _manager.TransitionToState(_manager.idleState);
        }
    }
}