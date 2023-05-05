using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private bool PlayerUnlockedAllSkillUps = false;
    [SerializeField]
    private PlayerAbilityEnum[] _playerUpgradableSkills = new PlayerAbilityEnum[] {
        PlayerAbilityEnum.SpreadFire,
        PlayerAbilityEnum.LightningRain,
        PlayerAbilityEnum.IceShield,
        PlayerAbilityEnum.NaturesMelody,
    };
    [SerializeField] private Dictionary<PlayerAbilityEnum, int> playerSkillUps;
    private List<List<PlayerAbilityEnum>> playerAvailableSkillUps = new List<List<PlayerAbilityEnum>>();
    [SerializeField] private GameObject _skillUpCanvasGO;
    [SerializeField] private GameObject _skillUpOrbGO;
    [SerializeField] private Transform _skillUpOrbSpawnPoint;

    [SerializeField] private Animator _choicesAnimator;
    [SerializeField] private GameObject _choicesHolder;
    [SerializeField] private GameObject _choiceGO;
    [SerializeField] private int _maxSkillUpsToDisplay = 3;

    [SerializeField] private GameObject[] _levelsGOs;
    [SerializeField] private Vector3 _origin = Vector3.zero;
    [SerializeField] private PlayerStateManager _playerStateManagerCS;
    [SerializeField] private Transform _playerReset;
    [SerializeField] private TextMeshProUGUI _lifesText;
    [SerializeField] private GameObject _fadeOutGo;
    [SerializeField] private GameObject _fadeInGo;

    [SerializeField] private AudioClip skillUpSound;

    private AudioSource LevelAudioSource;

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

    private bool _playerPickedUpSkillUpOrb = false;

    private static LevelManager _instance;
    public static LevelManager instance => _instance;

    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);
    }

    void Start()
    {
        LevelAudioSource = GetComponent<AudioSource>();

        SubscribeToRequiredEvents();

        playerStateManagerCS.healthManagerCS.OnHealthPointsEmpty += OnLevelFailed;

        _maxLevelIndex = _levelsGOs.Length;

        StartLevel();

        GetCurrentAvailableSkillUps();
    }

    private void GetCurrentAvailableSkillUps()
    {

        playerAvailableSkillUps = new List<List<PlayerAbilityEnum>>(2);

        for (int level = 0; level + 2 <= _playerStateManagerCS.spreadFireMaxLevel; level++)
        {
            if (level > playerAvailableSkillUps.Count - 1) playerAvailableSkillUps.Add(new List<PlayerAbilityEnum>(_playerUpgradableSkills.Length));

            playerAvailableSkillUps[level].Add(PlayerAbilityEnum.SpreadFire);
        }
        for (int level = 0; level + 2 <= _playerStateManagerCS.lightningRainMaxLevel; level++)
        {
            if (level > playerAvailableSkillUps.Count - 1) playerAvailableSkillUps.Add(new List<PlayerAbilityEnum>(_playerUpgradableSkills.Length));
            playerAvailableSkillUps[level].Add(PlayerAbilityEnum.LightningRain);
        }
        for (int level = 0; level + 2 <= _playerStateManagerCS.iceShieldMaxLevel; level++)
        {
            if (level > playerAvailableSkillUps.Count - 1) playerAvailableSkillUps.Add(new List<PlayerAbilityEnum>(_playerUpgradableSkills.Length));
            playerAvailableSkillUps[level].Add(PlayerAbilityEnum.IceShield);
        }
        for (int level = 0; level + 2 <= _playerStateManagerCS.naturesMelodyMaxLevel; level++)
        {
            if (level > playerAvailableSkillUps.Count - 1) playerAvailableSkillUps.Add(new List<PlayerAbilityEnum>(_playerUpgradableSkills.Length));
            playerAvailableSkillUps[level].Add(PlayerAbilityEnum.NaturesMelody);
        }

        // for (int i = 0; i < playerAvailableSkillUps.Count; i++)
        // {
        //     for (int y = 0; y < playerAvailableSkillUps[i].Count; y++)
        //     {
        //         Debug.Log("list i=" + i + ", value=" + playerAvailableSkillUps[i][y]);
        //     }
        // }
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

        if (_currentLevel % 3 == 0 && _currentLevel != 0 && !_playerPickedUpSkillUpOrb) Instantiate(_skillUpOrbGO, _skillUpOrbSpawnPoint.position, Quaternion.identity, _currentLevelGO.transform);

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
        if (dmgReceiver == null)
        {
            Debug.LogWarning("No EnemyDamageReceiver found on " + enemyToRemove.gameObject.name + ", enemy will not be Destroyed properly (LevelManager.cs).");
            return;
        }
        if (_currentEnemiesInLevel.Contains(dmgReceiver))
        {
            _currentEnemiesInLevel.Remove(dmgReceiver);
            enemyToRemove.OnHealthPointsEmpty -= OnEnemyDeath;
        }

        if (enemyCount <= 0) OnLevelCompleted();
    }

    private void OnLevelFailed(HealthManager hm)
    {
        if (playerStateManagerCS.currentLifes > 1)
        {
            playerStateManagerCS.currentLifes--;
            _lifesText.text = playerStateManagerCS.currentLifes + "";
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
        _playerPickedUpSkillUpOrb = false;
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
        if (PlayerUnlockedAllSkillUps) return;
        var playerAvailableSkillUpsTemp = new List<List<PlayerAbilityEnum>>();
        foreach (var item in playerAvailableSkillUps)
        {
            var newItem = new List<PlayerAbilityEnum>(item);
            playerAvailableSkillUpsTemp.Add(newItem);
        }
        bool canvasUp = false;


        int lastInstantiatedSkillUpIndex = -1;
        for (int i = 0; i < _maxSkillUpsToDisplay; i++)
        {
            bool skillUpsRemaining = false;
            for (int y = 0; y < playerAvailableSkillUpsTemp.Count; y++)
            {
                if (playerAvailableSkillUpsTemp[y].Count > 0)
                {
                    skillUpsRemaining = true;
                    break;
                }
            }
            if (skillUpsRemaining == false)
            {
                Debug.Log("Player unlocked all the skills, gg!");
                PlayerUnlockedAllSkillUps = true;
                return;
            }
            if (canvasUp == false)
            {
                ToggleSkillUpCanvas();
                canvasUp = true;
            }
            for (int levelIndex = 0; levelIndex < playerAvailableSkillUpsTemp.Count; levelIndex++)
            {
                if (playerAvailableSkillUpsTemp[levelIndex].Count < 1 && lastInstantiatedSkillUpIndex == levelIndex) return;
                if (playerAvailableSkillUpsTemp[levelIndex].Count < 1) continue;

                int randomSkillIndex = UnityEngine.Random.Range(0, playerAvailableSkillUpsTemp[levelIndex].Count);

                var drawnSkillUp = playerAvailableSkillUpsTemp[levelIndex][randomSkillIndex];

                playerAvailableSkillUpsTemp[levelIndex].Remove(drawnSkillUp);

                var skillInfos = _skillsUpgradesInformationsCS.GetSkillsInformations(drawnSkillUp, levelIndex);

                var instantiatedSkillUpChoice = Instantiate(_choiceGO, _choicesHolder.transform);
                lastInstantiatedSkillUpIndex = levelIndex;

                var skillUpChoice = instantiatedSkillUpChoice.GetComponent<SkillUpgradeChoice>();

                if (skillUpChoice == null)
                {
                    Debug.LogWarning("No skillUpChoice script found on " + skillUpChoice.name + "(LevelManager.cs)");
                    return;
                }

                skillUpChoice.DisplaySkillUpgradeInformation(drawnSkillUp, skillInfos[0], skillInfos[1]);

                break;
            }
        }
    }

    public void OnSkillUpChosen(PlayerAbilityEnum ability)
    {
        _playerPickedUpSkillUpOrb = true;
        LevelAudioSource.PlayOneShot(skillUpSound);

        int chosenSkillUpLevel;
        switch (ability)
        {
            case PlayerAbilityEnum.SpreadFire:
                playerStateManagerCS.spreadFireLevel++;
                chosenSkillUpLevel = playerStateManagerCS.spreadFireLevel;
                break;

            case PlayerAbilityEnum.LightningRain:
                playerStateManagerCS.lightningRainLevel++;
                chosenSkillUpLevel = playerStateManagerCS.lightningRainLevel;
                break;

            case PlayerAbilityEnum.IceShield:
                playerStateManagerCS.iceShieldLevel++;
                chosenSkillUpLevel = playerStateManagerCS.iceShieldLevel;
                break;

            case PlayerAbilityEnum.NaturesMelody:
                playerStateManagerCS.naturesMelodyLevel++;
                chosenSkillUpLevel = playerStateManagerCS.naturesMelodyLevel;
                break;

            default:
                Debug.LogError("Ability type " + ability + " not supported as chosen skill up...");
                return;
        }

        playerAvailableSkillUps[chosenSkillUpLevel - 2].Remove(ability);

        _choicesAnimator.SetTrigger("fadeOut");
        Invoke("ToggleSkillUpCanvas", 3f);
    }

    private void ToggleSkillUpCanvas()
    {

        if (_skillUpCanvasGO == null)
        {
            Debug.LogWarning("Choices canvas is null (LevelManager.cs)");
            return;
        }
        if (_skillUpCanvasGO.activeSelf)
        {
            for (int i = 0; i < _choicesHolder.transform.childCount; i++)
            {
                GameObject.Destroy(_choicesHolder.transform.GetChild(i).gameObject);
            }
        }
        _skillUpCanvasGO?.SetActive(!_skillUpCanvasGO.activeSelf);
    }

    private void OnGameOver()
    {
        GameManager.instance.levelReached = _currentLevel;
        SceneManager.LoadScene(GameManager.instance.gameOverSceneIndex);
    }

    public void FadeCanvasToggle(bool fadeIn)
    {
        if (_fadeInGo == null)
        {
            Debug.LogWarning(_fadeInGo.name + "is null (LevelManager.cs)");
            return;
        }
        if (_fadeOutGo == null)
        {
            Debug.LogWarning(_fadeOutGo.name + "is null (LevelManager.cs)");
            return;
        }
        _fadeInGo?.SetActive(fadeIn);
        _fadeOutGo?.SetActive(!fadeIn);
    }

    public event Action OnLevelStart;
}