using UnityEngine;
using System.Collections;

public class StairFloorCollision : MonoBehaviour {

    public UnluckOnTrigger luckControl;
    public Collider[] changeColliders;
    bool crazyMode = false;
    public bool isClosed = true;
    Animator myAnim;
    float counter = 0;

	// Use this for initialization
	void Start () {
        myAnim = GetComponent<Animator>();
        myAnim.SetBool("isClosed", isClosed);
        luckControl.setActive(!isClosed);
    }

    // Update is called once per frame
    void Update () {
        if (crazyMode)
        {
            counter -= Time.deltaTime;

            if (counter <= 0)
            {
                counter += .1f;
                isClosed = !isClosed;
                myAnim.SetBool("isClosed", isClosed);
                luckControl.setActive(!isClosed);
            }
            if(Vector3.Angle(transform.up, Vector3.up) < 10)
            {
                crazyMode = false;
                for (int i = 0; i < changeColliders.Length; ++i) changeColliders[i].isTrigger = !changeColliders[i].isTrigger;

            }
        }
	}

    void OnCollisionEnter(Collision c)
    {
        if(c.contacts[0].thisCollider.name == "Base" && c.contacts[0].otherCollider.name == "Floor" && !crazyMode)
        {
            for (int i = 0; i < changeColliders.Length; ++i) changeColliders[i].isTrigger = !changeColliders[i].isTrigger;
            crazyMode = true;
            isClosed = !isClosed;
            myAnim.SetBool("isClosed", isClosed);
            luckControl.setActive(!isClosed);
            counter = .1f;
        }        
    }
}
