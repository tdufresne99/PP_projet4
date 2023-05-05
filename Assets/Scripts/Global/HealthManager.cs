using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public GameObject lowHealthWarning;
    public GameObject iconHolder;
    [NonSerialized] public Color defaultColor;
    private Color _stunColor = new Color(0.5f,0.5f,0.5f,0.5f);
    public Color stunColor => _stunColor;
    [SerializeField] private RectTransform _healthBar;
    [SerializeField] private Image _healthBarColor;
    private ShieldManager _shieldManagerCS;

    [SerializeField] private float _maxHealthPoints = 100f;
    public float maxHealthPoints => _maxHealthPoints;

    public bool isDead = false;
    [SerializeField] private float _currentHealthPoints = 100f;
    public float currentHealthPoints

    {
        get => _currentHealthPoints;
        set
        {
            if (_currentHealthPoints == value || isDead) return;
            if (value <= 0)
            {
                isDead = true;
                _currentHealthPoints = 0;
                OnHealthPointsEmpty?.Invoke(this);
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
        defaultColor = _healthBarColor.color;
        _shieldManagerCS = GetComponent<ShieldManager>();
    }

    public void SetHealthPointsValues(float maxHP)
    {
        _maxHealthPoints = maxHP;
        currentHealthPoints = maxHealthPoints;
    }

    public void ReceiveDamage(float damageReceived)
    {
        if (_shieldManagerCS != null)
        {
            if (_shieldManagerCS.currentShieldPoints > 0)
            {
                _shieldManagerCS.ReceiveDamage(damageReceived);
                return;
            }
        }

        currentHealthPoints -= damageReceived;
        if(lowHealthWarning != null && currentHealthPoints / maxHealthPoints <= 0.3f) lowHealthWarning.SetActive(true);
    }

    public void ReceiveHealing(float healingReceived)
    {
        currentHealthPoints += healingReceived;
        if(lowHealthWarning != null && currentHealthPoints / maxHealthPoints > 0.3f) lowHealthWarning.SetActive(false);
    }

    private void AjustHealthBar()
    {
        _healthBar.localScale = new Vector3(_currentHealthPoints / _maxHealthPoints, 1, 1);
    }

    public void ChangeHealthbarColor(Color color)
    {
        _healthBarColor.color = color;
    }

    public event Action<HealthManager> OnHealthPointsEmpty;
}
