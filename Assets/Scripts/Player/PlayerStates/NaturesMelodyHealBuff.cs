using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Player
{
    public class NaturesMelodyHealBuff : MonoBehaviour
    {
        private GameObject _buffIconGO;
        private GameObject _instanciatedBuffIconGO;
        private Coroutine _coroutineHealOverTime;
        private float _totalHealing;
        private float _duration;
        private int _ticks;
        private float _cooldownReduction;
        private PlayerStateManager _playerStateManagerCS;
        private PlayerHealingReceiver _playerHealingReceiverCS;
        private HealthManager _playerHealthManagerCS;

        void Awake()
        {
            var existingBuffs = GetComponents<NaturesMelodyHealBuff>();
            foreach (var existingBuff in existingBuffs)
            {
                if (existingBuff != null && existingBuff != this) Destroy(existingBuff);
            }
        }

        void Start()
        {
            _coroutineHealOverTime = StartCoroutine(CoroutineHealOverTime());
        }

        public void GetBuffValues(PlayerStateManager playerStateManager, PlayerHealingReceiver playerHealingReceiver, float totalHealing, float duration, int ticks, float cooldownReduction)
        {
            _playerStateManagerCS = playerStateManager;
            _playerHealingReceiverCS = playerHealingReceiver;
            _playerHealthManagerCS = _playerStateManagerCS.healthManagerCS;
            _buffIconGO = _playerStateManagerCS.naturesMelodyBuffIconGO;
            _totalHealing = totalHealing;
            _duration = duration;
            _ticks = ticks;
            _cooldownReduction = cooldownReduction;
        }

        private IEnumerator CoroutineHealOverTime()
        {
            var remainingTime = _duration;
            var timePerTicks = _duration / _ticks;
            var healingPerTicks = _totalHealing / _ticks;

            _instanciatedBuffIconGO = Instantiate(_buffIconGO, _playerHealthManagerCS.iconHolder.transform.position, _playerHealthManagerCS.iconHolder.transform.rotation, _playerHealthManagerCS.iconHolder.transform);
            var iconText = _instanciatedBuffIconGO.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            _playerStateManagerCS.cooldownReduction += _cooldownReduction;
            while (remainingTime > 0)
            {
                iconText.text = remainingTime + "";

                _playerHealingReceiverCS.ReceiveHealing(healingPerTicks);
                yield return new WaitForSecondsRealtime(timePerTicks);
                remainingTime -= timePerTicks;
            }
            _playerStateManagerCS.cooldownReduction -= _cooldownReduction;

            Destroy(this);
        }

        void OnDestroy()
        {
            if(_instanciatedBuffIconGO != null) Destroy(_instanciatedBuffIconGO);
        }
    }
}