using UnityEngine;
using System.Collections;

public class FreeFallWithRK4 : MonoBehaviour
{
    public Vector3 position;            // Position of the object
    public Vector3 velocity;            // Velocity of the object
    public float mass = 1.0f;           // Mass of the object
    public float gravity = -9.81f;      // Gravitational acceleration
    public float frictionCoefficient = 0.0f; // Friction coefficient (0 for testing)
    public float timeStep = 0.02f;      // Time step for the integration
    public float restitution = 1.0f;    // Coefficient of restitution for collision
    public float radius = 0.5f;         // Radius of the ball for collision detection

    public Transform ropeAnchor;        // The anchor point where the rope is attached (optional, for rope simulation)
    public float ropeLength = 10.0f;    // Length of the rope

    private Vector3 ropeDirection;      // Direction of the rope
    private float currentRopeLength;    // The current length of the rope during simulation

    void Start()
    {
        position = new Vector3(0, 10, 0);  // Initial position (10 units above the ground)
        velocity = Vector3.zero;            // Initial velocity is zero
        ropeDirection = Vector3.down;       // Assume the rope is vertical initially
        currentRopeLength = ropeLength;     // Set initial rope length
    }

    public void FixedUpdate()
    {
        // Compute the acceleration due to gravity, rope tension, and friction
        Vector3 acceleration = ComputeAcceleration(position, velocity);
        RK4Integration(acceleration);

        // Update position based on rope constraints (simulate rope)
        SimulateRopeConstraints();

        // Check for collision with the ground (for simple bounce simulation)
        if (position.y <= 0.0f)
        {
            HandleGroundCollision();
        }

        // Update the position in Unity
        transform.position = position;
    }

    Vector3 ComputeAcceleration(Vector3 pos, Vector3 vel)
    {
        // Calculate gravity and friction forces
        Vector3 gravityForce = mass * new Vector3(0, gravity, 0);
        Vector3 frictionForce = -frictionCoefficient * vel;

        // If the ball is attached to the rope, apply rope tension force
        Vector3 ropeForce = Vector3.zero;
        if (ropeAnchor != null)
        {
            ropeForce = (ropeAnchor.position - pos).normalized * (currentRopeLength - ropeLength) * 10f;  // Hooke's Law for rope tension
        }

        return (gravityForce + frictionForce + ropeForce) / mass;
    }

    void RK4Integration(Vector3 acceleration)
    {
        Vector3 k1_v = acceleration * timeStep;
        Vector3 k1_x = velocity * timeStep;

        Vector3 k2_v = ComputeAcceleration(position + k1_x * 0.5f, velocity + k1_v * 0.5f) * timeStep;
        Vector3 k2_x = (velocity + k1_v * 0.5f) * timeStep;

        Vector3 k3_v = ComputeAcceleration(position + k2_x * 0.5f, velocity + k2_v * 0.5f) * timeStep;
        Vector3 k3_x = (velocity + k2_v * 0.5f) * timeStep;

        Vector3 k4_v = ComputeAcceleration(position + k3_x, velocity + k3_v) * timeStep;
        Vector3 k4_x = (velocity + k3_v) * timeStep;

        velocity += (k1_v + 2 * k2_v + 2 * k3_v + k4_v) / 6;
        position += (k1_x + 2 * k2_x + 2 * k3_x + k4_x) / 6;
    }

    void HandleGroundCollision()
    {
        // Correct position to stay on the ground and reverse the velocity on the y-axis
        position.y = 0;
        velocity.y = -restitution * velocity.y;
    }

    // Rope simulation
    void SimulateRopeConstraints()
    {
        if (ropeAnchor != null)
        {
            // Calculate current rope length from anchor to position
            currentRopeLength = (ropeAnchor.position - position).magnitude;

            // If the ball moves past the rope length, we apply rope constraints
            if (currentRopeLength > ropeLength)
            {
                // Apply constraint by pulling the ball back toward the rope anchor
                Vector3 directionToAnchor = (ropeAnchor.position - position).normalized;
                position = ropeAnchor.position - directionToAnchor * ropeLength;
            }
        }
    }

    // Handle collisions with other balls
    public void HandleCollisionWithBall(FreeFallWithRK4 otherBall)
    {
        // Calculate the vector between the two balls
        Vector3 relativePosition = otherBall.position - position;
        float distance = relativePosition.magnitude;

        // Check if balls are colliding (distance <= sum of radii)
        if (distance <= radius + otherBall.radius)
        {
            // Calculate the normal vector for the collision response
            Vector3 normal = relativePosition.normalized;

            // Calculate the relative velocity along the normal vector
            float relativeVelocity = Vector3.Dot(velocity - otherBall.velocity, normal);

            // If the balls are moving towards each other, handle the collision
            if (relativeVelocity < 0)
            {
                // Calculate the impulse scalar based on the masses and relative velocity
                float impulse = (2 * relativeVelocity) / (mass + otherBall.mass);

                // Apply the impulse to both balls' velocities
                velocity -= impulse * otherBall.mass * normal;
                otherBall.velocity += impulse * mass * normal;

                // Separate the balls slightly to prevent overlap
                float overlap = radius + otherBall.radius - distance;
                position -= normal * overlap * 0.5f;
                otherBall.position += normal * overlap * 0.5f;
            }
        }
    }
}
