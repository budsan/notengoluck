using UnityEngine;
using System.Collections;

public class Extinct : MonoBehaviour {

	void OnTriggerStay(Collider col) {
        if (col.name == "Fire")
        {
            Transform g = col.transform.parent;
            Destroy(col.gameObject);
            if (g == null) return;
            if (g.gameObject.layer != Logic.playersLayer)
            {
                g.gameObject.layer = 0;
            }
            else
            {
                g.root.gameObject.tag = "Untagged";
            }

        }
    }

    public float time = .5f;
    float counter;

    void Start()
    {
        counter = time;
    }

    void Update()
    {
        counter -= Time.deltaTime;
        if (counter <= 0) Destroy(gameObject);
    }
}
