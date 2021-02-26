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
		PREMATCH,
		MATCH,
		END
	}

	//[ServerRPC(RequireOwnership = false)]
	public abstract void PlayerConnected(ulong clientObj);
	//[ServerRPC(RequireOwnership = false)]
	public abstract void PlayerDisconnected(ulong clientObj);
	public abstract void ServerStarted();

	public abstract void StartGameMode();

	public abstract void ResetGameMode();
}
