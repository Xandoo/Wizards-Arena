using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Connection;

public class S_TeamDeathMatch_X : S_GameMode_X
{
	public SOBJ_TDMSettings_X tdmSettings;

	public List<S_Player_X> teamA { get; private set; }
	public List<S_Player_X> teamB { get; private set;  }
	public List<S_Player_X> spectating { get; private set; }

	private bool teamSelect = true;

	private void Start()
	{
		teamA = new List<S_Player_X>(tdmSettings.teamAMaxPlayers);
		teamB = new List<S_Player_X>(tdmSettings.teamBMaxPlayers);
		spectating = new List<S_Player_X>(tdmSettings.spectatingMaxPlayers);
	}

	public override void PlayerConnected(ulong clientId)
	{
		/*NetworkingManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkedClient client);
		Debug.Log(client.PlayerObject);
		if (client.PlayerObject.IsLocalPlayer)
			InvokeServerRpc(AssignTeam, client.PlayerObject.GetComponent<S_Player_X>());
		else
			AssignTeam(client.PlayerObject.GetComponent<S_Player_X>());*/
	}

	public override void PlayerDisconnected(ulong clientId)
	{
		throw new System.NotImplementedException();
	}

	public override void ServerStarted()
	{
		Debug.Log("Server Has Started");
		NetworkedClient client = NetworkingManager.Singleton.ConnectedClientsList[0];
		AssignTeam(client.PlayerObject.GetComponent<S_Player_X>());
	}

	[ServerRPC(RequireOwnership = false)]
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

	[ServerRPC(RequireOwnership = false)]
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
