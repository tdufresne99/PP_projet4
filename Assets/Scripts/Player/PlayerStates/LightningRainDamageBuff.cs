using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using TMPro;

namespace Player
{
    public class LightningRainDamageBuff : MonoBehaviour
    {
        private GameObject _buffIconGO;
        private GameObject _instanciatedBuffIconGO;
        private int _stacks = 0;
        private float _buffPerStack = 0.1f;
        private float _duration = 10f;
        private Coroutine _coroutineLightningRainDamageBuff;
        private PlayerStateManager _playerStateManagerCS;
        private HealthManager _playerHealthManagerCS;

        void Awake()
        {
            var activeLightningRainDamageBuffs = GetComponents<LightningRainDamageBuff>();

            foreach (var buff in activeLightningRainDamageBuffs)
            {
                if(buff != this) Destroy(buff);
            }
        }

        void Start()
        {
            _coroutineLightningRainDamageBuff = StartCoroutine(CoroutineLightningRainDamageBuff());
        }

        public void GetBuffValues(PlayerStateManager playerStateManagerCS, int stacks, float buffPerStack, float duration)
        {
            _playerStateManagerCS = playerStateManagerCS;
            _playerHealthManagerCS = _playerStateManagerCS.healthManagerCS;
            _buffIconGO = playerStateManagerCS.lightningRainBuffIconGO;
            _stacks = stacks;
            _buffPerStack = buffPerStack;
            _duration = duration;
        }

        private IEnumerator CoroutineLightningRainDamageBuff()
        {
            _instanciatedBuffIconGO = Instantiate(_buffIconGO, _playerHealthManagerCS.iconHolder.transform.position, _playerHealthManagerCS.iconHolder.transform.rotation, _playerHealthManagerCS.iconHolder.transform);
            var iconText = _instanciatedBuffIconGO.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            var stacksText = _instanciatedBuffIconGO.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            stacksText.text = _stacks + "";
            _playerStateManagerCS.currentDamageMultiplier *= 1 + (_buffPerStack * _stacks);

            var remainingTime = _duration;
            while (remainingTime > 0)
            {
                iconText.text = remainingTime + "";
                yield return new WaitForSecondsRealtime(1f);
                remainingTime--;
            }
            Destroy(this);
        }

        void OnDestroy()
        {
            if(_instanciatedBuffIconGO != null) Destroy(_instanciatedBuffIconGO);
            _playerStateManagerCS.currentDamageMultiplier /= _buffPerStack * _stacks;
        }
    }
}