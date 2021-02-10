using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class AnimatorEventCatch : NetworkedBehaviour
{
    public S_Player_X player;

    void CastSpell()
    {
        if (IsLocalPlayer)
            InvokeServerRpc(player.CastSpell);
        else
            player.CastSpell();
    }
}
