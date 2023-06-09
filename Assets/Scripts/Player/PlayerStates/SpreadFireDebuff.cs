using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using TMPro;

namespace Player
{
    public class SpreadFireDebuff : MonoBehaviour
    {
        public GameObject spreadFireDebuffIconGO;
        public GameObject instanciatedSpreadFireDebuffIconGO;
        public TextMeshProUGUI stacksText;
        public PlayerStateManager playerStateManagerCS;
        public PlayerDamageDealer playerDamageDealerCS;
        private EnemyDamageReceiver _enemyDamageReceiverCS;
        private HealthManager _enemyHealthManagerCS;
        private Coroutine _coroutineSpreadFireTick;
        private float _spreadFireDamage;
        public float spreadFireDuration;
        [SerializeField] private float _spreadFireRemainingDuration;
        public float spreadFireRemainingDuration
        {
            get => _spreadFireRemainingDuration;
            set
            {
                if(value > spreadFireMaxDuration) value = spreadFireMaxDuration;
                _spreadFireRemainingDuration = value;
            }
        }
        public float spreadFireMaxDuration = 32f;
        public int spreadFireTicks;
        public float _spreadFireDamageIncreaseDebuff = 0;
        [SerializeField] private int _spreadFireLevel = 1;
        private bool _modifiedEnemyDamageReceiver = false;
        [SerializeField] private int _stacks = 1;
        public int stacks
        {
            get => _stacks;
            set
            {
                if(_spreadFireLevel > 2) Mathf.Clamp(value, 1, 3);
                else value = 1;
                Debug.Log(value);
                _stacks = value;
            }
        }

        void Awake()
        {
            _enemyDamageReceiverCS = GetComponent<EnemyDamageReceiver>();
            if (_enemyDamageReceiverCS == null)
            {
                Debug.LogWarning(gameObject.name + " does not have the 'EnemyDamageReceiver' component. (SpreadFireDebuff.cs)");
                Destroy(this);
            }

            _enemyHealthManagerCS = GetComponent<HealthManager>();
            if (_enemyHealthManagerCS == null)
            {
                Debug.LogWarning(gameObject.name + " does not have the 'HealthManager' component. (SpreadFireDebuff.cs)");
                Destroy(this);
            }
            

            var activeSpreadFireDebuff = GetComponent<SpreadFireDebuff>();
            if (activeSpreadFireDebuff != null)
            {
                if (activeSpreadFireDebuff != this)
                {
                    activeSpreadFireDebuff.stacks++;
                    activeSpreadFireDebuff.spreadFireRemainingDuration += activeSpreadFireDebuff.spreadFireDuration;
                    activeSpreadFireDebuff.stacksText.text = activeSpreadFireDebuff.stacks + "";
                    Debug.Log("destroy by awake");
                    Destroy(this);
                }
            }
        }
        void Start()
        {
            _spreadFireLevel = playerStateManagerCS.spreadFireLevel;
            spreadFireTicks = playerStateManagerCS.spreadFireTicksPerStack;
            spreadFireDuration = playerStateManagerCS.spreadFireDuration;
            _spreadFireDamage = playerStateManagerCS.spreadFireDamage;
            _spreadFireDamageIncreaseDebuff = playerStateManagerCS.spreadFireDamageIntakeMultiplier;

            _coroutineSpreadFireTick = StartCoroutine(CoroutineSpreadFireTick());
        }

        public void InstnciateDebuffIcon()
        {
            instanciatedSpreadFireDebuffIconGO = Instantiate(spreadFireDebuffIconGO, _enemyHealthManagerCS.iconHolder.transform.position, _enemyHealthManagerCS.iconHolder.transform.rotation, _enemyHealthManagerCS.iconHolder.transform);
        }

        private IEnumerator CoroutineSpreadFireTick()
        {
            _enemyDamageReceiverCS.damageMultiplier *= _spreadFireDamageIncreaseDebuff;
            _modifiedEnemyDamageReceiver = true;
            spreadFireRemainingDuration = spreadFireDuration;

            var tickTime = spreadFireDuration / spreadFireTicks;
            var damagePerTick = _spreadFireDamage / spreadFireTicks;
            var iconText = instanciatedSpreadFireDebuffIconGO.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            stacksText = instanciatedSpreadFireDebuffIconGO.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            
            while (spreadFireRemainingDuration > 0)
            {
                iconText.text = spreadFireRemainingDuration + "";
                playerDamageDealerCS.DealDamage(_enemyDamageReceiverCS, damagePerTick * stacks, playerStateManagerCS.currentLeech);
                yield return new WaitForSecondsRealtime(tickTime);
                spreadFireRemainingDuration -= tickTime;
            }
            Debug.Log("destroyed by coroutine");
            Destroy(this);
        }

        void OnDestroy()
        {
            if(instanciatedSpreadFireDebuffIconGO != null) Destroy(instanciatedSpreadFireDebuffIconGO);
            if(_modifiedEnemyDamageReceiver) _enemyDamageReceiverCS.damageMultiplier /= _spreadFireDamageIncreaseDebuff;
            if (_coroutineSpreadFireTick != null) StopCoroutine(_coroutineSpreadFireTick);
        }
    }
}
