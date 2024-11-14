using UnityEngine;
public class Spring : MonoBehaviour
{
    public float SpringConstant ;
    public float EquilibriumLength ;
    public float DampingCoefficient ;

    public Vector3 CalculateForce(Vector3 massPosition)
    {
        // Calculate spring force based on Hooke's law
        float stretch = Vector3.Distance(massPosition, Vector3.zero) - EquilibriumLength;
        return -SpringConstant * stretch * Vector3.Normalize(massPosition) - DampingCoefficient * massPosition;
    }
}