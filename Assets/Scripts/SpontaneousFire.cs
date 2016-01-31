using UnityEngine;
using System.Collections;

public class SpontaneousFire : MonoBehaviour {

	public GameObject fire;

	public void SpawnFire() {
		Instantiate (fire, transform.position, Quaternion.identity);
		this.enabled = false;
	}
}
