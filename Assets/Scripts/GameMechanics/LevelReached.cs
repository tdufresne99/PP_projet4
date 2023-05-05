using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelReached : MonoBehaviour
{
    private TextMeshProUGUI _levelReachedText;
    private string _text = "Vous avez atteint le niveau ";

    void Start()
    {
        _levelReachedText = GetComponent<TextMeshProUGUI>();
        _levelReachedText.text = _text + GameManager.instance.levelReached;
    }


}
