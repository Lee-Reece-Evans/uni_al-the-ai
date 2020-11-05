using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObjectRotateC : MonoBehaviour {

    public float rotationSpeed = 100f;
    public Vector3 direction = new Vector3(0, 0, 1);

	// Update is called once per frame
	void Update () {
        transform.Rotate(direction * rotationSpeed * Time.deltaTime);
	}
}
