using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    public void OnContinue()
    {
        var anim = GetComponentInParent<Animator>();
        if(anim != null) anim.SetTrigger("fadeOut");
        else Debug.LogWarning("No Animator component found in " + gameObject.name + "'s parent (ContinueButton.cs)");
        TrialsManager.instance?.OnContinue();
    }
}
