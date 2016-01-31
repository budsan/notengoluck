using UnityEngine;
using System.Collections;

public class SpontaneousFire : MonoBehaviour {

	public GameObject fire;

	public void SpawnFire() {
		Instantiate (fire, transform.position, fire.transform.rotation);
		this.enabled = false;
	}
}
