using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class AnimatorEventCatch : NetworkedBehaviour
{
    public S_Player_X player;

    void CastSpell()
    {
        if (!IsHost)
        {
            InvokeServerRpc(RequestCastSpellOnServer);
        }
        else
        {
            player.CastSpell();
        }
    }

    [ServerRPC]
    void RequestCastSpellOnServer()
    {
        player.CastSpell();
    }
}
