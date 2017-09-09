using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField]
    private Transform boids;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 centre = Vector3.zero;
		foreach(Transform t in boids.GetComponentsInChildren<Transform>())
        {
            centre += t.position;
        }
        centre /= boids.GetComponentsInChildren<Transform>().Length;


        transform.LookAt(centre);
    }
}
