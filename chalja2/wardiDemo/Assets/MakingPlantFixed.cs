using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakingPlantFixed : MonoBehaviour {

    private GameObject myPlantObject;
    private static float threshold = -0.1f;

	// Use this for initialization
	void Start () {
        myPlantObject = gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if(myPlantObject.transform.position.y < threshold)
        {
            StartCoroutine(Example());
        }
	}

    IEnumerator Example()
    {
        print(Time.time);
        yield return new WaitForSeconds(3);
        if(myPlantObject.transform.position.y < threshold)
        {
            myPlantObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
