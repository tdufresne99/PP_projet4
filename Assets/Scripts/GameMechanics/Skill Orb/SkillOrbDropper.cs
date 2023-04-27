using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillOrbDropper : MonoBehaviour
{
    [SerializeField] private GameObject _skillOrb;   
    void OnDestroy()
    {
        SkillOrbSpawner.InstantiateSkillOrb(_skillOrb, transform.position);
    }
}
