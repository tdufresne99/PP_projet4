using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLoadScene : MonoBehaviour
{
    public void LoadGameScene()
    {
        if (GameManager.instance != null) GameManager.instance.ButtonLoadGameScene();
        else Debug.LogWarning("Could not load game scene since GameManager is null (ButtonLoadScene.cs)");
    }
    public void LoadMenuScene()
    {
        if (GameManager.instance != null) GameManager.instance.ButtonLoadMenuScene();
        else Debug.LogWarning("Could not load menu scene since GameManager is null (ButtonLoadScene.cs)");
    }
}
