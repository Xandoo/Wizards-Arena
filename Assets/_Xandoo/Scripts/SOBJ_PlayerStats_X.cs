﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/Player/Player Stats")]
public class SOBJ_PlayerStats_X : ScriptableObject
{
    [SerializeField]
    int maxHealth = 100;
    [SerializeField]
    int maxMana = 100;
    [SerializeField]
    int maxArmor = 100;


    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public int GetMaxMana()
    {
        return maxMana;
    }
    public int GetMaxArmor()
    {
        return maxArmor;
    }
    
    /*public int Health
    {
        get
        {
            return Health;
        }

        set
        {
            Health = value;
            Mathf.Clamp(Health, 0, maxHealth);
        }
    }*/

}
