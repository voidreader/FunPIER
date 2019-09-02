using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Machine", menuName = "Machine")]
public class Machine : ScriptableObject {

    public int Level; // 사실상 ID
    

    public Sprite SpriteMergeUI;
    public Sprite SpriteBody,SpriteLeg;
    public Sprite SpriteFaceIdle, SpriteFaceShoot;

    [TextArea] public string DisplayName;


}
