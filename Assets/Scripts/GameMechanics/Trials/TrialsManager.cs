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
    [SerializeField] private Transform _playerReset;

    [SerializeField] private GameObject _fadeOut;
    [SerializeField] private GameObject _fadeIn;

    [SerializeField] private int _maxTutorialIndex;

    private Coroutine _couroutineNextTrial;
    private float _fadeOutTime = 3.2f;
    private static TrialsManager _instance;
    public static TrialsManager instance => _instance;

    void Awake()
    {
        if(_instance == null) _instance = this;
        else Destroy(this);
    }

    void Start()
    {
        playerStateManagerCS.healthManagerCS.OnHealthPointsEmpty += OnTrialFailed;

        _maxTutorialIndex = _trialsGOs.Length;
        OnBlockPlayerActions?.Invoke(true);
        StartTrial();
    }

    public void StartTrial()
    {
        FadeCanvasToggle(true);
        playerStateManagerCS.ResetPlayer(_playerReset);
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
        _currentTrialsEnemyCS.gameObject?.SetActive(true);
    }

    public void OnTrialFailed()
    {
        Debug.Log("Trial Failed");
        _couroutineNextTrial = StartCoroutine(CoroutinResetTrial());
    }

    private IEnumerator CoroutinResetTrial()
    {
        FadeCanvasToggle(false);
        yield return new WaitForSecondsRealtime(_fadeOutTime);
        Destroy(_currentTrialGO);
        StartTrial();
    }

    public void OnTrialSuccess()
    {
        Debug.Log("Trial success");
        if(_currentTrialIndex >= _maxTutorialIndex - 1)
        {
            // Start Lvl 1...
            OnTrialsCompleted();
            return;
        }
        _couroutineNextTrial = StartCoroutine(CoroutineNextTrial());
    }

    private IEnumerator CoroutineNextTrial()
    {
        FadeCanvasToggle(false);
        yield return new WaitForSecondsRealtime(_fadeOutTime);
        Destroy(_currentTrialGO);
        _currentTrialIndex++;
        StartTrial();
    }

    private void OnTrialsCompleted()
    {
        Debug.Log("Trials completed");
    } 

    public void FadeCanvasToggle(bool fadeIn)
    {
        if(_fadeIn == null)
        {
            Debug.LogWarning(_fadeIn.name + "is null (TrialsManager.cs)");
            return;
        }
        if(_fadeOut == null)
        {
            Debug.LogWarning(_fadeOut.name + "is null (TrialsManager.cs)");
            return;
        }
        _fadeIn?.SetActive(fadeIn);
        _fadeOut?.SetActive(!fadeIn);
    }

    public event Action OnTrialStart;
    public event Action<bool> OnBlockPlayerActions;
}
