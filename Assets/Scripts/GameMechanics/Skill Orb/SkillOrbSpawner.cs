using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillOrbSpawner : MonoBehaviour
{
    static public void InstantiateSkillOrb(GameObject skillOrb, Vector3 position)
    {
        Instantiate(skillOrb, position, Quaternion.identity);
    }
}
