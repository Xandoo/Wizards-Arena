using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using MLAPI.Messaging;

public class S_Projectile_X : NetworkedBehaviour
{
	public GameObject muzzleFire;
	public GameObject explosionFire;
	public GameObject missileFire;
	public LayerMask layerMask;


	public ulong Owner = 0;

	private S_Player_X owningPlayer;
	//private float timeElapsed = 0f;
	private bool hit = false;

    // Start is called before the first frame update
    void Start()
    {
		muzzleFire.SetActive(true);
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

		if (Physics.SphereCast(transform.position, 0.15f, transform.forward, out RaycastHit hitInfo, 0.15f, layerMask)  && !hit)
		{
			Debug.Log("Sphere Hit: " + hitInfo.transform.gameObject);
			hit = true;

			DisplayExplosion();

			if (IsHost)
			{
				if (hitInfo.transform.gameObject.tag.Equals("Player"))
				{
					ulong playerClientId = hitInfo.transform.gameObject.GetComponent<NetworkedObject>().OwnerClientId;
					InvokeClientRpcOnEveryone(DamagePlayer, playerClientId, Owner);
				}
			}
		}
    }

	[ClientRPC]
	private void DamagePlayer(ulong damagedPlayerClientId, ulong attackingPlayerClientId)
	{
		S_Player_X damagedPlayer = S_GameManager_X.Singleton.GetPlayerFromClientId(damagedPlayerClientId);
		S_Player_X attackingPlayer = S_GameManager_X.Singleton.GetPlayerFromClientId(attackingPlayerClientId);

		damagedPlayer.Damage(attackingPlayer.spellSettings.GetDamage(), attackingPlayerClientId);

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
