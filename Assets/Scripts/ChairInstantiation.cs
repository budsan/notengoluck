using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChairInstantiation : MonoBehaviour {

    public Transform chair;

	// Use this for initialization
	void Start () {
        List<Transform> childs = new List<Transform>();
        foreach(Transform child in transform)
        {
            if(child.name != "FireAble") childs.Add(child);
        }

        int instantiationNum = Random.Range(0, childs.Count + 1);
        for(int i = 0; i < instantiationNum; ++i)
        {
            int index = Random.Range(0, childs.Count);
            Transform newPos = childs[index];
            childs.RemoveAt(index);

            Instantiate(chair, newPos.position, newPos.rotation);
            
        }
	}

}
