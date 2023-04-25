using System.Collections;
using UnityEngine;


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
            _manager.abilityLocked = true;
            _lightRainPerformed = false;
            _charges = 0;
            _coroutineLightningRain = _manager.StartCoroutine(CoroutineChargeLightningRain());
        }

        public override void Execute()
        {
            if(Input.GetKeyUp(KeyCode.E))
            {
                if(_coroutineLightningRain != null) _manager.StopCoroutine(_coroutineLightningRain);
                PerformLightningRain();
            }
        }

        public override void Exit()
        {
            _manager.abilityLocked = false;
            
        }

        private IEnumerator CoroutineChargeLightningRain()
        {
            var activationDelayPerCharge = _manager.lightningRainActivationDelay / _manager.lightningRainMaxCharges;
            while (_charges < _manager.lightningRainMaxCharges)
            {
                _charges++;
                yield return new WaitForSecondsRealtime(activationDelayPerCharge);
                PerformLightningRain();
            }
        }

        private void PerformLightningRain()
        {
            if(_lightRainPerformed) return;
            _lightRainPerformed = true;
            var detectedEnemies = new System.Collections.Generic.List<EnemyDamageReceiver>();
            Collider[] colliders = Physics.OverlapSphere(_manager.transform.position, _manager.spreadFireRange);
            foreach (Collider collider in colliders)
            {
                var detectedEnemy = collider.GetComponent<EnemyDamageReceiver>();

                if (detectedEnemy != null)
                {
                    detectedEnemies.Add(detectedEnemy);

                    switch (detectedEnemy.enemyType)
                    {
                        case EnemyTypes.Melee:
                            var meleeManager = detectedEnemy.GetComponent<MeleeEnemy.MeleeEnemyStateManager>();
                            meleeManager.stunDuration = _manager.lightningRainStunDuration;
                            meleeManager.TransitionToState(meleeManager.stunState);
                            break;

                        case EnemyTypes.Range:
                            var rangeManager = detectedEnemy.GetComponent<RangeEnemy.RangeEnemyStateManager>();
                            rangeManager.stunDuration = _manager.lightningRainStunDuration;
                            rangeManager.TransitionToState(rangeManager.stunState);
                            break;

                        case EnemyTypes.Tank:
                            var tankManager = detectedEnemy.GetComponent<TankEnemy.TankEnemyStateManager>();
                            tankManager.stunDuration = _manager.lightningRainStunDuration;
                            tankManager.TransitionToState(tankManager.stunState);
                            break;

                        case EnemyTypes.Healer:
                            var healerManager = detectedEnemy.GetComponent<HealerEnemy.HealerEnemyStateManager>();
                            healerManager.stunDuration = _manager.lightningRainStunDuration;
                            healerManager.TransitionToState(healerManager.stunState);
                            break;

                        default:
                            break;
                    }

                    detectedEnemy.OnDamageReceived(_manager.lightningRainDamagePerCharge * _charges);

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