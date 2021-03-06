using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumReview : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        DamageFlags currentDmgFlags = DamageFlags.IsCrt | DamageFlags.IsPoision;
        currentDmgFlags |= DamageFlags.IsWind;

        currentDmgFlags ^= DamageFlags.IsCrt;

        if ((currentDmgFlags & DamageFlags.IsWind) != 0)
            Debug.Log("Wind include");
        if ((currentDmgFlags & DamageFlags.IsCrt) != 0)
            Debug.Log("crt include");
        if ((currentDmgFlags & DamageFlags.IsPoision) != 0)
            Debug.Log("Poision include");
        if ((currentDmgFlags & DamageFlags.IsFire) != 0)
            Debug.Log("fire include");
        if ((currentDmgFlags & DamageFlags.IsIce) != 0)
            Debug.Log("Ice include");
    }

}

[Flags]
public enum DamageFlags {
    None = 0,
    IsCrt = 1,
    IsFire = 2,
    IsPoision = 4,
    IsIce = 8,
    IsWind = 16
}