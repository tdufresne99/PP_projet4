using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    void Update()
    {
        if(Input.GetButton("Fire1")) _anim.SetBool("meleeAttack", true);
        else _anim.SetBool("meleeAttack", false);
    }
}
