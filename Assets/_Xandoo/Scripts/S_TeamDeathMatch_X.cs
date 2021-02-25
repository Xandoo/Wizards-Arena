using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Connection;

public class S_TeamDeathMatch_X : S_GameMode_X
{
	public Transform[] doors;
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
		if (isRunning)
		{
			switch (gameModeState)
			{
				case GameModeState.PREMATCH:
					preMatchTimer -= Time.deltaTime;
					if (preMatchTimer <= 0f)
					{
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

	//[ServerRPC(RequireOwnership = false)]
	public override void PlayerConnected(ulong clientObj)
	{
		NetworkingManager.Singleton.ConnectedClients.TryGetValue(clientObj, out NetworkedClient client);
		Debug.Log(client.PlayerObject.gameObject + " has connected.");

		AssignTeam(clientObj);
		UpdateConnectedPlayers();
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

	//[ServerRPC(RequireOwnership = false)]
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

	void AssignTeam(ulong obj)
	{
		S_Player_X p = S_GameManager_X.Singleton.GetPlayerFromClientId(obj);
		if (teamSelect)
		{
			teamA.Add(obj);
			p.teamID.Value = 0;
			if (!p.GetComponent<NetworkedObject>().IsLocalPlayer)
			{
				InvokeClientRpcOnClient(SetPlayerPosition, obj, p, teamASpawn[Random.Range(0, teamASpawn.Count - 1)].transform.position);
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
				p.transform.position = teamASpawn[Random.Range(0, teamASpawn.Count - 1)].transform.position;
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

			if (!p.GetComponent<NetworkedObject>().IsLocalPlayer)
			{
				InvokeClientRpcOnClient(SetPlayerPosition, obj, p, teamBSpawn[Random.Range(0, teamBSpawn.Count - 1)].transform.position);

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
				p.transform.position = teamBSpawn[Random.Range(0, teamBSpawn.Count - 1)].transform.position;
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
	void SetPlayerPosition(S_Player_X p, Vector3 pos)
	{
		p.SetPosition(pos);
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
			int randomInt = Random.Range(0, players.Count - 1);
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
}
