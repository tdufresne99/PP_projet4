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
                OnDeath();
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
        AjustHealthBar();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)) ReceiveDamage(10);
        if(Input.GetKeyDown(KeyCode.Alpha2)) ReceiveDamage(20);
        if(Input.GetKeyDown(KeyCode.Alpha3)) ReceiveDamage(30);

        if(Input.GetKeyDown(KeyCode.Alpha4)) ReceiveHealing(10);
        if(Input.GetKeyDown(KeyCode.Alpha5)) ReceiveHealing(20);
        if(Input.GetKeyDown(KeyCode.Alpha6)) ReceiveHealing(30);
    }

    public void ReceiveDamage(int damageReceived)
    {
        currentHealthPoints -= damageReceived;
    }

    public void ReceiveHealing(int healingReceived)
    {
        currentHealthPoints += healingReceived;
    }

    private void AjustHealthBar()
    {
        _healthBar.localScale = new Vector3(_currentHealthPoints / _maxHealthPoints, 1, 1);
    }

    private void OnDeath()
    {
        Debug.Log(gameObject.name + " is dead!");
    }
}
