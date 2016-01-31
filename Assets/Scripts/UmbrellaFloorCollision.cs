using UnityEngine;
using System.Collections;

public class UmbrellaFloorCollision : MonoBehaviour {

    public UnluckOnTrigger luckControl;
    bool isClosed = true;
    Animator myAnim;
    Rigidbody myRigid;

	// Use this for initialization
	void Start () {
        myAnim = GetComponent<Animator>();
        myRigid = GetComponent<Rigidbody>();
        luckControl.setActive(!isClosed);
    }

    // Update is called once per frame
    void Update () {
        
	}

    void OnCollisionEnter(Collision c)
    {
        if(c.contacts[0].thisCollider.GetComponent<ColliderItem>() != null && c.contacts[0].otherCollider.name == "Floor")
        {
            isClosed = !isClosed;
            myAnim.SetBool("isClosed", isClosed);
            luckControl.setActive(!isClosed);
            myRigid.velocity += Vector3.up*.5f;
        }        
    }
}
