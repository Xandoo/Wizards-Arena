using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TDM_Settings", menuName = "Scriptable Objects/Game Modes/Team Death Match")]
public class SOBJ_TDMSettings_X : ScriptableObject
{
	public int teamAMaxPlayers = 4;
	public int teamBMaxPlayers = 4;
	public int spectatingMaxPlayers = 2;

	public float preMatchTimer = 60;
	public float matchTime = 300;

	public int scoreToWin = 40;
}
