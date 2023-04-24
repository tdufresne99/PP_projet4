using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayersEnum : MonoBehaviour
{
    private static LayersEnum _instance;
    public static LayersEnum instance => _instance;

    [SerializeField] private LayerMask _groundLayer;
    public int GroundLayer => _groundLayer.value;

    void Awake()
    {
        if(_instance == null) _instance = this;
        else Destroy(this);
    }
}
