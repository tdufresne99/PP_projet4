using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Player;
using System;

public class TrialsManager : MonoBehaviour
{
    #region Calculated Values
    private GameObject _currentTrialGO;
    private TrialsIntro _currentTrialsIntroCS;
    private TrialsSkillDescription _currentTrialsSkillDescriptionCS;
    private TrialsEnemy _currentTrialsEnemyCS;
    private int _currentTrialIndex = 0;
    private int _maxTrialIndex;
    #endregion

    #region External Values
    [Header("Links")]
    public PlayerStateManager playerStateManagerCS;
    [SerializeField] private GameObject[] _trialsGOs;
    [SerializeField] private Vector3 _origin = Vector3.zero;
    [SerializeField] private Transform _playerReset;

    [SerializeField] private GameObject _fadeOutGo;
    [SerializeField] private GameObject _fadeInGo;
    #endregion

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

        _maxTrialIndex = _trialsGOs.Length;
        StartTrial();
    }

    public void StartTrial()
    {
        FadeCanvasToggle(true);
        playerStateManagerCS.ResetPlayer(_playerReset);
        InstanciateTrial();
        _currentTrialsIntroCS.ToggleActivity(true);
        OnTrialStart?.Invoke();
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
    }

    public void OnContinue()
    {
        _currentTrialsEnemyCS.gameObject?.SetActive(true);
    }

    public void OnTrialFailed(HealthManager hm)
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
        if(_currentTrialIndex >= _maxTrialIndex - 1)
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
        FadeCanvasToggle(false);
        Invoke("LoadLevelScene", _fadeOutTime);
    } 

    private void LoadLevelScene()
    {
        SceneManager.LoadScene("Levels");
    }

    public void FadeCanvasToggle(bool fadeIn)
    {
        if(_fadeInGo == null)
        {
            Debug.LogWarning("FadeInGO is null (TrialsManager.cs)");
            return;
        }
        if(_fadeOutGo == null)
        {
            Debug.LogWarning("FadeOutGO is null (TrialsManager.cs)");
            return;
        }
        _fadeInGo?.SetActive(fadeIn);
        _fadeOutGo?.SetActive(!fadeIn);
    }

    public event Action OnTrialStart;
    public event Action<bool> OnBlockPlayerActions;
}
