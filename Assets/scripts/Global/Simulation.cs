using UnityEngine;

public class Simulation : MonoBehaviour
{
    public float damping = 0.1f; // Coefficient de frottement
    public float stiffness = 1.0f; // Constante de rappel du ressort
    public float mass = 1.0f; // Masse du cube
    public Vector3 initialPosition;
    public Vector3 initialVelocity;

    private Vector3 position;
    private Vector3 velocity;

    void Start()
    {
        position = initialPosition;
        velocity = initialVelocity;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        Vector3 gravity = new Vector3(0, -9.81f, 0) * mass;
        Vector3 springForce = -stiffness * (position - initialPosition); // Force de rappel du ressort
        Vector3 dampingForce = -damping * velocity;
        Vector3 netForce = gravity + springForce + dampingForce;
        Vector3 acceleration = netForce / mass;

        // Appel de la méthode RK4SolverMethod de la classe utilitaire
        (position, velocity) = RK4Utility.RK4SolverMethod(position, velocity, acceleration, dt);

        transform.position = position;
    }
}
