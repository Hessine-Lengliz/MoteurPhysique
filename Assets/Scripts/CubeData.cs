using UnityEngine;

[System.Serializable]
public class CubeData
{
    public float mass = 1f;                          // Mass of the main (big) cube
    public bool hasSmallCube = false;                // Flag to decide if a small cube should be added
    public float smallCubeMass = 0.5f;               // Mass of the small cube

    [HideInInspector] public Vector3 position;       // Position of the main cube
    [HideInInspector] public Vector3 velocity;       // Velocity of the main cube
    [HideInInspector] public GameObject cubeObject;  // Instantiated main cube GameObject
    [HideInInspector] public LineRenderer lineRenderer; // LineRenderer for the rope of the main cube

    [HideInInspector] public Vector3 smallCubePosition;  // Position of the small cube
    [HideInInspector] public Vector3 smallCubeVelocity;  // Velocity of the small cube
    [HideInInspector] public GameObject smallCubeObject; // Instantiated small cube GameObject
    [HideInInspector] public LineRenderer smallCubeLineRenderer; // LineRenderer for the rope of the small cube
}
