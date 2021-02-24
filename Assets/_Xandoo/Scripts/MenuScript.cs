using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;

public class MenuScript : NetworkedBehaviour
{
	public GameObject menuPanel;
	public GameObject pausePanel;
	//public S_GameManager_X gameManager;

	public void Host()
	{
		NetworkingManager.Singleton.StartHost();
		menuPanel.SetActive(false);
		S_GameManager_X.Singleton.gameRunning = true;
	}

	public void Join()
	{
		NetworkingManager.Singleton.StartClient();
		menuPanel.SetActive(false);
		S_GameManager_X.Singleton.gameRunning = true;
	}

	public void Disconnect()
	{
		NetworkingManager.Singleton.DisconnectClient(OwnerClientId);
		menuPanel.SetActive(true);
		pausePanel.SetActive(false);
		S_GameManager_X.Singleton.gameRunning = false;
	}

	public void SetIP(string ip)
	{
		NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = ip;
	}

	private void Update()
	{
		if (Input.GetButtonDown("Pause") && S_GameManager_X.Singleton.gameRunning && !S_GameManager_X.Singleton.isPaused)
		{
			S_GameManager_X.Singleton.isPaused = !S_GameManager_X.Singleton.isPaused;
			Cursor.lockState = CursorLockMode.None;
			pausePanel.SetActive(true);
		}
		else if (Input.GetButtonDown("Pause"))
		{
			S_GameManager_X.Singleton.isPaused = !S_GameManager_X.Singleton.isPaused;
			Cursor.lockState = CursorLockMode.Locked;
			pausePanel.SetActive(false);
		}
	}
}
