using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialsEnemy : MonoBehaviour
{
    private HealthManager _healthManagerCS;
    void Start()
    {
        _healthManagerCS = GetComponent<HealthManager>();
        if(_healthManagerCS != null)
        {
            _healthManagerCS.OnHealthPointsEmpty += OnTrialEnemyEliminated;
        }
        else Debug.LogWarning("No HealthManager component found on " + gameObject.name + " (TrialsEnemy.cs)");
    }
    public void ToggleActivity(bool active)
    {
        gameObject.SetActive(active);
    }
    void OnTrialEnemyEliminated(HealthManager hm)
    {
        TrialsManager.instance?.OnTrialSuccess();
    }
    void OnDestroy()
    {
        if(_healthManagerCS != null) _healthManagerCS.OnHealthPointsEmpty -= OnTrialEnemyEliminated;
    }
}
