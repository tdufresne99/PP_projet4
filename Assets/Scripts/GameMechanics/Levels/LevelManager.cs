using System;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Enemy;
using System.Collections;
using TMPro;

public class LevelManager : MonoBehaviour
{
    #region Calculated Values
    private GameObject _currentLevelGO;
    private LevelsIntro _currentLevelsIntroCS;
    [SerializeField] private SkillsUpgradesInformations _skillsUpgradesInformationsCS;
    private int _maxLevelIndex;
    [SerializeField] private List<EnemyDamageReceiver> _currentEnemiesInLevel;
    public int enemyCount => _currentEnemiesInLevel.Count;
    #endregion

    public PlayerStateManager playerStateManagerCS;
    [SerializeField] private List<PlayerAbilityEnum> _playerPotentialSkillUpgrades = new List<PlayerAbilityEnum>{
        PlayerAbilityEnum.SpreadFire,
        PlayerAbilityEnum.SpreadFire,
        PlayerAbilityEnum.LightningRain,
        PlayerAbilityEnum.LightningRain,
        PlayerAbilityEnum.IceShield,
        PlayerAbilityEnum.IceShield,
        PlayerAbilityEnum.NaturesMelody,
        PlayerAbilityEnum.NaturesMelody,
    };
    [SerializeField] private GameObject _choicesParentGO;
    [SerializeField] private GameObject[] _choicesGOs;
    [SerializeField] private GameObject[] _levelsGOs;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private Vector3 _origin = Vector3.zero;
    [SerializeField] private Transform _playerReset;
    [SerializeField] private GameObject _fadeOutGo;
    [SerializeField] private GameObject _fadeInGo;

    private float _fadeOutTime = 3.2f;
    private Coroutine _coroutineResetLevel;
    private Coroutine _couroutineNextLevel;


    [SerializeField] private int _currentLevel = 1;
    public int currentLevel
    {
        get => _currentLevel;
        set
        {
            if (_currentLevel == value) return;
            if (value < 1) value = 1;
            _currentLevel = value;
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
        SubscribeToRequiredEvents();

        playerStateManagerCS.healthManagerCS.OnHealthPointsEmpty += OnLevelFailed;

        _maxLevelIndex = _levelsGOs.Length;

        StartLevel();
    }

    private void SubscribeToRequiredEvents()
    {

    }

    private void StartLevel()
    {
        FadeCanvasToggle(true);
        playerStateManagerCS.ResetPlayer(_playerReset);
        InstanciateLevel();
        _currentLevelsIntroCS.ToggleActivity(true);
    }

    private void InstanciateLevel()
    {
        var randomLevelIndex = UnityEngine.Random.Range(0, _levelsGOs.Length);

        _currentLevelGO = Instantiate(_levelsGOs[randomLevelIndex], _origin, Quaternion.identity);

        _currentLevelsIntroCS = _currentLevelGO.GetComponentInChildren<LevelsIntro>();
        if (_currentLevelsIntroCS == null) Debug.LogError("No LevelsIntro component found in " + _currentLevelGO.name + "'s children (LevelsManager.cs)");
        else _currentLevelsIntroCS.ToggleActivity(false);

        GetEnemiesInLevel();
    }

    private void GetEnemiesInLevel()
    {
        var enemiesInLevel = _currentLevelGO.GetComponentsInChildren<EnemyDamageReceiver>();
        _currentEnemiesInLevel = new List<EnemyDamageReceiver>(enemiesInLevel.Length);
        foreach (var enemy in enemiesInLevel)
        {
            _currentEnemiesInLevel.Add(enemy);
            enemy.GetComponent<HealthManager>().OnHealthPointsEmpty += OnEnemyDeath;
        }
    }

    private void OnEnemyDeath(HealthManager enemyToRemove)
    {
        var dmgReceiver = enemyToRemove.GetComponent<EnemyDamageReceiver>();
        if(dmgReceiver == null) 
        {
            Debug.LogWarning("No EnemyDamageReceiver found on " + enemyToRemove.gameObject.name + ", enemy will not be Destroyed properly (LevelManager.cs).");
            return;
        } 
        if(_currentEnemiesInLevel.Contains(dmgReceiver))
        {
            _currentEnemiesInLevel.Remove(dmgReceiver);
            enemyToRemove.OnHealthPointsEmpty -= OnEnemyDeath;
        }

        if(enemyCount <= 0) OnLevelCompleted();
    }

    private void OnLevelFailed(HealthManager hm)
    {
        if(playerStateManagerCS.currentLifes > 1)
        {
            playerStateManagerCS.currentLifes--;
            _coroutineResetLevel = StartCoroutine(CoroutinResetLevel());
        }
        else OnGameOver();
    }

    private IEnumerator CoroutinResetLevel()
    {
        FadeCanvasToggle(false);
        yield return new WaitForSecondsRealtime(_fadeOutTime);
        Destroy(_currentLevelGO);
        StartLevel();
    }

    private void OnLevelCompleted()
    {
        _couroutineNextLevel = StartCoroutine(CoroutineNextLevel());
    }

    private IEnumerator CoroutineNextLevel()
    {
        FadeCanvasToggle(false);
        yield return new WaitForSecondsRealtime(_fadeOutTime);
        Destroy(_currentLevelGO);
        currentLevel++;
        StartLevel();
    }

    public void OnSkillUpOrbPickedUp()
    {
        _choicesParentGO.SetActive(true);

        var tempPotentialSkillUps = _playerPotentialSkillUpgrades;
        var chosenSkills = new List<PlayerAbilityEnum>(_choicesGOs.Length);

        for (int i = 0; i < _choicesGOs.Length; i++)
        {
            int index = UnityEngine.Random.Range(0, tempPotentialSkillUps.Count);
            var chosenSkill = tempPotentialSkillUps[i];
            chosenSkills.Add(chosenSkill);
            tempPotentialSkillUps.Remove(chosenSkill);
        }

        for (int i = 0; i < chosenSkills.Count; i++)
        {
            var choiceGO = _choicesGOs[i];

            var ability = chosenSkills[i];
            int abilityLevel;
            switch (ability)
            {
                case PlayerAbilityEnum.SpreadFire :
                abilityLevel = playerStateManagerCS.spreadFireLevel;
                break;

                case PlayerAbilityEnum.LightningRain :
                abilityLevel = playerStateManagerCS.lightningRainLevel;
                break;
                
                case PlayerAbilityEnum.IceShield :
                abilityLevel = playerStateManagerCS.iceShieldLevel;
                break;

                case PlayerAbilityEnum.NaturesMelody :
                abilityLevel = playerStateManagerCS.naturesMelodyLevel;
                break;

                default:
                abilityLevel = 0;
                break;
            }

            var skillInfos = _skillsUpgradesInformationsCS.GetSkillsInformations(ability, abilityLevel - 1);

            var skillUpChoice = choiceGO.GetComponent<SkillUpgradeChoice>();
            if(skillUpChoice == null) 
            {
                Debug.LogWarning("No skillUpChoice script found on " + skillUpChoice.name + "(LevelManager.cs)");
                continue;
            }
            skillUpChoice.DisplaySkillUpgradeInformation(ability, skillInfos[0], skillInfos[1]);
        }

    }

    private void OnGameOver()
    {
        Debug.Log("GameOver");
    }

    public void FadeCanvasToggle(bool fadeIn)
    {
        if(_fadeInGo == null)
        {
            Debug.LogWarning(_fadeInGo.name + "is null (LevelManager.cs)");
            return;
        }
        if(_fadeOutGo == null)
        {
            Debug.LogWarning(_fadeOutGo.name + "is null (LevelManager.cs)");
            return;
        }
        _fadeInGo?.SetActive(fadeIn);
        _fadeOutGo?.SetActive(!fadeIn);
    }

    public event Action OnLevelStart;
}