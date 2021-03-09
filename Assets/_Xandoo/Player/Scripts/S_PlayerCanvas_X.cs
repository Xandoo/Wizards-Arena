using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;

public class S_PlayerCanvas_X : NetworkedBehaviour
{
	public Canvas thirdPersonCanvas;
	public Canvas firstPersonCanvas;
	public GameObject pausePanel;
	public GameObject gameHud;
	public Slider thirdPersonHealthBar;
	public Slider firstPersonHealthBar;
	public Slider cooldownIndicator;
	public Button startGameButton;

	public GameObject EndScreenPanel;
	public Text EndScreenText;

	private GameObject[] allPlayers;
	private S_Player_X localPlayer;

	public Text timerText;
	public Text teamAScoreText;
	public Text teamBScoreText;

	private S_TeamDeathMatch_X gameMode;

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

		if (!IsHost)
		{
			startGameButton.gameObject.SetActive(false);
		}

		thirdPersonHealthBar.maxValue = localPlayer.playerStats.GetMaxHealth();
		thirdPersonHealthBar.value = thirdPersonHealthBar.maxValue;
		
		firstPersonHealthBar.maxValue = localPlayer.playerStats.GetMaxHealth();
		firstPersonHealthBar.value = firstPersonHealthBar.maxValue;
		cooldownIndicator.maxValue = localPlayer.spellSettings.GetCastTime();
		gameMode = (S_TeamDeathMatch_X)S_GameManager_X.Singleton.gameMode;
		gameMode.OnScoreChanged += UpdateScore;
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

			if (Input.GetButtonDown("Pause") && S_GameManager_X.Singleton.gameRunning && !S_GameManager_X.Singleton.isPaused)
			{
				S_GameManager_X.Singleton.isPaused = !S_GameManager_X.Singleton.isPaused;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				pausePanel.SetActive(true);
				gameHud.SetActive(false);
			}
			else if (Input.GetButtonDown("Pause"))
			{
				S_GameManager_X.Singleton.isPaused = !S_GameManager_X.Singleton.isPaused;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				pausePanel.SetActive(false);
				if (!GetComponent<S_PlayerMovement_X>().IsSpectating)
				{
					gameHud.SetActive(true);
				}
			}
		}

		

		if (IsHost)
		{
			if (S_GameManager_X.Singleton.gameModeRunning)
			{
				string min = "";
				string sec = "";
				switch (gameMode.gameModeState)
				{

					case S_GameMode_X.GameModeState.PREMATCH:
						min = Mathf.Floor(gameMode.preMatchTimer / 60).ToString("00");
						sec = (gameMode.preMatchTimer % 60).ToString("00");
						UpdateClientUITime(string.Format("{0}:{1}", min, sec));
						InvokeClientRpcOnEveryone(UpdateClientUITime, string.Format("{0}:{1}", min, sec));

						break;
					case S_GameMode_X.GameModeState.MATCH:
						min = Mathf.Floor(gameMode.matchTimer / 60).ToString("00");
						sec = (gameMode.matchTimer % 60).ToString("00");
						UpdateClientUITime(string.Format("{0}:{1}", min, sec));
						InvokeClientRpcOnEveryone(UpdateClientUITime, string.Format("{0}:{1}", min, sec));

						break;
					case S_GameMode_X.GameModeState.END:
						InvokeClientRpcOnEveryone(UpdateClientUITime, "00:00");
						UpdateClientsScore(0, 0);
						break;
				}
			}
		}
	}

	public void StartGameMode()
	{
		S_GameManager_X.Singleton.StartGameMode();
		gameMode = (S_TeamDeathMatch_X)S_GameManager_X.Singleton.gameMode;
		
	}

	public void Disconnect()
	{
		if (IsHost)
		{
			NetworkingManager.Singleton.StopHost();
		}
		else if (IsClient)
		{
			NetworkingManager.Singleton.StopClient();
		}

		S_GameManager_X.Singleton.ResetGame();
		timerText.text = "00:00";
		teamAScoreText.text = "00";
		teamBScoreText.text = "00";
		S_GameManager_X.Singleton.MenuPanel.SetActive(true);
		pausePanel.SetActive(false);
		S_GameManager_X.Singleton.gameRunning = false;
		S_GameManager_X.Singleton.isPaused = false;

	}

	public void UpdateScore(int a, int b)
	{
		InvokeClientRpcOnEveryone(UpdateClientsScore, a, b);
	}

	[ClientRPC]
	void UpdateClientUITime(string time)
	{
		timerText.text = time;
	}

	[ClientRPC]
	void UpdateClientsScore(int a, int b)
	{
		teamAScoreText.text = a.ToString("00");
		teamBScoreText.text = b.ToString("00");
	}

	public void SetSpectate()
	{
		Debug.LogWarning("Set Spectate.");
		if (IsHost)
		{
			InvokeClientRpcOnEveryone(SetSpectateMulticast, GetComponent<S_Player_X>());
		}
		else
		{
			InvokeServerRpc(SetSpectateServer, GetComponent<S_Player_X>());
		}
	}

	[ServerRPC]
	void SetSpectateServer(S_Player_X player)
	{
		InvokeClientRpcOnEveryone(SetSpectateMulticast, player);
	}

	[ClientRPC]
	void SetSpectateMulticast(S_Player_X player)
	{
		player.GetComponent<S_PlayerMovement_X>().SetIsSpectating(true);
	}
}
