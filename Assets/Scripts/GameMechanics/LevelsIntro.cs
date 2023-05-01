using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelsIntro : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelName;
    [SerializeField] private TextMeshProUGUI _level;

    void Start()
    {
        _levelName.text = "Niveau " + LevelManager.instance.currentLevel;
        _level.text = "Niveau " + LevelManager.instance.currentLevel;
    }

    public void ToggleActivity(bool active)
    {
        gameObject.SetActive(active);
    }
}