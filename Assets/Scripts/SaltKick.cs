using UnityEngine;
using System.Collections;

public class SaltKick : MonoBehaviour {

    public UnluckOnTrigger myPot;

	// Use this for initialization
	void Start () {
        myPot.setActive(false);

    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnCollisionEnter(Collision c)
    {
        if(c.contacts[0].otherCollider.name == "Floor")
        {
            myPot.setActive(true);
            Destroy(gameObject);
        }
    }
}
