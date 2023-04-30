using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    public void OnContinue()
    {
        GetComponentInParent<Animator>().SetTrigger("fadeOut");
        TrialsManager.instance?.OnContinue();
    }
}
