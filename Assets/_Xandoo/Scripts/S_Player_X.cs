using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;

public class S_Player_X : NetworkedBehaviour
{
    public Transform spellSpawnLocation;

    public SOBJ_PlayerStats_X playerStats;
    public SOBJ_Spell_X spellSettings;

    public int health;

    public NetworkedVar<int> Health = new NetworkedVar<int>(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.OwnerOnly });
    public NetworkedVar<int> Mana = new NetworkedVar<int>(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.OwnerOnly });
    public NetworkedVar<int> Armor = new NetworkedVar<int>(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.OwnerOnly });

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

        Health.Value = playerStats.GetMaxHealth();
        health = playerStats.GetMaxHealth();
        Mana.Value = playerStats.GetMaxMana();
        //Armor.Value = playerStats.GetMaxHealth();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator CastSpell()
    {
        yield return new WaitForSeconds(spellSettings.GetCastTime());
        if (IsHost)
        {
            SpawnProjectile();
        }
        else
        {
            InvokeServerRpc(RequestSpawnProjectileOnServer);
        }
    }

    [ServerRPC]
    void RequestSpawnProjectileOnServer()
    {
        SpawnProjectile();
    }

    void SpawnProjectile()
    {
        GameObject spell = Instantiate(spellSettings.GetTrailFX(), spellSpawnLocation.position, spellSpawnLocation.rotation);
        spell.GetComponent<NetworkedObject>().Spawn();
        spell.GetComponent<Rigidbody>().AddForce(spell.transform.forward * spellSettings.GetSpeed());
    }

    public void Heal(int amount)
    {
        Health.Value += amount;
        health += amount;
        Mathf.Clamp(Health.Value, 0, playerStats.GetMaxHealth());
    }

    public void Damage(int amount)
    {
        Health.Value -= amount;
        health -= amount;
        Mathf.Clamp(Health.Value, 0, playerStats.GetMaxHealth());
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit");
    }
}
