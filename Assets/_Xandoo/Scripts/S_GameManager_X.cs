using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;

public class S_GameManager_X : NetworkedBehaviour
{

	private static S_GameManager_X _singleton;

	public static S_GameManager_X Singleton { get { return _singleton; } }

	public S_GameMode_X gameMode;
	public GameObject MenuPanel;

	public bool gameRunning = false;
	public bool isPaused = false;

	public bool gameModeRunning = false;

	private void Awake()
	{
		if (_singleton != null && _singleton != this)
		{
			Destroy(gameObject);
			return;
		}

		_singleton = this;

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
		Debug.Log("Player Disconnected");
		if (IsHost)
		{
			gameMode.PlayerDisconnected(obj);
		}
		
	}

	private void ClientConnected(ulong obj)
	{

		GetPlayerFromClientId(obj).gameObject.name = "Player Client " + obj;

		if (IsHost)
		{
			gameMode.PlayerConnected(obj);
		}
	}

	private void SeverStarted()
	{
		gameMode.ServerStarted();
	}

	public S_Player_X GetPlayerFromClientId(ulong obj)
	{
		NetworkingManager.Singleton.ConnectedClients.TryGetValue(obj, out NetworkedClient client);
		S_Player_X p = client.PlayerObject.GetComponent<S_Player_X>();
		return p;
	}

	public void StartGameMode()
	{
		gameModeRunning = true;
		gameMode.StartGameMode();
	}

	public void ResetGame()
	{
		gameMode.ResetGameMode();
		gameRunning = false;
		isPaused = false;
		gameModeRunning = false;
	}
}
