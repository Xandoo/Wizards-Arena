using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TDM_Settings", menuName = "Scriptable Objects/Game Modes/Team Death Match")]
public class SOBJ_TDMSettings_X : ScriptableObject
{
	public int teamAMaxPlayers = 4;
	public int teamBMaxPlayers = 4;
	public int spectatingMaxPlayers = 2;

	public Material allyMaterial;
	public Material enemyMaterial;

	public float preMatchTimer = 60f;
	public float matchTime = 300f;
	public float respawnTime = 3f;

	public int scoreToWin = 40;
}
