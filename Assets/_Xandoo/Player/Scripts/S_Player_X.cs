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
using MLAPI.Prototyping;

public class S_Player_X : NetworkedBehaviour
{
    public Transform spellSpawnLocation;

    public SOBJ_PlayerStats_X playerStats;
    public SOBJ_Spell_X spellSettings;

    public NetworkedVarInt teamID = new NetworkedVarInt(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.ServerOnly }, -1);

	public int Health = 0;

	public SkinnedMeshRenderer[] bodyParts;


	// Start is called before the first frame update
	void Start()
    {
        
        if (IsLocalPlayer)
        {
            Animator[] children = GetComponentsInChildren<Animator>();
            children[0].gameObject.SetActive(true);
			children[1].transform.position = new Vector3(transform.position.x, -1000, transform.position.z);
        }
        else
        {
            Animator[] children = GetComponentsInChildren<Animator>();
            children[0].gameObject.SetActive(false);
			children[1].gameObject.SetActive(true);
        }

        Health = playerStats.GetMaxHealth();

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
            SpawnProjectile(OwnerClientId);
        }
        else
        {
            InvokeServerRpc(RequestSpawnProjectileOnServer, OwnerClientId);
        }
    }

    [ServerRPC]
    void RequestSpawnProjectileOnServer(ulong requestingPlayer)
    {
        SpawnProjectile(requestingPlayer);
    }

    void SpawnProjectile(ulong spawningPlayer)
    {
        GameObject spell = Instantiate(spellSettings.GetProjectile(), spellSpawnLocation.position, spellSpawnLocation.rotation);
        spell.GetComponent<NetworkedObject>().Spawn();
        //spell.GetComponent<Rigidbody>().AddForce(spell.transform.forward * spellSettings.GetSpeed());
		spell.GetComponent<S_Projectile_X>().Init(spawningPlayer);
		//Debug.Log(OwnerClientId);
		//spell.GetComponent<S_Projectile_X>().owningPlayer = this;
    }

    public void Heal(int amount)
    {
        Health += amount;
        Health = Mathf.Clamp(Health, 0, playerStats.GetMaxHealth());
		GetComponent<S_PlayerCanvas_X>().healthBar.value = Health;
	}

	// This is the client that took damage
    public void Damage(int amount, ulong damager)
    {
		if (!GetComponent<S_PlayerMovement_X>().IsSpectating)
		{
			Health -= amount;
			Health = Mathf.Clamp(Health, 0, playerStats.GetMaxHealth());

			if (Health <= 0)
			{

				GetComponent<S_PlayerMovement_X>().SetIsSpectating(true);
				Health = playerStats.GetMaxHealth();

				S_GameManager_X.Singleton.gameMode.RespawnPlayer(this);
			}

			GetComponent<S_PlayerCanvas_X>().healthBar.value = Health;
		}
	}

	[ServerRPC]
	void AskServerToRespawn(S_Player_X p)
	{
		S_GameManager_X.Singleton.gameMode.RespawnPlayer(p);
	}

	[ClientRPC]
	public void Teleport(Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;
	}

	[ClientRPC]
	public void ChangeMaterial(Material material)
	{
		foreach (SkinnedMeshRenderer meshRenderer in bodyParts)
		{
			meshRenderer.material = material;
		}
	}
}
