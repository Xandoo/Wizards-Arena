using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;

public abstract class S_GameMode_X : NetworkedBehaviour
{
	public enum GameModeState
	{
		NONE,
		PREMATCH,
		MATCH,
		END
	}

	public abstract void PlayerConnected(ulong clientObj);

	public abstract void PlayerDisconnected(ulong clientObj);

	public abstract void ServerStarted();

	public abstract void StartGameMode();

	public abstract void ResetGameMode();

	public abstract void RespawnPlayer(S_Player_X player);
}
