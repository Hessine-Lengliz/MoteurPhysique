using UnityEngine;

public class Spring
{
    public CubeData cubeA;           // Premier cube
    public CubeData cubeB;           // Second cube
    public float restLength;     // Longueur de repos du ressort
    public float stiffness;      // Constante de raideur (k)
    public float damping;        // Amortissement

    public Spring(CubeData a, CubeData b, float restLength, float stiffness, float damping)
    {
        this.cubeA = a;
        this.cubeB = b;
        this.restLength = restLength;
        this.stiffness = stiffness;
        this.damping = damping;
    }

    public void ApplyForce()
    {
        // Calculer la direction et la distance entre les deux cubes
        Vector3 direction = cubeB.position - cubeA.position;
        float currentDistance = direction.magnitude;

        // Calcul de la force de rappel selon la loi de Hooke
        float displacement = currentDistance - restLength;
        Vector3 springForce = -stiffness * displacement * direction.normalized;

        // Calcul de l'amortissement
        Vector3 relativeVelocity = cubeB.velocity - cubeA.velocity;
        Vector3 dampingForce = -damping * relativeVelocity;

        // Appliquer les forces résultantes sur les deux cubes
        cubeA.velocity += (springForce + dampingForce) / cubeA.mass * Time.fixedDeltaTime;
        cubeB.velocity -= (springForce + dampingForce) / cubeB.mass * Time.fixedDeltaTime;
    }
}
