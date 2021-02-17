using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public abstract class S_GameMode_X : NetworkedBehaviour
{
	public abstract void PlayerConnected(ulong clientId);
	public abstract void PlayerDisconnected(ulong clientId);
	public abstract void ServerStarted();
}
