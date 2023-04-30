using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using System;

public class TrialsManager : MonoBehaviour
{
    public PlayerStateManager playerStateManagerCS;
    [SerializeField] private GameObject[] _trialsGOs;

    [SerializeField] private GameObject _currentTrialGO;
    [SerializeField] private TrialsIntro _currentTrialsIntroCS;
    [SerializeField] private TrialsSkillDescription _currentTrialsSkillDescriptionCS;
    [SerializeField] private TrialsEnemy _currentTrialsEnemyCS;
    [SerializeField] private int _currentTrialIndex = 0;

    [SerializeField] private Vector3 _origin = Vector3.zero;

    [SerializeField] private GameObject _fadeOut;
    [SerializeField] private GameObject _fadeIn;

    [SerializeField] private int _maxTutorialIndex;

    private static TrialsManager _instance;
    public static TrialsManager instance => _instance;

    void Awake()
    {
        if(_instance == null) _instance = this;
        else Destroy(this);
    }

    void Start()
    {
        _maxTutorialIndex = _trialsGOs.Length;
        OnBlockPlayerActions?.Invoke(true);
        StartTrial();
    }

    public void StartTrial()
    {
        FadeCanvasToggle(true);
        InstanciateTrial();
        _currentTrialsIntroCS.ToggleActivity(true);
        OnTrialStart?.Invoke();
        OnBlockPlayerActions?.Invoke(false);
    }

    private void InstanciateTrial()
    {
        _currentTrialGO = Instantiate(_trialsGOs[_currentTrialIndex], _origin, Quaternion.identity);
        GetTrialNecessaryReferences();
    }

    private void GetTrialNecessaryReferences()
    {
        _currentTrialsIntroCS = _currentTrialGO.GetComponentInChildren<TrialsIntro>();
        if (_currentTrialsIntroCS == null) Debug.LogError("No TrialsIntro component found in " + _currentTrialGO.name + "'s children (TrialsManager.cs)");
        else _currentTrialsIntroCS.ToggleActivity(false);

        _currentTrialsSkillDescriptionCS = _currentTrialGO.GetComponentInChildren<TrialsSkillDescription>();
        if (_currentTrialsSkillDescriptionCS == null) Debug.LogError("No TrialsSkillDescription component found in " + _currentTrialGO.name + "'s children (TrialsManager.cs)");
        else _currentTrialsSkillDescriptionCS.ToggleActivity(false);

        _currentTrialsEnemyCS = _currentTrialGO.GetComponentInChildren<TrialsEnemy>();
        if (_currentTrialsEnemyCS == null) Debug.LogError("No TrialsEnemy component found in " + _currentTrialGO.name + "'s children (TrialsManager.cs)");
        else _currentTrialsEnemyCS.ToggleActivity(false);
    }

    public void OnSkillOrbPickUp()
    {
        _currentTrialsSkillDescriptionCS.gameObject.SetActive(true);
        OnBlockPlayerActions?.Invoke(true);
    }

    public void OnContinue()
    {
        OnBlockPlayerActions?.Invoke(false);
        _currentTrialsEnemyCS.gameObject.SetActive(true);
    }

    public void OnTrialFailed()
    {
        Destroy(_currentTrialGO);
        StartTrial();
    }

    public void OnTrialCompleted()
    {
        FadeCanvasToggle(false);

        if(_currentTrialIndex >= _maxTutorialIndex - 1)
        {
            // Start Lvl 1...
            Debug.Log("Trials completed");
            return;
        }
        Destroy(_currentTrialGO);
        _currentTrialIndex++;
        StartTrial();
    }

    public void FadeCanvasToggle(bool fadeIn)
    {
        _fadeIn.SetActive(fadeIn);
        _fadeOut.SetActive(!fadeIn);
    }

    public event Action OnTrialStart;
    public event Action<bool> OnBlockPlayerActions;
}
