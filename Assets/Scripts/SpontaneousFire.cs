using UnityEngine;
using System.Collections;

public class SpontaneousFire : MonoBehaviour {

	public GameObject fire;

	public void SpawnFire() {
		GameObject insFire = (GameObject) Instantiate (fire, transform.position, fire.transform.rotation);
        insFire.name = "Fire";
        insFire.transform.localScale = Vector3.one;
		this.enabled = false;
	}
}
