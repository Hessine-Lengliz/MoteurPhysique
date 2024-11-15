using UnityEngine;
using System.Collections.Generic;

public class PhysicsEngine : MonoBehaviour
{
    public GameObject cubePrefab;                    // Prefab for the main (big) cube
    public GameObject smallCubePrefab;               // Prefab for the small cube
    public Vector3 anchorStartPoint = new Vector3(0, 10, 0); // Start point of the first anchor
    public float anchorSpacing = 2.0f;               // Horizontal spacing between each anchor point
    public float restLength = 2.0f;                  // Rest length of the rope for big cubes
    public float smallRestLength = 2.5f;             // Adjust this value to increase separation between small and big cubes
    public float stiffness = 20f;                    // Stiffness (spring constant) of the rope
    public float damping = 1f;                       // Damping factor to reduce oscillation

    public List<CubeData> cubes = new List<CubeData>();  // List of CubeData for each cube
    private List<Vector3> anchorPoints = new List<Vector3>(); // List of anchor points for each cube

    void Start()
    {
        anchorPoints.Clear();

        if (cubes.Count == 0)
        {
            Debug.LogError("Cubes list is empty. Please assign CubeData elements in the Inspector.");
            return;
        }

        for (int i = 0; i < cubes.Count; i++)
        {
            Vector3 anchorPoint = anchorStartPoint + new Vector3(i * anchorSpacing, 0, 0);
            anchorPoints.Add(anchorPoint);

            cubes[i].position = anchorPoint;
            cubes[i].velocity = Vector3.zero;

            cubes[i].cubeObject = Instantiate(cubePrefab, cubes[i].position, Quaternion.identity);
            LineRenderer lineRenderer = cubes[i].cubeObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.positionCount = 2;
            Material ropeMaterial = new Material(Shader.Find("Unlit/Color"));
            ropeMaterial.color = Color.black;
            lineRenderer.material = ropeMaterial;
            cubes[i].lineRenderer = lineRenderer;

            if (cubes[i].hasSmallCube && smallCubePrefab != null)
            {
                // Position the small cube even higher above the main cube using the increased smallRestLength
                cubes[i].smallCubePosition = cubes[i].position + new Vector3(0, smallRestLength, 0);
                cubes[i].smallCubeVelocity = Vector3.zero;

                // Instantiate the small cube at the adjusted position
                cubes[i].smallCubeObject = Instantiate(smallCubePrefab, cubes[i].smallCubePosition, Quaternion.identity);
                LineRenderer smallLineRenderer = cubes[i].smallCubeObject.AddComponent<LineRenderer>();
                smallLineRenderer.startWidth = 0.05f;
                smallLineRenderer.endWidth = 0.05f;
                smallLineRenderer.positionCount = 2;
                smallLineRenderer.material = ropeMaterial;

                cubes[i].smallCubeLineRenderer = smallLineRenderer;
            }
        }
    }

    void FixedUpdate()
    {
        if (cubes.Count != anchorPoints.Count)
        {
            Debug.LogError("Mismatch between cubes and anchor points count. Check initialization.");
            return;
        }

        for (int i = 0; i < cubes.Count; i++)
        {
            ApplyGravity(i, mainCube: true);          // Apply gravity to the main cube
            ApplySpringForce(i, mainCube: true);      // Apply spring force to the main cube
            UpdatePosition(i, mainCube: true);        // Update the main cube's position

            cubes[i].cubeObject.transform.position = cubes[i].position;
            cubes[i].lineRenderer.SetPosition(0, anchorPoints[i]);
            cubes[i].lineRenderer.SetPosition(1, cubes[i].position);

            if (cubes[i].hasSmallCube && cubes[i].smallCubeObject != null)
            {
                ApplyGravity(i, mainCube: false);         // Apply gravity to the small cube
                ApplySpringForce(i, mainCube: false);     // Apply spring force to the small cube
                UpdatePosition(i, mainCube: false);       // Update the small cube's position

                cubes[i].smallCubeObject.transform.position = cubes[i].smallCubePosition;
                cubes[i].smallCubeLineRenderer.SetPosition(0, cubes[i].position);
                cubes[i].smallCubeLineRenderer.SetPosition(1, cubes[i].smallCubePosition);
            }
        }
    }

    private void ApplyGravity(int index, bool mainCube)
    {
        Vector3 gravityForce = new Vector3(0, -9.81f, 0) * (mainCube ? cubes[index].mass : cubes[index].smallCubeMass);
        if (mainCube)
            cubes[index].velocity += gravityForce / cubes[index].mass * Time.fixedDeltaTime;
        else
            cubes[index].smallCubeVelocity += gravityForce / cubes[index].smallCubeMass * Time.fixedDeltaTime;
    }

    private void ApplySpringForce(int index, bool mainCube)
    {
        Vector3 direction;
        float currentDistance;
        float displacement;
        Vector3 springForce;

        if (mainCube)
        {
            direction = cubes[index].position - anchorPoints[index];
            currentDistance = direction.magnitude;
            displacement = currentDistance - restLength;
            springForce = -stiffness * displacement * direction.normalized;
            cubes[index].velocity += (springForce - damping * cubes[index].velocity) / cubes[index].mass * Time.fixedDeltaTime;
        }
        else
        {
            direction = cubes[index].smallCubePosition - cubes[index].position;
            currentDistance = direction.magnitude;
            displacement = currentDistance - smallRestLength;
            springForce = -stiffness * displacement * direction.normalized;
            cubes[index].smallCubeVelocity += (springForce - damping * cubes[index].smallCubeVelocity) / cubes[index].smallCubeMass * Time.fixedDeltaTime;
        }
    }

    private void UpdatePosition(int index, bool mainCube)
    {
        if (mainCube)
            cubes[index].position += cubes[index].velocity * Time.fixedDeltaTime;
        else
            cubes[index].smallCubePosition += cubes[index].smallCubeVelocity * Time.fixedDeltaTime;
    }
}
