using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BlockBase))]
public class object_02_brick01 : MonoBehaviour
{
    public BlockBase Base { get; private set; }

    void Awake()
    {
        Base = GetComponent<BlockBase>();
    }


}
