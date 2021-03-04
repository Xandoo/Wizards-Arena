using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using MLAPI.Messaging;
using MLAPI.Connection;

public class S_Projectile_X : NetworkedBehaviour
{
	public GameObject muzzleFire;
	public GameObject explosionFire;
	public GameObject missileFire;
	public LayerMask layerMask;


	public ulong Owner;
	public float radius = 0.3f;

	public S_Player_X owningPlayer;
	//private float timeElapsed = 0f;
	private bool hit = false;

    // Start is called before the first frame update
    void Start()
    {
		muzzleFire.SetActive(true);
    }

	public void Init(ulong ownerClientId)
	{
		Owner = ownerClientId;
		owningPlayer = S_GameManager_X.Singleton.GetPlayerFromClientId(Owner);
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		//timeElapsed += Time.deltaTime;

		if (muzzleFire.activeSelf == true)
		{
			ParticleSystem muzPartSys = muzzleFire.GetComponent<ParticleSystem>();
			if (muzPartSys.time >= muzPartSys.main.duration)
			{
				muzzleFire.SetActive(false);
			}
		}
		
		if (explosionFire.activeSelf == true)
		{
			ParticleSystem muzPartSys = explosionFire.GetComponent<ParticleSystem>();
			if (muzPartSys.time >= muzPartSys.main.duration)
			{
				explosionFire.SetActive(false);
				if (IsHost)
				{
					Destroy(gameObject);
				}
			}
		}

		if (IsHost)
		{
			if (!hit)
			{
				Move();
			}
		}

		if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hitInfo, radius, layerMask)  && !hit)
		{

			if (IsHost)
			{
				if (hitInfo.transform.gameObject.tag.Equals("Player"))
				{
					
					ulong playerClientId = hitInfo.transform.gameObject.GetComponent<NetworkedObject>().OwnerClientId;
					
					S_Player_X damagedPlayer = S_GameManager_X.Singleton.GetPlayerFromClientId(playerClientId);
					S_Player_X attackingPlayer = S_GameManager_X.Singleton.GetPlayerFromClientId(Owner);

					InvokeClientRpcOnEveryone(DamagePlayer, damagedPlayer, attackingPlayer);
				}
			}
			hit = true;

			DisplayExplosion();
		}
    }

	[ClientRPC]
	private void DamagePlayer(S_Player_X damagedPlayer, S_Player_X attackingPlayer)
	{
		damagedPlayer.Damage(attackingPlayer.spellSettings.GetDamage(), attackingPlayer.OwnerClientId);
	}

	private void Move()
	{
		transform.position = transform.position + (transform.forward * owningPlayer.spellSettings.GetSpeed());
	}

	private void DisplayExplosion()
	{
		missileFire.SetActive(false);
		explosionFire.SetActive(true);
	}
}
