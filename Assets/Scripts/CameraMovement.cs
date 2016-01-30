using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public Transform player;

	const float DEEP_FACTOR = 0.6f;

	// Update is called once per frame
	void LateUpdate () {
		Vector3 pos = transform.position;
		pos.x = player.position.x;
		pos.z = player.position.z * DEEP_FACTOR;
		transform.position = pos;

		transform.LookAt (player);
	}
}
