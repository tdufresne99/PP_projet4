using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private RectTransform _healthBar;

    [SerializeField] private float _maxHealthPoints = 100f;
    public float maxHealthPoints => _maxHealthPoints;

    [SerializeField] private float _currentHealthPoints = 100f;
    public float currentHealthPoints
    {
        get => _currentHealthPoints;
        set
        {
            if (_currentHealthPoints == value) return;

            if (value <= 0)
            {
                _currentHealthPoints = 0;
                OnHealthPointsEmpty?.Invoke();
            }
            else if (value >= _maxHealthPoints)
            {
                _currentHealthPoints = _maxHealthPoints;
            }
            else 
            {
                _currentHealthPoints = value;
            }

            AjustHealthBar();
        }
    }

    public void SetHealthPointsValues(float maxHP)
    {
        _maxHealthPoints = maxHP;
        currentHealthPoints = maxHealthPoints;
    }

    public void ReceiveDamage(float damageReceived)
    {
        currentHealthPoints -= damageReceived;
        OnDamageReceived?.Invoke();
    }

    public void ReceiveHealing(float healingReceived)
    {
        currentHealthPoints += healingReceived;
    }

    private void AjustHealthBar()
    {
        _healthBar.localScale = new Vector3(_currentHealthPoints / _maxHealthPoints, 1, 1);
    }

    public event Action OnHealthPointsEmpty;
    public event Action OnDamageReceived;
}
