using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;

public class S_GameManager_X : NetworkedBehaviour
{

	private static S_GameManager_X _singleton;

	public static S_GameManager_X Singleton { get; }

	public List<S_Player_X> teamA;
	public List<S_Player_X> teamB;

	private bool teamSelect = true;

	private void Awake()
	{
		if (_singleton != null && _singleton != this)
		{
			Destroy(gameObject);
		}
		else
		{
			_singleton = this;
		}
		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		NetworkingManager.Singleton.OnServerStarted += SeverStarted;
		NetworkingManager.Singleton.OnClientConnectedCallback += ClientConnected;
		NetworkingManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
	}

	private void ClientDisconnected(ulong obj)
	{
		throw new System.NotImplementedException();
	}

	private void ClientConnected(ulong obj)
	{
		NetworkingManager.Singleton.ConnectedClients.TryGetValue(obj, out NetworkedClient client);
		Debug.Log(client.PlayerObject);
		if (client.PlayerObject.IsLocalPlayer)
			InvokeServerRpc(AssignTeam, client.PlayerObject.GetComponent<S_Player_X>());
		else
			AssignTeam(client.PlayerObject.GetComponent<S_Player_X>());
	}

	private void SeverStarted()
	{
		Debug.Log("Server Has Started");
		NetworkedClient client = NetworkingManager.Singleton.ConnectedClientsList[0];
		AssignTeam(client.PlayerObject.GetComponent<S_Player_X>());
	}


	[ServerRPC]
	void AssignTeam(S_Player_X p)
	{
		if (teamSelect)
		{
			teamA.Add(p);
			p.teamID.Value = 0;
			teamSelect = !teamSelect;
		}
		else
		{
			teamB.Add(p);
			p.teamID.Value = 1;
			teamSelect = !teamSelect;
		}
	}

	[ServerRPC]
	public void SelectTeam(S_Player_X p, int teamId)
	{
		switch (teamId)
		{
			case 0:
				teamA.Add(p);
				p.teamID.Value = 0;
				break;
			case 1:
				teamB.Add(p);
				p.teamID.Value = 1;
				break;
			default:
				break;
		}
	}

	[ServerRPC]
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
				teamA.Add(players[randomInt]);
				players[randomInt].teamID.Value = 0;
				players.RemoveAt(randomInt);
				teamSelect = !teamSelect;
			}
			else
			{
				teamB.Add(players[randomInt]);
				players[randomInt].teamID.Value = 1;
				players.RemoveAt(randomInt);
				teamSelect = !teamSelect;
			}
		}
	}
}
