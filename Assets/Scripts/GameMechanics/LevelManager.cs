using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int _currentLevel = 0;
    public int currentLevel
    {
        get => _currentLevel;
        set
        {
            if (_currentLevel == value) return;
            if (value < 0) value = 0;
            _currentLevel = value;
            OnLevelChange.Invoke();
        }
    }

    private static LevelManager _instance;
    public static LevelManager instance => _instance;

    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);
    }

    void Start()
    {
        
    }

    public void MoveToNextLevel(bool tutorial)
    {
        currentLevel++;
    }

    public event Action OnLevelChange;
}