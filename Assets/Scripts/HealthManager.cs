using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private RectTransform _healthBar;

    [SerializeField] private float _maxHealthPoints;
    public float maxHealthPoints => _maxHealthPoints;

    [SerializeField] private float _currentHealthPoints;
    public float currentHealthPoints
    {
        get => _currentHealthPoints;
        set
        {
            if (_currentHealthPoints == value) return;

            if (value <= 0)
            {
                _currentHealthPoints = 0;
                OnHealthPointsEmpty.Invoke();
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
        _currentHealthPoints = _maxHealthPoints;
        AjustHealthBar();
    }

    public void ReceiveDamage(float damageReceived)
    {
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
