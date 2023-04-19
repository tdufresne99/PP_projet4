using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthManager))]
public class PlayerDamageReceiver : MonoBehaviour
{
    private HealthManager _healthManagerCS;
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
        _healthManagerCS = GetComponent<HealthManager>();
    }

    public void OnDamageReceived(float damage)
    {
        var accurateDamageReceived = damage * _damageMultiplier;
        _healthManagerCS.ReceiveDamage(accurateDamageReceived);
    }
}