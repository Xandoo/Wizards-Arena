using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;

public class S_PlayerCanvas_X : NetworkedBehaviour
{
	public Canvas canvas;
	public Slider healthBar;

	private GameObject[] allPlayers;

	private void Start()
	{
		if (IsLocalPlayer)
		{
			canvas.gameObject.SetActive(false);
		}

		healthBar.maxValue = GetComponent<S_Player_X>().playerStats.GetMaxHealth();
		healthBar.value = healthBar.maxValue;
	}

	private void Update()
	{
		allPlayers = GameObject.FindGameObjectsWithTag("Player");

		if (IsLocalPlayer)
		{
			foreach (GameObject p in allPlayers)
			{
				p.GetComponent<S_PlayerCanvas_X>().canvas.transform.LookAt(transform);
			}
		}
	}
}
