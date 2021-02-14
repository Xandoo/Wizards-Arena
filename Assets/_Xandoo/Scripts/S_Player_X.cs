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

    public int teamID = -1;

    public NetworkedVarInt Health = new NetworkedVarInt(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.OwnerOnly });

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
		spell.GetComponent<ETFXProjectileScript>().Owner = this;
    }

    public void Heal(int amount)
    {
        Health.Value += amount;
        Mathf.Clamp(Health.Value, 0, playerStats.GetMaxHealth());
    }

    public void Damage(int amount, S_Player_X damager = null)
    {
		if (damager)
		{
			Health.Value -= amount;
			Mathf.Clamp(Health.Value, 0, playerStats.GetMaxHealth());
			Debug.Log(damager.GetComponent<NetworkedObject>().NetworkId);

			if (IsHost)
			{
				UpdateHealthBarOfDamagedPlayer(GetComponent<S_PlayerCanvas_X>(), Health.Value);
			}
			
			InvokeClientRpc(UpdateHealthBarOfDamagedPlayer, new List<ulong> { damager.GetComponent<NetworkedObject>().NetworkId }, GetComponent<S_PlayerCanvas_X>(), Health.Value);
			
		}
		else
		{
			Health.Value -= amount;
			Mathf.Clamp(Health.Value, 0, playerStats.GetMaxHealth());
		}

		//GetComponent<S_PlayerCanvas_X>().healthBar.value = Health.Value;
    }

	[ClientRPC]
	private void UpdateHealthBarOfDamagedPlayer(S_PlayerCanvas_X can, int healthValue)
	{
		can.healthBar.value = healthValue;
	}
}
