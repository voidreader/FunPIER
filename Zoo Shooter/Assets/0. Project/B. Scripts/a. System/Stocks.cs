using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stocks : MonoBehaviour
{
    public static Stocks main = null;

    public List<Weapon> ListWeapons;

    private void Awake() {
        main = this;
    }
}
