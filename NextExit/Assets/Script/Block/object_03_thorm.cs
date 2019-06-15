using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BlockBase))]
public class object_03_thorm : MonoBehaviour
{
    public BlockBase Base { get; private set; }

    void Awake()
    {
        Base = GetComponent<BlockBase>();
    }


}
