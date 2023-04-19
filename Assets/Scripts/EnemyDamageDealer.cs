using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageDealer : MonoBehaviour
{
    [SerializeField] private PlayerDamageReceiver _playerDamageReceiver;
    [SerializeField] private float _damageMultiplier = 1;
    public float damageMultiplier
    {
        get => _damageMultiplier;
        set
        {
            _damageMultiplier = value;
        }
    }

    void Awake()
    {

    }

    public void OnDamageDealt(float damage)
    {
        var calculatedDamage = damage * _damageMultiplier;
        _playerDamageReceiver.OnDamageReceived(calculatedDamage);
    }
}