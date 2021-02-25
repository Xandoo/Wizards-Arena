using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;
using UnityEngine.UI;

public class MenuScript : NetworkedBehaviour
{
	public GameObject menuPanel;
	public GameObject pausePanel;
	public GameObject gameHud;

	public Button startGameButton;
	public Text timerText;
	public Text teamAScoreText;
	public Text teamBScoreText;
	private S_TeamDeathMatch_X gameMode;

	public void Host()
	{
		NetworkingManager.Singleton.StartHost();
		menuPanel.SetActive(false);
		gameHud.SetActive(true);
		S_GameManager_X.Singleton.gameRunning = true;
	}

	public void Join()
	{
		NetworkingManager.Singleton.StartClient();
		menuPanel.SetActive(false);
		gameHud.SetActive(true);
		S_GameManager_X.Singleton.gameRunning = true;
		startGameButton.gameObject.SetActive(false);
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
		
		menuPanel.SetActive(true);
		pausePanel.SetActive(false);
		S_GameManager_X.Singleton.gameRunning = false;
		S_GameManager_X.Singleton.isPaused = false;
	}

	public void SetIP(string ip)
	{
		NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = ip;
	}

	public void StartGameMode()
	{
		S_GameManager_X.Singleton.StartGameMode();
		gameMode = (S_TeamDeathMatch_X)S_GameManager_X.Singleton.gameMode;
	}

	private void Update()
	{
		if (Input.GetButtonDown("Pause") && S_GameManager_X.Singleton.gameRunning && !S_GameManager_X.Singleton.isPaused)
		{
			S_GameManager_X.Singleton.isPaused = !S_GameManager_X.Singleton.isPaused;
			Cursor.lockState = CursorLockMode.None;
			pausePanel.SetActive(true);
			gameHud.SetActive(false);
		}
		else if (Input.GetButtonDown("Pause"))
		{
			S_GameManager_X.Singleton.isPaused = !S_GameManager_X.Singleton.isPaused;
			Cursor.lockState = CursorLockMode.Locked;
			pausePanel.SetActive(false);
			gameHud.SetActive(true);
		}

		if (S_GameManager_X.Singleton.gameModeRunning)
		{
			string min = "";
			string sec = "";
			switch (gameMode.gameModeState)
			{
				
				case S_GameMode_X.GameModeState.PREMATCH:
					min = Mathf.Floor(gameMode.preMatchTimer / 60).ToString("00");
					sec = (gameMode.preMatchTimer % 60).ToString("00");
					timerText.text = string.Format("{0}:{1}", min, sec);
					break;
				case S_GameMode_X.GameModeState.MATCH:
					min = Mathf.Floor(gameMode.preMatchTimer / 60).ToString("00");
					sec = (gameMode.preMatchTimer % 60).ToString("00");
					timerText.text = string.Format("{0}:{1}", min, sec);
					break;
				case S_GameMode_X.GameModeState.END:
					break;
			}
		}
	}
}
