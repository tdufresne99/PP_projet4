using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

namespace Player
{
    public class LightningRainDamageBuff : MonoBehaviour
    {
        private int _stacks = 0;
        private float _buffPerStack = 0.1f;
        private float _duration = 10f;
        private Coroutine _coroutineLightningRainDamageBuff;
        private PlayerStateManager _playerStateManagerCS;

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
            _stacks = stacks;
            _buffPerStack = buffPerStack;
            _duration = duration;
        }

        private IEnumerator CoroutineLightningRainDamageBuff()
        {
            _playerStateManagerCS.currentDamageMultiplier *= 1 + (_buffPerStack * _stacks);
            yield return new WaitForSecondsRealtime(_duration);
            Destroy(this);
        }

        void OnDestroy()
        {
            _playerStateManagerCS.currentDamageMultiplier /= _buffPerStack * _stacks;
        }
    }
}