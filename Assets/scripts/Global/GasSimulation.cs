using System.Collections.Generic;
using UnityEngine;

public class GasSimulation : MonoBehaviour
{
    public int numSpheres = 20;
    public float boxSize = 10f;
    public float sphereRadius = 0.5f;
    public float maxInitialVelocity = 5f;
    private List<Sphere> spheres = new List<Sphere>();

    void Start()
    {
        for (int i = 0; i < numSpheres; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-boxSize / 2 + sphereRadius, boxSize / 2 - sphereRadius),
                Random.Range(-boxSize / 2 + sphereRadius, boxSize / 2 - sphereRadius),
                Random.Range(-boxSize / 2 + sphereRadius, boxSize / 2 - sphereRadius)
            );

            Vector3 velocity = new Vector3(
                Random.Range(-maxInitialVelocity, maxInitialVelocity),
                Random.Range(-maxInitialVelocity, maxInitialVelocity),
                Random.Range(-maxInitialVelocity, maxInitialVelocity)
            );

            spheres.Add(new Sphere(position, velocity, sphereRadius, 1f));
        }
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        foreach (Sphere sphere in spheres)
        {
            sphere.UpdatePosition(deltaTime);
            HandleWallCollisions(sphere);
        }

        List<(Sphere, Sphere)> potentialCollisions = SweepAndPrune();
        ResolveCollisions(potentialCollisions);
    }

    private void HandleWallCollisions(Sphere sphere)
    {
        //if (sphere.position.x - sphere.radius < -boxSize / 2 || sphere.position.x + sphere.radius > boxSize / 2)
        //    sphere.velocity.x *= -1;

        //if (sphere.position.y - sphere.radius < -boxSize / 2 || sphere.position.y + sphere.radius > boxSize / 2)
        //    sphere.velocity.y *= -1;

        //if (sphere.position.z - sphere.radius < -boxSize / 2 || sphere.position.z + sphere.radius > boxSize / 2)
        //    sphere.velocity.z *= -1;

        // Vérifie collision avec les parois X
        if (sphere.position.x - sphere.radius < -boxSize / 2)
        {
            sphere.position.x = -boxSize / 2 + sphere.radius; // Replace à la surface
            sphere.velocity.x *= -1; // Inverse la vitesse
        }
        else if (sphere.position.x + sphere.radius > boxSize / 2)
        {
            sphere.position.x = boxSize / 2 - sphere.radius; // Replace à la surface
            sphere.velocity.x *= -1;
        }

        // Vérifie collision avec les parois Y
        if (sphere.position.y - sphere.radius < -boxSize / 2)
        {
            sphere.position.y = -boxSize / 2 + sphere.radius;
            sphere.velocity.y *= -1;
        }
        else if (sphere.position.y + sphere.radius > boxSize / 2)
        {
            sphere.position.y = boxSize / 2 - sphere.radius;
            sphere.velocity.y *= -1;
        }

        // Vérifie collision avec les parois Z
        if (sphere.position.z - sphere.radius < -boxSize / 2)
        {
            sphere.position.z = -boxSize / 2 + sphere.radius;
            sphere.velocity.z *= -1;
        }
        else if (sphere.position.z + sphere.radius > boxSize / 2)
        {
            sphere.position.z = boxSize / 2 - sphere.radius;
            sphere.velocity.z *= -1;
        }

        // Met à jour la position visuelle de la sphère après correction
        sphere.visualObject.transform.position = sphere.position;
    }

    private List<(Sphere, Sphere)> SweepAndPrune()
    {
        List<(Sphere, Sphere)> potentialCollisions = new List<(Sphere, Sphere)>();
        spheres.Sort((a, b) => a.position.x.CompareTo(b.position.x));

        for (int i = 0; i < spheres.Count; i++)
        {
            for (int j = i + 1; j < spheres.Count; j++)
            {
                if (spheres[j].position.x - spheres[i].position.x > spheres[i].radius + spheres[j].radius)
                    break;

                if (Mathf.Abs(spheres[i].position.y - spheres[j].position.y) < (spheres[i].radius + spheres[j].radius) &&
                    Mathf.Abs(spheres[i].position.z - spheres[j].position.z) < (spheres[i].radius + spheres[j].radius))
                {
                    potentialCollisions.Add((spheres[i], spheres[j]));
                }
            }
        }

        return potentialCollisions;
    }

    private void ResolveCollisions(List<(Sphere, Sphere)> potentialCollisions)
    {
        foreach (var (s1, s2) in potentialCollisions)
        {
            if (IsColliding(s1, s2))
            {
                Vector3 normal = (s2.position - s1.position).normalized;
                Vector3 relativeVelocity = s2.velocity - s1.velocity;
                float velocityAlongNormal = Vector3.Dot(relativeVelocity, normal);

                if (velocityAlongNormal > 0)
                    continue;

                float impulseMagnitude = -2 * velocityAlongNormal / (s1.mass + s2.mass);
                Vector3 impulse = impulseMagnitude * normal;

                s1.velocity -= impulse / s1.mass;
                s2.velocity += impulse / s2.mass;
            }
        }
    }

    private bool IsColliding(Sphere s1, Sphere s2)
    {
        float distance = Vector3.Distance(s1.position, s2.position);
        return distance < s1.radius + s2.radius;
    }
}
