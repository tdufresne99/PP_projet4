using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _shieldBars;

    [SerializeField] private float _maxShieldPoints = 200f;

    [SerializeField] private float _shieldPointsPerBar = 50f;

    [SerializeField] private float _currentShieldPoints = 0;
    public float currentShieldPoints
    {
        get => _currentShieldPoints;
        set
        {
            if (_currentShieldPoints == value) return;

            if (value <= 0)
            {
                _currentShieldPoints = 0;
                OnShieldBreak?.Invoke();
            }
            else if (value >= _maxShieldPoints)
            {
                _currentShieldPoints = _maxShieldPoints;
            }
            else
            {
                _currentShieldPoints = value;
            }

            AjustShieldBars();
        }
    }

    public void SetShieldPointsValues(float maxShieldPoints)
    {
        _maxShieldPoints = maxShieldPoints;
        if(_shieldBars.Length > 0) _shieldPointsPerBar = _maxShieldPoints / _shieldBars.Length;
        currentShieldPoints = 0;
        AjustShieldBars();
    }

    public void ReceiveDamage(float damageReceived)
    {
        currentShieldPoints -= damageReceived;
        OnDamageReceived?.Invoke();
    }

    public void ReceiveShield(float shieldReceived)
    {
        currentShieldPoints += shieldReceived;
    }

    private void AjustShieldBars()
    {
        var shieldBars = Mathf.CeilToInt(currentShieldPoints / _shieldPointsPerBar);

        for (int i = 0; i < _shieldBars.Length; i++)
        {
            if (i < shieldBars) _shieldBars[i].SetActive(true);
            else _shieldBars[i].SetActive(false);
        }
    }

    public event Action OnShieldBreak;
    public event Action OnDamageReceived;
}