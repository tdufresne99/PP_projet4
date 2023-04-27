using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbDropperOnDeath : MonoBehaviour
{
    private HealthManager _healthManagerCS;
    [SerializeField] private GameObject _orbGO;

    void Start()
    {
        _healthManagerCS = GetComponent<HealthManager>();
        if(_healthManagerCS == null)
        {
            Debug.LogWarning("HealthManager not found on " + gameObject.name + " (OrbDropperOnDeath.cs)");
            Destroy(this);
        }
        else
        {
            _healthManagerCS.OnHealthPointsEmpty += InstanciateOrb;
        }
    }

    void InstanciateOrb()
    {
        Instantiate(_orbGO, transform.position, Quaternion.identity);
    }
}