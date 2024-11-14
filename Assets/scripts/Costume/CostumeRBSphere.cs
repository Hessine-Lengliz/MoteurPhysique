using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CustomRBSphere : MonoBehaviour
{
    // Rigidbody-like properties
    public Vector3 velocity = Vector3.zero;
    public Vector3 angularVelocity = Vector3.zero;
    public float mass = 1f;  // Mass of the ball
    public float radius = 0.5f; // Radius of the ball for rolling
    public float friction = 0.1f;  // Rolling friction
    public Vector3 position = Vector3.zero;
    public Vector3 rotation = Vector3.zero;
   
    // Ramp variables
    public Transform startPoint;
    public Transform endPoint;
    private Vector3[] vertices;
    private Mesh mesh;

    // Gravity
    public float gravity = 9.81f;  // Gravity strength
    public float drag = 0.05f;  // Linear drag to simulate air resistance

    void Awake()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        vertices = new Vector3[] {
            new Vector3(-1, -1, -1),
            new Vector3( 1, -1, -1),
            new Vector3( 1,  1, -1),
            new Vector3(-1,  1, -1),
            new Vector3(-1, -1,  1),
            new Vector3( 1, -1,  1),
            new Vector3( 1,  1,  1),
            new Vector3(-1,  1,  1)
        };

        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = new int[] {
            0, 2, 1, 0, 3, 2, 4, 5, 6, 4, 6, 7,
            0, 7, 3, 0, 4, 7, 1, 2, 6, 1, 6, 5,
            0, 1, 5, 0, 5, 4, 3, 7, 6, 3, 6, 2
        };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }

    void Update()
    {
        ApplyGravity();
        ApplyRollingMotion();
        ApplyAirResistance();
        UpdatePositionAndRotation();
    }

    void ApplyGravity()
    {
        // Calculate the gravitational force
        Vector3 gravityForce = Vector3.down * gravity * mass;
        velocity += gravityForce * Time.deltaTime;  // Update velocity with gravitational force
    }

    void ApplyRollingMotion()
    {
        // Simple rolling physics based on velocity and the friction coefficient
        Vector3 rampDirection = (endPoint.position - startPoint.position).normalized;
        float angle = Vector3.Angle(rampDirection, Vector3.up);  // Angle of the ramp with respect to the vertical

        // Calculate acceleration along the ramp due to gravity
        Vector3 acceleration = rampDirection * Mathf.Sin(angle * Mathf.Deg2Rad) * gravity;

        // Update velocity based on the ramp slope
        velocity += acceleration * Time.deltaTime;

        // Rolling resistance (friction)
        velocity *= (1 - friction * Time.deltaTime);  // Decelerate the ball over time due to friction

        // Apply angular velocity (rotational movement)
        angularVelocity = velocity / radius;  // Simple relation between linear velocity and angular velocity

        // Apply rotational drag (simulate the loss of energy in the rotation)
        angularVelocity *= (1 - friction * Time.deltaTime);  // Decelerate rotation due to friction

        // Rotate the ball based on angular velocity
        rotation += angularVelocity * Time.deltaTime;
    }

    void ApplyAirResistance()
    {
        // Apply drag force to slow down the ball's velocity
        velocity *= 1 - drag * Time.deltaTime;
    }

    void UpdatePositionAndRotation()
    {
        // Update the ball's position based on its velocity
        position += velocity * Time.deltaTime;

        // Apply position change to the mesh (you can use this for custom meshes)
        transform.position = position;

        // Apply rotation to the mesh based on angular velocity
        transform.Rotate(Vector3.right, angularVelocity.magnitude * Time.deltaTime);  // Rotate around the X-axis (ball axis)
    }
}
