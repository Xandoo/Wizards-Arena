using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;

public class S_PlayerCanvas_X : NetworkedBehaviour
{
	public Canvas thirdPersonCanvas;
	public Canvas firstPersonCanvas;
	public Slider thirdPersonHealthBar;
	public Slider firstPersonHealthBar;
	public Slider cooldownIndicator;

	private GameObject[] allPlayers;
	private S_Player_X localPlayer;

	private void Start()
	{
		localPlayer = GetComponent<S_Player_X>();
		if (IsLocalPlayer)
		{
			thirdPersonCanvas.gameObject.SetActive(false);
		}
		else
		{
			firstPersonCanvas.gameObject.SetActive(false);
		}

		thirdPersonHealthBar.maxValue = localPlayer.playerStats.GetMaxHealth();
		thirdPersonHealthBar.value = thirdPersonHealthBar.maxValue;
		
		firstPersonHealthBar.maxValue = localPlayer.playerStats.GetMaxHealth();
		firstPersonHealthBar.value = firstPersonHealthBar.maxValue;
		cooldownIndicator.maxValue = localPlayer.spellSettings.GetCastTime();
	}

	private void Update()
	{
		allPlayers = GameObject.FindGameObjectsWithTag("Player");

		if (IsLocalPlayer)
		{
			foreach (GameObject p in allPlayers)
			{
				p.GetComponent<S_PlayerCanvas_X>().thirdPersonCanvas.transform.LookAt(transform);
			}

			
			cooldownIndicator.value = GetComponent<S_PlayerMovement_X>().cooldownTime;
			firstPersonHealthBar.value = localPlayer.Health;
		}
	}
}
