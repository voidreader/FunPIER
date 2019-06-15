using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BlockBase))]
public class object_01_entry : MonoBehaviour {

    public BlockBase Base { get; private set; }

    void Awake()
    {
        Base = GetComponent<BlockBase>();
    }


}
