using UnityEngine;
using System.Collections.Generic;

public class Boids : MonoBehaviour
{
    [SerializeField]
    private int boidsCount = 10;

    [SerializeField]
    private GameObject boidPrefab;

    [SerializeField]
    private float spawnRadius = 5f;

    [SerializeField]
    private float proximityRadius = 2f;
    
    private List<Transform> boids;

    private void Start()
    {
        boids = new List<Transform>(boidsCount);

        SpawnRemoveBoids(boidsCount);
    }
	
	private void Update ()
    {
		if(boidsCount != boids.Count)
        {
            SpawnRemoveBoids(boidsCount - boids.Count);
        }
	}

    private void SpawnRemoveBoids(int count)
    {
        if(count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Transform newBoid = Instantiate(boidPrefab, transform).transform;
                newBoid.position = transform.position + Random.insideUnitSphere * spawnRadius;

                boids.Add(newBoid);
            }
        }
        else
        {
            for(int i = 0; i < Mathf.Abs(count); i++)
            {
                Destroy(boids[boids.Count - 1].gameObject);

                boids.RemoveAt(boids.Count - 1);
            }
        }
    }
}
