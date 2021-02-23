using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using MLAPI.Serialization;
using System.Security.Policy;
using System.IO;
using MLAPI.Serialization.Pooled;

public class S_Player_X : NetworkedBehaviour
{
    public Transform spellSpawnLocation;

    public SOBJ_PlayerStats_X playerStats;
    public SOBJ_Spell_X spellSettings;

    public NetworkedVarInt teamID = new NetworkedVarInt(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.ServerOnly }, -1);

	public NetworkedVarInt Health = new NetworkedVarInt(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.OwnerOnly });

	public SkinnedMeshRenderer[] bodyParts;


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

	[ClientRPC]
	public void SetPosition(Vector3 pos)
	{
		transform.position = pos;
	}

	[ClientRPC]
	public void ChangeMaterial(Material material)
	{
		foreach (SkinnedMeshRenderer meshRenderer in bodyParts)
		{
			Debug.Log("Set " + meshRenderer + " materials to " + material);
			meshRenderer.material = material;
		}
	}
}
