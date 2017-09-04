using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Boids artificial life algorithm http://en.wikipedia.org/wiki/Boids
separation: steer to avoid crowding local flockmates
alignment: steer towards the average heading of local flockmates
cohesion: steer to move toward the average position (center of mass) of local flockmates
*/

public class Boid : MonoBehaviour
{
    private Vector3 separationVector = new Vector3(0, 0, 0);
    private Vector3 alignmentVector = new Vector3(0, 0, 0);
    private Vector3 cohesionVector = new Vector3(0, 0, 0);

    public Vector3 separationWeight = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 alignmentWeight = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 cohesionWeight = new Vector3(1.0f, 1.0f, 1.0f);

    public List<Boid> flock = new List<Boid>();
    private Vector3 currentHeading = new Vector3(0, 0, 0);
    private Vector3 newHeading = new Vector3(0, 0, 0);
    public float speed = 10.0f;
    public float currentHeadingWeight = 1.0f;

    void Start()
    {
        GetFlock();
        currentHeading = transform.forward;
    }

    class Heading
    {
        public Heading(Quaternion rotation, float weight)
        {
            this.rotation = rotation;
            this.weight = weight;
        }
        public Quaternion rotation;
        public float weight;
    };

    void Update()
    {

        // think
        separationVector = Separation();
        alignmentVector = Alignment();
        cohesionVector = Cohesion();

        // collect headings, but only use if non-zero
        var headings = new List<Heading>();
        if (currentHeadingWeight != 0.0f)
            headings.Add(new Heading(transform.rotation, currentHeadingWeight));
        if (separationVector != Vector3.zero)
            headings.Add(new Heading(Quaternion.LookRotation(separationVector), separationWeight.x));
        if (alignmentVector != Vector3.zero)
            headings.Add(new Heading(Quaternion.LookRotation(alignmentVector), alignmentWeight.x));
        if (cohesionVector != Vector3.zero)
            headings.Add(new Heading(Quaternion.LookRotation(cohesionVector), cohesionWeight.x));

        // normalize weights so they add up to one
        var totalWeight = 0.0f;
        foreach (var heading in headings)
            totalWeight += heading.weight;
        foreach (var heading in headings)
            heading.weight /= totalWeight;

        totalWeight = 0.0f;
        var newHeading = Quaternion.identity;

        foreach (var heading in headings)
        {
            totalWeight += heading.weight;
            newHeading = Quaternion.Slerp(newHeading, heading.rotation, heading.weight / totalWeight);
        }

        transform.rotation = newHeading;
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    // separation: steer to avoid crowding local flockmates
    Vector3 Separation()
    {
        Vector3 resultVector = new Vector3(0, 0, 0);

        for (int i = 0; i < flock.Count; i++)
        {
            Vector3 differenceVector = new Vector3(0, 0, 0);
            differenceVector = transform.position - flock[i].transform.position;

            float magnitude = differenceVector.magnitude;
            if (magnitude < 100.0f)
            {
                float weightedMagnitude = 1.0f / (magnitude * magnitude);
                differenceVector = Vector3.Scale(differenceVector, new Vector3(weightedMagnitude, weightedMagnitude, weightedMagnitude));
                resultVector = resultVector + differenceVector;
            }
        }
        return resultVector;
    }

    // alignment: steer towards the average heading of local flockmates
    Vector3 Alignment()
    {
        Vector3 resultVector = new Vector3(0, 0, 0);

        for (int i = 0; i < flock.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, flock[i].transform.position);
            Vector3 otherHeading = flock[i].transform.forward;

            resultVector = Vector3.Slerp(resultVector, otherHeading, (1.0f / (distance * distance)));
        }
        return resultVector;
    }

    // cohesion: steer to move toward the average position (center of mass) of local flockmates
    Vector3 Cohesion()
    {
        Vector3 resultVector = new Vector3(0, 0, 0);

        for (int i = 0; i < flock.Count; i++)
        {
            resultVector = resultVector + flock[i].transform.position;
        }
        resultVector = resultVector / (float)flock.Count;

        //print (resultVector.ToString());
        resultVector = resultVector - transform.position;

        return resultVector;
    }

    void GetFlock()
    {
        Boid[] boids = FindObjectsOfType(typeof(Boid)) as Boid[];

        // add all boids except for self
        for (int i = 0; i < boids.Length; i++)
        {
            int id1 = boids[i].GetInstanceID();
            Boid boid = gameObject.GetComponent(typeof(Boid)) as Boid;
            int id2 = boid.GetInstanceID();

            if (id1 != id2)
            {
                flock.Add(boids[i]);
                //print ( "me=" + gameObject.name + " adding=" + boids[i].name );
            }
        }
    }
}
