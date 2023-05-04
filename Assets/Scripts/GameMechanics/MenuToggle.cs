using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuToggle : MonoBehaviour
{   
    private Toggle trialsToggle;
    void Start()
    {
        trialsToggle = GetComponent<Toggle>();
        if(trialsToggle == null)
        {
            Debug.LogWarning("No Toggle component found on " + gameObject.name + " (MenuToggle.cs)");
        }
        else if (GameManager.instance == null)
        {
            Debug.LogWarning("GameManager is null (MenuToggle.cs)");
        }
        else
        {
            GameManager.instance.trialsToggle = trialsToggle;
        }
    }
}
