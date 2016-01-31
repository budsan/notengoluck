using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
	const float DEEP_FACTOR = 0.6f;

	// Update is called once per frame
	void LateUpdate ()
	{
		int count = 0;
		Vector3 med = new Vector3();
		for (int i = 0; i < Logic.ins.Players.Length; i++)
		{
			GameObject player = Logic.ins.Players[i];
			if (player == null)
				continue;

			med += player.transform.position;
			count++;
		}

		med /= count;
		Vector3 pos = transform.position;
		//if (count == 0)
		//	med = new Vector3(0, 0, 0);
		pos.x = med.x;
		pos.z = -7f + med.z * DEEP_FACTOR;
		transform.position = pos;
		transform.LookAt(med);
	}
}
