using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRotate : MonoBehaviour {
    public float fTheta;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        fTheta += Time.deltaTime;
        transform.localEulerAngles = new Vector3(0, 180 + Mathf.Rad2Deg * fTheta,0);
	}
}
