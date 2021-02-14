using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_SpawnPoint_X : MonoBehaviour
{
	public int teamID = -1;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawCube(transform.position, new Vector3(1f, 2f, 1f));
	}
}
