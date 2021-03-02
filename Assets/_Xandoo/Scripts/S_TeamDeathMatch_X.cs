using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Connection;
using MLAPI.Prototyping;
using System;

public class S_TeamDeathMatch_X : S_GameMode_X
{
	public GameObject doors;
	public S_SpawnPoint_X[] spawnPoints;
	public SOBJ_TDMSettings_X tdmSettings;

	public List<ulong> teamA { get; private set; }
	public List<ulong> teamB { get; private set; }
	public List<ulong> spectating { get; private set; }

	private List<S_SpawnPoint_X> teamASpawn;
	private List<S_SpawnPoint_X> teamBSpawn;

	private bool teamSelect = true;
	private bool isRunning = false;

	private float timeElapsed = 0f;
	public float preMatchTimer = 0f;
	public float matchTimer = 0f;

	public int teamAEleminations = 0;
	public int teamBEleminations = 0;
	public bool winningTeam;

	public GameModeState gameModeState;

	public event Action<int, int> OnScoreChanged;

	private void Start()
	{
		teamA = new List<ulong>(tdmSettings.teamAMaxPlayers);
		teamB = new List<ulong>(tdmSettings.teamBMaxPlayers);
		spectating = new List<ulong>(tdmSettings.spectatingMaxPlayers);
		teamASpawn = new List<S_SpawnPoint_X>();
		teamBSpawn = new List<S_SpawnPoint_X>();

		preMatchTimer = tdmSettings.preMatchTimer;
		matchTimer = tdmSettings.matchTime;

		foreach (S_SpawnPoint_X point in spawnPoints)
		{
			switch (point.teamID)
			{
				case 0:
					teamASpawn.Add(point);
					break;
				case 1:
					teamBSpawn.Add(point);
					break;
			}
		}
	}

	private void Update()
	{
		if (IsHost)
		{
			if (isRunning)
			{
				switch (gameModeState)
				{
					case GameModeState.PREMATCH:
						preMatchTimer -= Time.deltaTime;
						if (preMatchTimer <= 0f)
						{
							//OpenDoors();
							InvokeClientRpcOnEveryone(SetDoorsActive, false);
							gameModeState = GameModeState.MATCH;
						}
						break;
					case GameModeState.MATCH:
						matchTimer -= Time.deltaTime;

						if (teamAEleminations >= tdmSettings.scoreToWin)
						{
							winningTeam = true;
							gameModeState = GameModeState.END;
						}

						if (teamBEleminations >= tdmSettings.scoreToWin)
						{
							winningTeam = false;
							gameModeState = GameModeState.END;
						}

						if (matchTimer <= 0f)
						{
							gameModeState = GameModeState.END;
						}

						break;
					case GameModeState.END:
						break;
				}

				timeElapsed += Time.deltaTime;
			}
		}
	}

	public override void PlayerConnected(ulong clientObj)
	{
		NetworkingManager.Singleton.ConnectedClients.TryGetValue(clientObj, out NetworkedClient client);
		Debug.Log(client.PlayerObject.gameObject + " has connected.");

		AssignTeam(clientObj);
		UpdateConnectedPlayers();

		if (gameModeState == GameModeState.MATCH)
		{
			InvokeClientRpcOnClient(SetDoorsActive, clientObj, false);
		}
	}

	private void UpdateConnectedPlayers()
	{
		NetworkedClient[] clients = NetworkingManager.Singleton.ConnectedClientsList.ToArray();
		foreach (NetworkedClient client in clients)
		{
			if (!client.PlayerObject.IsOwnedByServer)
			{
				ulong obj = client.ClientId;
				if (client.PlayerObject.GetComponent<S_Player_X>().teamID.Value == 0)
				{
					foreach (ulong clientId in teamA)
					{
						InvokeClientRpcOnClient(SetTeamMaterials, obj, S_GameManager_X.Singleton.GetPlayerFromClientId(clientId), true);
					}
					foreach (ulong clientId in teamB)
					{
						InvokeClientRpcOnClient(SetTeamMaterials, obj, S_GameManager_X.Singleton.GetPlayerFromClientId(clientId), false);
					}
				}
				else if (client.PlayerObject.GetComponent<S_Player_X>().teamID.Value == 1)
				{
					foreach (ulong clientId in teamB)
					{
						InvokeClientRpcOnClient(SetTeamMaterials, obj, S_GameManager_X.Singleton.GetPlayerFromClientId(clientId), true);
					}
					foreach (ulong clientId in teamA)
					{
						InvokeClientRpcOnClient(SetTeamMaterials, obj, S_GameManager_X.Singleton.GetPlayerFromClientId(clientId), false);
					}
				}
			}
			else
			{
				if (client.PlayerObject.GetComponent<S_Player_X>().teamID.Value == 0)
				{
					foreach (ulong clientId in teamA)
					{
						SetTeamMaterials(S_GameManager_X.Singleton.GetPlayerFromClientId(clientId), true);
					}
					foreach (ulong clientId in teamB)
					{
						SetTeamMaterials(S_GameManager_X.Singleton.GetPlayerFromClientId(clientId), false);
					}
				}
				else if (client.PlayerObject.GetComponent<S_Player_X>().teamID.Value == 1)
				{
					foreach (ulong clientId in teamB)
					{
						SetTeamMaterials(S_GameManager_X.Singleton.GetPlayerFromClientId(clientId), true);
					}
					foreach (ulong clientId in teamA)
					{
						SetTeamMaterials(S_GameManager_X.Singleton.GetPlayerFromClientId(clientId), false);
					}
				}
			}
		}
		
	}

	public override void PlayerDisconnected(ulong clientId)
	{

		if (teamA.Contains(clientId))
		{
			teamA.Remove(clientId);
		}
		else if (teamB.Contains(clientId))
		{
			teamB.Remove(clientId);
		}
	}

	public override void ServerStarted()
	{
		Debug.Log("Server Has Started");
		NetworkedClient client = NetworkingManager.Singleton.ConnectedClientsList[0];
		AssignTeam(client.PlayerObject.OwnerClientId);
	}

	public override void StartGameMode()
	{
		isRunning = true;
		gameModeState = GameModeState.PREMATCH;
	}

	public override void ResetGameMode()
	{
		Start();
		SetDoorsActive(true);
		teamSelect = false;
		isRunning = false;
	}

	void AssignTeam(ulong obj)
	{
		S_Player_X p = S_GameManager_X.Singleton.GetPlayerFromClientId(obj);
		if (teamSelect)
		{
			int randomNumber = UnityEngine.Random.Range(0, teamASpawn.Count - 1);
			teamA.Add(obj);
			p.teamID.Value = 0;
			if (!p.GetComponent<NetworkedObject>().IsLocalPlayer)
			{
				
				InvokeClientRpcOnClient(SetPlayerPosition, obj, p, teamASpawn[randomNumber].transform.position, teamASpawn[randomNumber].transform.rotation);
				/*
				foreach (S_Player_X player in teamA)
				{
					InvokeClientRpcOnClient(SetTeamMaterials, obj, player, true);
				}
				foreach (S_Player_X player in teamB)
				{
					InvokeClientRpcOnClient(SetTeamMaterials, obj, player, false);
				}
				*/
			}
			else
			{
				SetPlayerPosition(p, teamASpawn[randomNumber].transform.position, teamASpawn[randomNumber].transform.rotation);
				//p.transform.position = teamASpawn[Random.Range(0, teamASpawn.Count - 1)].transform.position;
				/*
				foreach (S_Player_X player in teamA)
				{
					SetTeamMaterials(player, true);
				}
				foreach (S_Player_X player in teamB)
				{
					SetTeamMaterials(player, false);
				}
				*/
			}
			
			teamSelect = !teamSelect;
		}
		else
		{
			teamB.Add(obj);
			p.teamID.Value = 1;
			int randomNumber = UnityEngine.Random.Range(0, teamBSpawn.Count - 1);
			if (!p.GetComponent<NetworkedObject>().IsLocalPlayer)
			{
				InvokeClientRpcOnClient(SetPlayerPosition, obj, p, teamBSpawn[randomNumber].transform.position, teamBSpawn[randomNumber].transform.rotation);

				/*
				foreach (S_Player_X player in teamB)
				{
					InvokeClientRpcOnClient(SetTeamMaterials, obj, player, true);
				}
				foreach (S_Player_X player in teamA)
				{
					InvokeClientRpcOnClient(SetTeamMaterials, obj, player, false);
				}
				*/
			}
			else
			{
				SetPlayerPosition(p, teamBSpawn[randomNumber].transform.position, teamBSpawn[randomNumber].transform.rotation);
				//p.transform.position = teamBSpawn[Random.Range(0, teamBSpawn.Count - 1)].transform.position;
				/*
				foreach (S_Player_X player in teamB)
				{
					SetTeamMaterials(player, true);
				}
				foreach (S_Player_X player in teamA)
				{
					SetTeamMaterials(player, false);
				}
				*/
			}

			teamSelect = !teamSelect;
		}
	}

	[ClientRPC]
	void SetPlayerPosition(S_Player_X p, Vector3 pos, Quaternion rot)
	{
		p.Teleport(pos, rot);
	}
	
	[ClientRPC]
	void SetTeamMaterials(S_Player_X p, bool ally)
	{
		if (ally)
		{ 
			p.ChangeMaterial(tdmSettings.allyMaterial);
		}
		else
		{
			p.ChangeMaterial(tdmSettings.enemyMaterial);
		}
	}

	[ClientRPC]
	void SetDoorsActive(bool active)
	{
		doors.SetActive(active);
	}
	
	[ServerRPC(RequireOwnership = false)]
	public void SelectTeam(ulong obj, int teamId)
	{
		S_Player_X p = S_GameManager_X.Singleton.GetPlayerFromClientId(obj);

		switch (teamId)
		{
			case 0:
				teamA.Add(obj);
				p.teamID.Value = 0;
				break;
			case 1:
				teamB.Add(obj);
				p.teamID.Value = 1;
				break;
			default:
				break;
		}
	}

	[ServerRPC(RequireOwnership = false)]
	public void BalanceTeams()
	{
		teamA.Clear();
		teamB.Clear();
		teamSelect = true;

		List<NetworkedClient> clients = NetworkingManager.Singleton.ConnectedClientsList;
		List<S_Player_X> players = new List<S_Player_X>();

		foreach (NetworkedClient cli in clients)
		{
			players.Add(cli.PlayerObject.GetComponent<S_Player_X>());
		}

		for (int i = 0; i < players.Count; i++)
		{
			int randomInt = UnityEngine.Random.Range(0, players.Count - 1);
			if (teamSelect)
			{
				teamA.Add(clients[randomInt].ClientId);
				players[randomInt].teamID.Value = 0;
				players.RemoveAt(randomInt);
				teamSelect = !teamSelect;
			}
			else
			{
				teamB.Add(clients[randomInt].ClientId);
				players[randomInt].teamID.Value = 1;
				players.RemoveAt(randomInt);
				teamSelect = !teamSelect;
			}
		}
	}

	public override void RespawnPlayer(S_Player_X player)
	{
		if (IsOwner)
		{
			InvokeServerRpc(IncreaseScore, player);
		}
		StartCoroutine(RespawnPlayerCoroutine(player));
	}

	[ServerRPC]
	void IncreaseScore(S_Player_X player)
	{
		if (player.teamID.Value == 0)
		{
			teamBEleminations += 1;
		}
		else if (player.teamID.Value == 1)
		{
			teamAEleminations += 1;
		}
		OnScoreChanged(teamAEleminations, teamBEleminations);
	}

	IEnumerator RespawnPlayerCoroutine(S_Player_X player)
	{
		yield return new WaitForSeconds(tdmSettings.respawnTime);
		
		if (player.teamID.Value == 0)
		{
			int randomNumber = UnityEngine.Random.Range(0, teamASpawn.Count - 1);

			SetPlayerPosition(player, teamASpawn[randomNumber].transform.position, teamASpawn[randomNumber].transform.rotation);
		}
		else if (player.teamID.Value == 1)
		{
			int randomNumber = UnityEngine.Random.Range(0, teamBSpawn.Count - 1);

			SetPlayerPosition(player, teamBSpawn[randomNumber].transform.position, teamBSpawn[randomNumber].transform.rotation);
		}
		player.GetComponent<S_PlayerMovement_X>().SetIsSpectating(false);
	}
}
