using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element
{
    Fire,
    Water,
    Poison,
    Lightning
}

[CreateAssetMenu(fileName = "Spell", menuName = "Scriptable Objects/Spell")]
public class SOBJ_Spell_X : ScriptableObject
{
    [SerializeField]
    Element element;
    [SerializeField]
    [Tooltip("Time in seconds to cast spell.")]
    float castTime = 0.4f;
    [SerializeField]
    float cooldown = 1f;
    [SerializeField]
    float speed = 5f;
    [SerializeField]
    int damage = 3;
    [SerializeField]
    int damageOverTime = 2;
    [SerializeField]
    float areaOfEffect = 4f;
    [SerializeField]
    GameObject preCastFX;
    [SerializeField]
    GameObject trailFX;
    [SerializeField]
    GameObject hitFX;

    public Element GetElement()
    {
        return element;
    }

    public float GetCastTime()
    {
        return castTime;
    }

    public int GetDamage()
    {
        return damage;
    }

    public int GetDamageOverTime()
    {
        return damageOverTime;
    }

    public float GetAreaOfEffect()
    {
        return areaOfEffect;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetCooldown()
    {
        return cooldown;
    }

    public GameObject GetPreCastFX()
    {
        return preCastFX;
    }
    
    public GameObject GetTrailFX()
    {
        return trailFX;
    }
    
    public GameObject GetHitFX()
    {
        return hitFX;
    }

}
