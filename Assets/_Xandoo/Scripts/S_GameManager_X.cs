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

	public S_GameMode_X gameMode;


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
		/*
		NetworkingManager.Singleton.ConnectedClients.TryGetValue(obj, out NetworkedClient client);
		Debug.Log(client.PlayerObject.gameObject + " has disconnected.");

		if (IsLocalPlayer)
			InvokeServerRpc(gameMode.PlayerDisconnected, client);
		else
			gameMode.PlayerDisconnected(client);
		*/
	}

	private void ClientConnected(ulong obj)
	{
		if (IsHost)
		{
			gameMode.PlayerConnected(obj);
		}
	}

	private void SeverStarted()
	{
		gameMode.ServerStarted();
	}
}
