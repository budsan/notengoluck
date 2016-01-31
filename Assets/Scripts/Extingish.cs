using UnityEngine;
using System.Collections;

public class Extingish : MonoBehaviour {


    bool isGrabbed = false;

    public void setGrab(bool grab)
    {
        isGrabbed = grab;
    }

    public GameObject extinguisher;
    float counter = .1f;

	// Update is called once per frame
	void Update () {
	    if(isGrabbed)
        {
            counter -= Time.deltaTime;
            if(counter <= 0)
            {
                counter += .1f;
                GameObject insExt = (GameObject)Instantiate(extinguisher, transform.position, extinguisher.transform.rotation);

            }
        }
	}
}
