using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int menuSceneIndex = 0;
    public int trialSceneIndex = 1;
    public int levelSceneIndex = 2;
    public int gameOverSceneIndex = 3;
    public int levelReached = 0;
    public Toggle trialsToggle;
    public bool trials;
    private AudioSource _gameManagerAudioSource;
    private static GameManager _instance;
    public static GameManager instance => _instance;
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        _gameManagerAudioSource = GetComponent<AudioSource>();
    }

    public void ToggleTrials()
    {
        if(trialsToggle == null) 
        {
            Debug.LogWarning("Trials Toggle is null (GameManager.cs)");
            return;
        }

        trials = trialsToggle.isOn;
    }

    public void ButtonLoadGameScene()
    {
        int sceneIndexToLoad;
        if(trials) sceneIndexToLoad = trialSceneIndex;
        else sceneIndexToLoad = levelSceneIndex;

        SceneManager.LoadScene(sceneIndexToLoad);
    }

    public void ButtonLoadMenuScene()
    {
        SceneManager.LoadScene(menuSceneIndex);
    }

    public void PlaySoundOneShot(AudioClip clip)
    {
        _gameManagerAudioSource.PlayOneShot(clip);
    }
}
