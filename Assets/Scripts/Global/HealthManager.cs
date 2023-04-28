using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private RectTransform _healthBar;
    private ShieldManager _shieldManagerCS;

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

    void Start()
    {
        _shieldManagerCS = GetComponent<ShieldManager>();
    }

    public void SetHealthPointsValues(float maxHP)
    {
        _maxHealthPoints = maxHP;
        currentHealthPoints = maxHealthPoints;
    }

    public void ReceiveDamage(float damageReceived)
    {
        if(_shieldManagerCS != null)
        {
            if(_shieldManagerCS.currentShieldPoints > 0)
            {
                _shieldManagerCS.ReceiveDamage(damageReceived);
                return;
            }
        }

        currentHealthPoints -= damageReceived;
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
}
