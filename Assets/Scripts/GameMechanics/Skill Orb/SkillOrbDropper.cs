using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillOrbDropper : MonoBehaviour
{
    private HealthManager _healthManagerCS;
    [SerializeField] private GameObject _skillOrb;

    void Start()
    {
        _healthManagerCS = GetComponent<HealthManager>();
        if(_healthManagerCS == null)
        {
            Debug.LogWarning("HealthManager not found on " + gameObject.name + " (SkillOrbDropper.cs)");
            Destroy(this);
        }
        else
        {
            _healthManagerCS.OnHealthPointsEmpty += SpawnSkillOrb;
        }
    }

    void SpawnSkillOrb()
    {
        Instantiate(_skillOrb, transform.position, Quaternion.identity);
    }
}
