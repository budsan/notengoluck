using UnityEngine;
using System.Collections;

public class SpontaneousFire : MonoBehaviour {

	public GameObject fire;

	public void SpawnFire() {
        if (transform.parent.gameObject.layer == Logic.fireLayer) return;
		GameObject insFire = (GameObject) Instantiate (fire, transform.position, fire.transform.rotation);
        insFire.name = "Fire";
        insFire.transform.localScale = Vector3.one;
        insFire.transform.SetParent(transform.parent, true);
        this.enabled = false;
	}
}
