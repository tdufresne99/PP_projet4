using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialsEnemy : MonoBehaviour
{
    public void ToggleActivity(bool active)
    {
        Debug.Log("SetActiveFalse");
        gameObject.SetActive(active);
    }
    void OnDestroy()
    {
        TrialsManager.instance?.OnTrialCompleted();
    }
}
