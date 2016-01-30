using UnityEngine;
using System.Collections;

public class Logic : MonoBehaviour {

	public static Logic ins;


	public int playersLayer;
	public int fireLayer;

	// Use this for initialization
	void Awake () {
		ins = this;
	}
}
