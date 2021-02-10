using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class S_Player_X : NetworkedBehaviour
{
    public Transform spellSpawnLocation;

    public SOBJ_PlayerStats_X playerStats;
    public SOBJ_Spell_X spellSettings;

    // Start is called before the first frame update
    void Start()
    {
        
        if (IsLocalPlayer)
        {
            Animator[] children = GetComponentsInChildren<Animator>();
            children[0].gameObject.SetActive(true);
            children[1].gameObject.SetActive(false);
        }
        else
        {
            Animator[] children = GetComponentsInChildren<Animator>();
            children[1].gameObject.SetActive(true);
            children[0].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CastSpell()
    {
        GameObject spell = Instantiate(spellSettings.GetTrailFX(), spellSpawnLocation.position, spellSpawnLocation.rotation);
        spell.GetComponent<NetworkedObject>().Spawn();
        spell.GetComponent<Rigidbody>().AddForce(spell.transform.forward * spellSettings.GetSpeed());
    }

    public void Heal(int amount)
    {
       playerStats.Health += amount;
    }

    public void Damage(int amount)
    {
        playerStats.Health -= amount;

    }
}
