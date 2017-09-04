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

    [SerializeField]
    private float velocity = 1f;

    [SerializeField]
    private float rotationSpeed = 1f;

    [SerializeField]
    private Transform leaderTransform;

    [Header("Coesfficients")]
    [SerializeField]
    private float cohesionCoeff = 1f;
    [SerializeField]
    private float alignmentCoeff = 1f;
    [SerializeField]
    private float separationCoeff = 1f;
    [SerializeField]
    private float randomCoeff = 1f;
    [SerializeField]
    private float leaderCoeff = 1f;

    private List<Transform> boids;
    private List<float> noises;

    private void Start()
    {
        boids = new List<Transform>(boidsCount);
        noises = new List<float>(boidsCount);

        SpawnRemoveBoids(boidsCount);
    }
	
	private void Update ()
    {
		if(boidsCount != boids.Count)
        {
            SpawnRemoveBoids(boidsCount - boids.Count);
        }

        UpdateBoids();
	}

    private void SpawnRemoveBoids(int count)
    {
        if(count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Transform newBoid = Instantiate(boidPrefab, transform).transform;
                newBoid.position = transform.position + Random.insideUnitSphere * spawnRadius;
                newBoid.rotation = Random.rotation;
                boids.Add(newBoid);
                noises.Add(Random.value * 10f);
            }
        }
        else
        {
            for(int i = 0; i < Mathf.Abs(count); i++)
            {
                Destroy(boids[boids.Count - 1].gameObject);

                boids.RemoveAt(boids.Count - 1);
                noises.RemoveAt(boidsCount - 1);
            }
        }
    }

    private void UpdateBoids()
    {        
        for(int i = 0; i < boids.Count; i++)
        {
            Vector3 cohesion = Vector3.zero;
            Vector3 alignment = Vector3.zero;
            Vector3 separation = Vector3.zero;
            Vector3 leader = leaderTransform.position - boids[i].position;
            Vector3 random = boids[i].right * (Mathf.PerlinNoise(noises[i], Time.time) * 2f - 1f) +
                boids[i].up * (Mathf.PerlinNoise(Time.time, noises[i]) * 2f - 1f);
            random.Normalize();
            
            int nearbyBoids = 0;
            for (int j = 0; j < boidsCount; j++)
            {
                if(Vector3.Distance(boids[j].position, boids[i].position) < proximityRadius)
                {
                    Vector3 diff = boids[i].position - boids[j].position;

                    cohesion += boids[i].position;
                    alignment += boids[i].forward;
                    separation += diff.normalized * (1 - diff.magnitude / proximityRadius);
                    nearbyBoids++;
                }
            }
            float denom = 1f / nearbyBoids;

            cohesion *= denom;
            alignment *= denom;

            Vector3 newForward = cohesion * cohesionCoeff +
                alignment * alignmentCoeff +
                separation * separationCoeff +
                leader * leaderCoeff +
                random * randomCoeff;

            newForward.Normalize();
            
            boids[i].rotation = Quaternion.Slerp(boids[i].rotation, Quaternion.LookRotation(newForward), rotationSpeed * Time.deltaTime);

            boids[i].position += boids[i].forward * velocity * Time.deltaTime;
        }
    }
}
