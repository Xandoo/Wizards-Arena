using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;
using UnityEngine.UI;
using MLAPI.Messaging;
using MLAPI.Transports.Tasks;

public class MenuScript : NetworkedBehaviour
{
	public GameObject menuPanel;

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

	public void SetIP(string ip)
	{
		NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = ip;
	}

	private void Update()
	{
		
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void FullScreen()
	{
		Screen.SetResolution(1920, 1080, true);
	}

	public void Winodwed()
	{
		Screen.SetResolution(960, 540, false);
	}

	public void ClearPlayerPref()
	{
		PlayerPrefs.DeleteAll();
	}
}
