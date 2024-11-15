using UnityEngine;

public class ElasticRope : MonoBehaviour
{
    public CubeData cube;
    public Vector3 anchorPoint = new Vector3(0, 10, 0);
    public float restLength = 2.0f;
    public float stiffness = 20f;
    public float damping = 1f;
    private LineRenderer lineRenderer;

    void Start()
    {
        // Add or get the LineRenderer component
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.positionCount = 2;
        }
    }

    void FixedUpdate()
    {
        // Check if lineRenderer or cube is null to prevent NullReferenceException
        if (lineRenderer == null || cube == null)
        {
            Debug.LogWarning("LineRenderer or Cube is not set in ElasticRope.");
            return;
        }

        Vector3 direction = cube.position - anchorPoint;
        float currentDistance = direction.magnitude;

        float displacement = currentDistance - restLength;
        Vector3 springForce = -stiffness * displacement * direction.normalized;

        Vector3 dampingForce = -damping * cube.velocity;

        Vector3 totalForce = springForce + dampingForce;
        cube.velocity += totalForce / cube.mass * Time.fixedDeltaTime;

        Vector3 gravityForce = new Vector3(0, -9.81f, 0) * cube.mass;
        cube.velocity += gravityForce / cube.mass * Time.fixedDeltaTime;

        cube.position += cube.velocity * Time.fixedDeltaTime;
        cube.cubeObject.transform.position = cube.position;

        // Update LineRenderer to visually represent the rope
        lineRenderer.SetPosition(0, anchorPoint);    // Start point at the anchor
        lineRenderer.SetPosition(1, cube.position);  // End point at the cube’s position
    }
}
