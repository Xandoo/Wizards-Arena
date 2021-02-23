using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;

public class MenuScript : MonoBehaviour
{
	public GameObject menuPanel;


	public void Host()
	{
		NetworkingManager.Singleton.StartHost();
		menuPanel.SetActive(false);
	}

	public void Join()
	{
		NetworkingManager.Singleton.StartClient();
		menuPanel.SetActive(false);
	}

	public void SetJoinIP(string ip)
	{
		NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = ip;
	}
}
