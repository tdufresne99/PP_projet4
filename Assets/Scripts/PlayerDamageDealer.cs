using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageDealer : MonoBehaviour
{
    [SerializeField] private float _baseDamage = 10;
    public float baseDamage
    {
        get => _baseDamage;
        set
        {
            _baseDamage = value;
            AjustDamageValue();
        }
    }

    [SerializeField] private float _damageMultiplier = 1;
    public float damageMultiplier
    {
        get => _damageMultiplier;
        set
        {
            _damageMultiplier = value;
            AjustDamageValue();
        }
    }

    [SerializeField] private float _currentDamage;
    public float currentDamage => _currentDamage;

    void Awake()
    {
        AjustDamageValue();
    }

    private void AjustDamageValue()
    {
        _currentDamage = _baseDamage * _damageMultiplier;
    }
}