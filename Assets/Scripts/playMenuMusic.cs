using UnityEngine;
using System.Collections;

public class playMenuMusic : MonoBehaviour {

	// Use this for initialization
	void Start () {
        AudioSource a = GetComponent<AudioSource>();
        a.time = 71f;
        a.Play();
	}
	

}
