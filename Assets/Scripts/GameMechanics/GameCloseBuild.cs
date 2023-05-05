using UnityEngine;
using UnityEditor;

public class GameCloseBuild : MonoBehaviour
{
    public void CloseGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

