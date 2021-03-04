using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class AnimatorEventCatch : MonoBehaviour
{
    public Animator anim;
	public S_Player_X player;

    void SetAnimSpeedToCastTime()
	{
		anim.speed += player.spellSettings.GetCastTime();
	}

	void ResetAnimSpeed()
	{
		anim.speed = 1f;
	}
}
